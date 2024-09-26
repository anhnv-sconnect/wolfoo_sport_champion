using AnhNV.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WFSport.Base
{
    public class LoadSceneManager : MonoBehaviour
    {
        [SerializeField] LoadingLazy loadingPanel;

        private string sceneHandleName;
        public Action OnLoadComplete;
        public Action OnLoadSuccess;

        void OnChangeScene(System.Action OnComplete)
        {
            var lastSceneHandleName = SceneManager.GetActiveScene();
            SceneManager.LoadSceneAsync(sceneHandleName, LoadSceneMode.Single).completed += (data) =>
            {
                OnLoadSuccess?.Invoke();
                if (lastSceneHandleName.IsValid())
                {
                    SceneManager.UnloadSceneAsync(lastSceneHandleName).completed += (handle) =>
                    {
                        OnComplete?.Invoke();
                    };
                }
                else
                {
                    OnComplete?.Invoke();
                }
            };
        }
        public void LoadScene(string name, bool isUsingLoading = true)
        {
            if(isUsingLoading)
            {
                loadingPanel.Show();
            }

            OnLoadScene(name, () =>
            {
                loadingPanel.Hide();
                loadingPanel.OnHide = () =>
                {
                    OnLoadComplete?.Invoke();
                };
            });
        }
        public void OnLoadScene(string name, System.Action OnComplete)
        {
            sceneHandleName = name;

            OnChangeScene(() =>
            {
                GC.Collect();
                Resources.UnloadUnusedAssets();

                OnComplete?.Invoke();
            });
        }

    }
}
