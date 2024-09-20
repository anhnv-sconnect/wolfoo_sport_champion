using AnhNV.Helper;
using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WFSport.Base;

namespace WFSport.Gameplay
{
    public class MinigameSystemUI : MonoBehaviour
    {
        [SerializeField] private Button backBtn;
        private bool canClick = true;

        // Start is called before the first frame update
        void Start()
        {
            backBtn.onClick.AddListener(OnClickBackBtn);
        }

        private void OnClickBackBtn()
        {
            if (!canClick) return;
            canClick = false;
            Holder.PlaySound?.Invoke();
            Holder.OpenDialog?.Invoke("PauseDialog");
            Debug.Log("Click Back");
            EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene { home = true });
        }
    }
}
