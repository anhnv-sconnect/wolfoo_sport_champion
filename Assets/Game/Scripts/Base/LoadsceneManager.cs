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
        public Action OnLoadClosing;

        public bool IsLoadCompleted;

        void OnChangeScene(System.Action OnComplete)
        {
            IsLoadCompleted = false;
            var lastSceneHandleName = SceneManager.GetActiveScene();
            SceneManager.LoadSceneAsync(sceneHandleName, LoadSceneMode.Single).completed += (data) =>
            {
                if (lastSceneHandleName.IsValid())
                {
                    OnLoadSuccess?.Invoke();

                    GC.Collect();
                    Resources.UnloadUnusedAssets();

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
                loadingPanel.OnHiding = () =>
                {
                    Debug.Log("OnLoad Success");
                    IsLoadCompleted = true;
                    OnLoadComplete?.Invoke();
                };
                loadingPanel.OnHided = () =>
                {
                    OnLoadClosing?.Invoke();
                };
            });
        }
        public void OnLoadScene(string name, System.Action OnComplete)
        {
            sceneHandleName = name;
            OnChangeScene(OnComplete);
        }

    }
}
