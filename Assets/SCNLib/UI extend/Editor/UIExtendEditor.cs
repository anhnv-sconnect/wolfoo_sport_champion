using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCN.Common;

namespace SCN.UIExtend
{
    public class UIExtendEditor : Editor
    {
        [MenuItem("GameObject/SCN/Scroll Infinity/Horizontal")]
        static void CreateHorizontalScrollInfinity()
        {
            string prefabPath = "Assets/SCNLib/UI extend/Scroll infinity/";
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath 
                    + "Horizontal scroll infinity.prefab"));
                obj.name = "Horizontal scroll infinity";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath 
                    + "Horizontal scroll infinity.prefab"), Selection.activeGameObject.transform);
                obj.name = "Horizontal scroll infinity";
                Selection.activeGameObject = obj;
            }

            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        [MenuItem("GameObject/SCN/Scroll Infinity/Vertical")]
        static void CreateVerticalScrollInfinity()
        {
            string prefabPath = "Assets/SCNLib/UI extend/Scroll infinity/";
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath
                    + "Vertical scroll infinity.prefab"));
                obj.name = "Vertical scroll infinity";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath
                    + "Vertical scroll infinity.prefab"), Selection.activeGameObject.transform);
                obj.name = "Vertical scroll infinity";
                Selection.activeGameObject = obj;
            }

            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        [MenuItem("GameObject/SCN/UI/Button")]
        static void CreateCustomBtn()
        {
            string prefabPath = "Assets/SCNLib/UI extend/UI/";
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath
                    + "Button.prefab"));
                obj.name = "Button";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath
                    + "Button.prefab"), Selection.activeGameObject.transform);
                obj.name = "Button";
                Selection.activeGameObject = obj;
            }
            
            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }
}