using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AnhNV.GameBase
{
    public class PopupManager : MonoBehaviour
    {
        public enum DialogName
        {
            Pause
        }
        [SerializeField] string path;
        [SerializeField] GameObject popup;

        [NaughtyAttributes.Button]
        private void GetAllPopup()
        {
            string[] files = Directory.GetFiles(path, "*.prefab", SearchOption.TopDirectoryOnly);

            //sourcePanels = new UIPanel[files.Length];
            //for (int i = 0; i < files.Length; i++)
            //{
            //    var fileName = files[i];
            //    var uiPanel = UnityEditor.AssetDatabase.LoadAssetAtPath<UIPanel>(fileName);
            //    sourcePanels[i] = uiPanel;
            //}
        }

        private void Start()
        {
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OpenDialog>(OpenDialog);   
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OnClosingDialog>(OnClosing);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OpenDialog>(OpenDialog);
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OnClosingDialog>(OnClosing);
        }

        private void OnClosing(EventKeyBase.OnClosingDialog obj)
        {
            popup.SetActive(false);
        }

        private void OpenDialog(EventKeyBase.OpenDialog obj)
        {
            popup.SetActive(true);
        }
    }
}
