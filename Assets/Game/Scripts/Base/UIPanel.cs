using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.Dialog
{
    public interface UIPanel
    {
        public void Show(System.Action OnShowing, System.Action OnComplete);
        public void Hide(System.Action OnHiding, System.Action OnComplete);
    }
}
