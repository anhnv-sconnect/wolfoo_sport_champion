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
        // Start is called before the first frame update
        void Start()
        {
            loadingPanel.Setup(2);
            loadingPanel.Show();
            loadingPanel.OnShow = () =>
            {
                EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene { home = true, notUsingLoading = true });
                loadingPanel.Hide();
            };
        }
    }
}
