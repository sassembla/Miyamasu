using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class PackageGenerator {
    [MenuItem("Window/Miyamasu Test Runner Editor/Update UnityPackage", false, 21)] public static void UnityPackage () {
        var assetPaths = new List<string>();
        var packageSourcePath = "Assets/MiyamasuTestRunner";
        
        CollectPathsRecursively(packageSourcePath, assetPaths);

        AssetDatabase.ExportPackage(assetPaths.ToArray(), "MiyamasuTestRunner.unitypackage", ExportPackageOptions.IncludeDependencies);
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