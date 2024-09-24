using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SCN.MagnetCoin 
{
    public class MagnetCoinEditor : Editor
    {
        [MenuItem("GameObject/SCN/Effect/Magnet coin")]
        static void CreateHorizontalScrollInfinity()
        {
            string prefabPath = "Assets/SCNLib/Magnet coin/";
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath
                    + "Magnet coin.prefab"));
                obj.name = "Magnet coin";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath
                    + "Magnet coin.prefab"), Selection.activeGameObject.transform);
                obj.name = "Magnet coin";
                Selection.activeGameObject = obj;
            }

            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }
}