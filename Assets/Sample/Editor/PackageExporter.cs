using UnityEditor;
using System.IO;
using UnityEngine;

namespace Sample.Editor
{
    /// <summary>
    /// A helper class with a static method to export the SDK as a .unitypackage.
    /// This is designed to be called by a command-line build process (like GitHub Actions).
    /// </summary>
    public static class PackageExporter
    {
        /// <summary>
        /// The menu item allows for manual triggering from the Unity Editor.
        /// The static method is what the CI process will call.
        /// </summary>
        [MenuItem("Rollic SDK/Export Package")]
        public static void Export()
        {
            string sdkFolderPath = "Assets/RollicSDK";
            string outputDirectory = "Build";
            string outputFileName = "RollicSDK.unitypackage";
            string fullOutputPath = Path.Combine(outputDirectory, outputFileName);

            // Ensure the output directory exists
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            Debug.Log($"Starting package export to '{fullOutputPath}'...");

            AssetDatabase.ExportPackage(
                sdkFolderPath,
                fullOutputPath,
                ExportPackageOptions.Recurse
            );

            Debug.Log("Package export successful!");
        }
    }
}