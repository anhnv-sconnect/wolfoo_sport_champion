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
        private void Awake()
        {
            var a = GameController.Instance;
        }
        // Start is called before the first frame update
        void Start()
        {

        }
        public void OnIntroCompleted()
        {
            EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene { home = true });
        }
    }
}
