using System.IO;
using UnityEngine;
using UnityEditor;

namespace YooAsset.Editor
{
    public class PackageImporterWindow : EditorWindow
    {
        static PackageImporterWindow _thisInstance;

        [MenuItem("YooAsset/Extension/补丁包导入工具", false, 101)]
        static void ShowWindow()
        {
            if (_thisInstance == null)
            {
                _thisInstance = EditorWindow.GetWindow(typeof(PackageImporterWindow), false, "补丁包导入工具", true) as PackageImporterWindow;
                _thisInstance.minSize = new Vector2(800, 600);
            }
            _thisInstance.Show();
        }

        private string _manifestPath = string.Empty;
        private string _packageName = "DefaultPackage";

        private void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("选择补丁包", GUILayout.MaxWidth(150)))
            {
                string resultPath = EditorUtility.OpenFilePanel("Find", "Assets/", "bytes");
                if (string.IsNullOrEmpty(resultPath))
                    return;
                _manifestPath = resultPath;
            }
            EditorGUILayout.LabelField(_manifestPath);
            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(_manifestPath) == false)
            {
                if (GUILayout.Button("导入补丁包（全部文件）", GUILayout.MaxWidth(150)))
                {
                    string streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
                    EditorTools.ClearFolder(streamingAssetsRoot);
                    CopyPackageFiles(_manifestPath);
                }
            }
        }

        private void CopyPackageFiles(string manifestFilePath)
        {
            string outputDirectory = Path.GetDirectoryName(manifestFilePath);
            string manifestFileName = Path.GetFileName(manifestFilePath);
            
            if (string.IsNullOrEmpty(outputDirectory) || !Directory.Exists(outputDirectory))
            {
                Debug.LogError($"无效的目录路径: {outputDirectory}");
                return;
            }

            string streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            string targetDirectory = $"{streamingAssetsRoot}/{_packageName}";
            
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            int fileCount = 0;
            
            var files = Directory.GetFiles(outputDirectory);
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string extension = Path.GetExtension(fileName).ToLower();
                
                if (extension == ".bytes" || 
                    extension == ".hash" || 
                    extension == ".info" ||
                    fileName.Contains("_") && (fileName.EndsWith(".bundle") || fileName.EndsWith(".dat")))
                {
                    string destPath = $"{targetDirectory}/{fileName}";
                    EditorTools.CopyFile(file, destPath, true);
                    fileCount++;
                }
            }

            Debug.Log($"补丁包导入完成！共拷贝 {fileCount} 个文件到: {targetDirectory}");
            AssetDatabase.Refresh();
        }
    }
}