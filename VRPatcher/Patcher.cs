/*MIT License

Copyright (c) 2020 Ricardo Lopes

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using AssetsTools.NET;
using AssetsTools.NET.Extra;

public static class Patcher
{
    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

    public static void Patch(AssemblyDefinition assembly)
    {
    }

    public static void Initialize()
    {
        Console.WriteLine("The VR Patcher was made by Raicuparta");
        Console.WriteLine("Patching We Were in VR...");

        var installerPath = Assembly.GetExecutingAssembly().Location;
        Console.WriteLine("installerPath " + installerPath);

        var gameExePath = Process.GetCurrentProcess().MainModule.FileName;
        Console.WriteLine("gameExePath " + gameExePath);

        var gamePath = Path.GetDirectoryName(gameExePath);
        var gameName = Path.GetFileNameWithoutExtension(gameExePath);
        var dataPath = Path.Combine(gamePath, $"{gameName}_Data/");
        var gameManagersPath = Path.Combine(dataPath, $"globalgamemanagers");
        var gameManagersBackupPath = CreateGameManagersBackup(gameManagersPath);
        var patcherPath = Path.GetDirectoryName(installerPath);
        var classDataPath = Path.Combine(patcherPath, "classdata.tpk");

        PatchVR(gameManagersBackupPath, gameManagersPath, classDataPath);

        Console.WriteLine($"");
        Console.WriteLine("Installed successfully, probably.");
    }

    private static string CreateGameManagersBackup(string gameManagersPath)
    {
        Console.WriteLine($"Backing up '{gameManagersPath}'...");
        var backupPath = gameManagersPath + ".bak";
        if (File.Exists(backupPath))
        {
            Console.WriteLine($"Backup already exists.");
            return backupPath;
        }

        File.Copy(gameManagersPath, backupPath);
        Console.WriteLine($"Created backup in '{backupPath}'");
        return backupPath;
    }

    private static void PatchVR(string gameManagersBackupPath, string gameManagersPath, string classDataPath)
    {
        Console.WriteLine($"Using classData file from path '{classDataPath}'");

        var am = new AssetsManager();
        am.LoadClassPackage(classDataPath);
        var ggm = am.LoadAssetsFile(gameManagersBackupPath, false);
        var ggmFile = ggm.file;
        var ggmTable = ggm.table;
        am.LoadClassDatabaseFromPackage(ggmFile.typeTree.unityVersion);

        var replacers = new List<AssetsReplacer>();

        var buildSettings = ggmTable.GetAssetInfo(11);
        var buildSettingsBase = am.GetATI(ggmFile, buildSettings).GetBaseField();
        var enabledVRDevices = buildSettingsBase.Get("enabledVRDevices").Get("Array");
        var stringTemplate = enabledVRDevices.templateField.children[1];
        var vrDevicesList = new[] { StringField("OpenVR", stringTemplate) };
        enabledVRDevices.SetChildrenList(vrDevicesList);

        replacers.Add(new AssetsReplacerFromMemory(0, buildSettings.index, (int)buildSettings.curFileType, 0xffff,
            buildSettingsBase.WriteToByteArray()));

        using (var writer = new AssetsFileWriter(File.OpenWrite(gameManagersPath)))
        {
            ggmFile.Write(writer, 0, replacers, 0);
        }
    }

    private static AssetTypeValueField StringField(string str, AssetTypeTemplateField template)
    {
        return new AssetTypeValueField()
        {
            children = null,
            childrenCount = 0,
            templateField = template,
            value = new AssetTypeValue(EnumValueTypes.ValueType_String, str)
        };
    }
}
