using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using RenpyLauncher;
using UnityEngine;
using UnityEngine.UI;

namespace DDLC_Custom_Backgrounds
{
    [BepInPlugin("com.wackery.custombackground", "Custom Backgrounds DDLC", "1.0.0.0")]
    public class CustomBackgrounds : BaseUnityPlugin
    {
        public static List<CustomBackground> customBackgrounds;
        public static int selectedBackground = -1;
        public static string GalleryFolder;
        public static string SaveFile;
        void Awake()
        {
            GalleryFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Gallery");
            SaveFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "gallerysave.txt");
            if (!Directory.Exists(GalleryFolder))
            {
                Directory.CreateDirectory(GalleryFolder);
            }
            LoadAllBackgrounds();
            if (File.Exists(SaveFile))
            {
                Load(false);
            }
            else
            {
                Save();
            }
            var harmony = new Harmony("com.wackery.custombackgroundpatch");
            harmony.PatchAll();
        }

        public static void Save()
        {
            if (selectedBackground == -1)
            {
                File.WriteAllText(SaveFile, "0");
            }
            else
            {
                File.WriteAllText(SaveFile, "1" + customBackgrounds[selectedBackground].filename);
            }
        }
        public static void Load(bool refreshBackground)
        {
            var data = File.ReadAllText(SaveFile);
            if (!data.StartsWith("1"))
            {
                return;
            }
            data = data.Substring(1);
            int i = 0;
            foreach (CustomBackground background in customBackgrounds)
            {
                if (background.filename == data)
                {
                    selectedBackground = i;
                }
                i++;
            }
            if (refreshBackground)
            {
                ActivateBackground();
            }
        }
        void Update()
        {
            if (!Renpy.IsInitialized())
            {
                return;
            }
            if (Renpy.LauncherMain.ActiveAppId != LauncherAppId.Desktop)
            {
                return;
            }
            if (Input.GetKeyDown("left"))
            {
                if (selectedBackground == -1)
                {
                    selectedBackground = customBackgrounds.Count-1;
                }
                else
                {
                    selectedBackground -= 1;
                }
                ActivateBackground();
            }
            else if (Input.GetKeyDown("right"))
            {
                if (selectedBackground == customBackgrounds.Count-1)
                {
                    selectedBackground = -1;
                }
                else
                {
                    selectedBackground += 1;
                }
                ActivateBackground();
            }
        }

        public static void LoadBackground(string filename)
        {
            CustomBackground background = new CustomBackground();
            background.filename = filename;
            background.fullfilename = Path.Combine(GalleryFolder,filename);
            Texture2D tex = new Texture2D(2, 2);
            if (File.Exists(background.fullfilename))
            {
                var FileData = File.ReadAllBytes(background.fullfilename);
                if (tex.LoadImage(FileData)) {
                    background.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 100f);
                    customBackgrounds.Add(background);
                }

            }

        }
        internal void LoadAllBackgrounds()
        {
            customBackgrounds = new List<CustomBackground>();
            var backgrounds = Directory.GetFiles(GalleryFolder);
            foreach (string background in backgrounds)
            {
                Logger.LogInfo($"Loading Background {Path.GetFileName(background)}");
                LoadBackground(Path.GetFileName(background));
                Logger.LogInfo("Complete!");
            }
        }
        private static void setBackground(CustomBackground background)
        {
            GameObject.Find("CustomWallpaper").GetComponent<Image>().sprite = background.sprite;
        }
        internal static void ActivateBackground()
        {
            setBackground(selectedBackground);
        }
        public static void setBackground(int backgroundID)
        {
            selectedBackground = backgroundID;
            if (backgroundID == -1) {
                Renpy.LauncherMain.CheckForWallpaperChange();
            }
            else {
                setBackground(customBackgrounds[selectedBackground]);
                Save();
            }
        }
    }
    [HarmonyPatch(typeof(GalleryApp))]
    [HarmonyPatch("ConfirmWallpaper")]
    class CustomWallpaperPatch
    {
        public static void Postfix()
        {
            if (CustomBackgrounds.selectedBackground == -1)
            {
                return;
            }
            CustomBackgrounds.setBackground(-1);
            
        }
    }
    [HarmonyPatch(typeof(DesktopApp))]
    [HarmonyPatch("OnAppUpdate")]
    class CustomWallpaperPatchAlive
    {
        static void Postfix()
        {
            if (CustomBackgrounds.selectedBackground == -1)
            {
                return;
            }
            CustomBackgrounds.ActivateBackground();

        }
    }
}
