using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PackageGenerator {
    [MenuItem("Window/Miyamasu Test Runner Editor/Update UnityPackage", false, 21)] public static void UnityPackage () {
        // remove generated entry points.
        Directory.Delete("Assets/MiyamasuTestRunner/Runtime/Generated", true);

        var settings = Miyamasu.Settings.LoadSettings();
        File.Delete("Assets/MiyamasuTestRunner/Runtime/Resources/MiyamasuSettings.txt");
        Miyamasu.Settings.WriteSettings(new Miyamasu.RunnerSettings());

        var assetPaths = new List<string>();
        var packageSourcePath = "Assets/MiyamasuTestRunner";
        
        CollectPathsRecursively(packageSourcePath, assetPaths);
        
        AssetDatabase.ExportPackage(assetPaths.ToArray(), "MiyamasuTestRunner.unitypackage", ExportPackageOptions.IncludeDependencies);
        Miyamasu.Settings.WriteSettings(settings);
    }

    private static void CollectPathsRecursively (string path, List<string> assetPaths) {
        var filePaths = Directory.GetFiles(path);
        assetPaths.AddRange(filePaths);

        var dirPaths = Directory.GetDirectories(path);
        foreach (var dir in dirPaths) {
            CollectPathsRecursively(dir, assetPaths);
        }        
    }
}