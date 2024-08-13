using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport
{
    public abstract class LoadingPanel : MonoBehaviour
    {
        public System.Action OnShow;
        public System.Action OnHide;

        public abstract void Show();
        public abstract void Hide();
    }
}
