# WeWereInVR
### ATTENTION: Setting or entering lobby passwords currently doesn't work!!!


# About
 WeWereInVR is an unofficial VR mod for the game We Were Here. 
 The mod is a passion project of mine and I made it because I love the flatscreen game and thought it would be a perfect fit for VR.
 This project uses BepInEx.
 For more infos visit https://sites.google.com/view/wewereinvr/about

# Installing The Mod
 For installation instructions go to https://sites.google.com/view/wewereinvr/installation-instructions

# Building the Project:
## Requirements: -Visual Studio 2022
	       -AssetTools 2.X.X
	       -BepInEx 5.4
	       -We Were Here
	       -Unity Version 2018.4.12f1 or alternatively only the correct openvr_api.dll (located in (Pre-Early-Release)GameFolderFiles
 
## How To:
 
1. Open up SteamVRInject.sln in Visual Studio 2022/n
2. add any missing references(if any) //I just noticed I still left "using Valve.VR" in the code. This will throw an error if SteamVR.dll isn't present(which it isn't anymore). Just delete that line
3. click on "Build SteamVR Inject"
4. Download BepInEx 5.4 from https://github.com/BepInEx/BepInEx
5. Extract all BepInEx files into the root directory of your We Were Here installation
6. Run WeWereHere.exe once
7. copy openvr_api.dll to WeWereHere/WeWereHere_data/Plugins
8. Copy the SteamVRInject.dll/WeWereinVR.dll you built earlier(located under WeWereInVR/SteamVRInject/bin/debug) to (YourWeWereHereGameDirectory)/BepInEx/Plugins
9. In the same directory create a .txt file called "Settings.txt"
10. Open it in a text editor of your choice
11. In the first line write 1,74  //this is the in-game characters height, NOT your height.
12. In the second line write your VR headsets refresh rate in hz
13. In the third line write your VR headsets controller manufacturer(for Oculus/Meta Touch controllers write "Oculus/Meta", for HTC Vive Wands write "Vive", for anything else write "Custom")
14. if your controllers aren't yet natively supported by the mod create, in the same directory, a .txt file called "Bindings.txt"
15. find out the unity keycodes of your Trigger, Grip Button, Primary Button A, Primary Button B and right Thumbstick/Touchpad press
16. Into the first line of "Bindings.txt" write your GripButtons Keycode
17. Into the second your trigger button
18. Into the third your Primary Button A
19. Into the fourth your Primary Button B
20. And into the last your thumbstick press(this is your jump button)
21. Copy everything located under WeWereInVR/VRPatcher/bin/debug/netstandard2.0 to (YourWeWereHereGameDirectory)/BepInEx/Patchers
22. Copy netstandard2.0.dll (WeWereHere/(Pre-Early-Release)GameFolderFiles/netstandard2.0.dll) to (YourWeWereHereGameDirectory)/BepInEx/Patchers
23. Copy AssetTool.NET.dll (WeWereHere/(Pre-Early-Release)GameFolderFiles/AssetTools/AssetTool.NET.dll) to (YourWeWereHereGameDirectory)/BepInEx/Patchers
24. If I haven't forgotten anything, you should now be able to just start WeWereHere from steam and it should open in VR
 

