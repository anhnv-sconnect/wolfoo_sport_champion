using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.Dialog
{
    public abstract class LoadingPanel : MonoBehaviour
    {
        public System.Action OnShown;
        public System.Action OnShowing;
        public System.Action OnHided;
        public System.Action OnHiding;

        public abstract void Show();
        public abstract void Hide();
    }
}
