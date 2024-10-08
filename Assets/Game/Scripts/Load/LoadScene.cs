using AnhNV.Dialog;
using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;

namespace WFSport.Load
{
    public class LoadScene : MonoBehaviour
    {
        [SerializeField] LoadingLazy loadingPanel;

        private void Awake()
        {
            var a = GameController.Instance;
        }
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("LoadScene 1 ");
            loadingPanel.Setup(3);
            loadingPanel.Show();
            loadingPanel.OnShown = () =>
            {
                EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene { home = true });
                loadingPanel.Hide();
                loadingPanel.Setup(2);
            };
        }
    }
}
