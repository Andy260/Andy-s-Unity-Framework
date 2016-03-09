using System.IO;
using UnityEditor;
using UnityEngine;

namespace AndysUnityFramework
{
    public class ThumbnailCreator : EditorWindow
    {
        /// <summary>
        /// Creates a thumbnail asset from a prefab
        /// </summary>
        [MenuItem("Tri-Hard Games Tools/Asset Creation/Save Prefab Thumbnail")]
        static void CreateThumbnailFromPrefab()
        {
            // Get prefab path from user
            string prefabPath = EditorUtility.OpenFilePanel("Select Prefab", Application.dataPath, "prefab");

            // Ensure path is valid
            if (!File.Exists(prefabPath))
            {
                Debug.LogError("Invalid prefab path");
                return;
            }

            prefabPath = RemoveAssetsPathFromPath(prefabPath);

            // Load prefab from path
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError("Unable to load asset");
                return;
            }

            // Get save file location
            string savePath = EditorUtility.SaveFilePanel("Save Prefab Thumbnail Location", Application.dataPath, 
                prefab.name + " Thumbnail", "png");

            // Save thumbnail file
            CreateThumbnail(prefab, savePath);
        }

        [MenuItem("Assets/Tri-Hard Games Tools/Save Thumbnail")]
        static void CreateThumbnailFromSelected()
        {
            GameObject selectedObject = (GameObject)Selection.activeObject;

            if (selectedObject == null)
            {
                Debug.LogError("Unable to save thumbnail on NULL GameObject");
                return;
            }

            // Get save file location
            string savePath = EditorUtility.SaveFilePanel("Save Prefab Thumbnail Location", Application.dataPath,
                selectedObject.name + " Thumbnail", "png");

            // Save thumbnail file
            CreateThumbnail(selectedObject, savePath);
        }

        [MenuItem("Assets/Tri-Hard Games Tools/Save Thumbnail", true)]
        static bool IsSelectionGameObject()
        {
            return Selection.activeObject.GetType() == typeof(GameObject);
        }

        /// <summary>
        /// Creates a thumbnail asset from a given GameObject
        /// </summary>
        /// <param name="a_gameObject">GameObject which to save a thumbnail asset of</param>
        /// <param name="a_savePath">Full path to save thumbnail asset to</param>
        static void CreateThumbnail(GameObject a_gameObject, string a_savePath)
        {
            // Save thumbnail file
            Texture2D thumbnail = AssetPreview.GetAssetPreview(a_gameObject);
            if (thumbnail != null)
            {
                File.WriteAllBytes(a_savePath, thumbnail.EncodeToPNG());

                // Refresh asset database so user sees new thumbnail asset within editor
                string savePath = RemoveAssetsPathFromPath(a_savePath);
                AssetDatabase.ImportAsset(savePath, ImportAssetOptions.Default);
            }
        }

        /// <summary>
        /// Removes the asset path from a given path, 
        /// for Unity functions which require paths relative to the assets folder
        /// </summary>
        /// <param name="a_path">Path which to remove asset path from</param>
        /// <returns></returns>
        static string RemoveAssetsPathFromPath(string a_path)
        {
            int index = a_path.IndexOf(Application.dataPath);
            a_path = (index < 0) ? a_path
                : a_path.Remove(index, Application.dataPath.Length);
            a_path = "Assets" + a_path;
            a_path.Replace("/", "");

            return a_path;
        }
    }
}
