# DDLC+ Custom Backgrounds
A BepInEx Plugin for DDLC+ which allows you to use custom desktop backgrounds

# Installation
Copy the DDLC-Custom-Backgrounds.dll file into the `BepInEx/plugins` folder. 

# Build instructions
Copy the following files to the lib folder:
* all the files from `Doki Doki Literature Club Plus_Data\Managed` from your DDLC+ installation
* BepInEx.dll (from https://github.com/BepInEx/BepInEx/releases)
Then use `dotnet publish` to build the dll. The built dll will be in `bin\Debug\netstandard2.0\publish` directory.

# Usage
Place your .png or .jpg image in the `BepInEx/plugins/Gallery` folder, then use the left and right arrow keys to change your background
