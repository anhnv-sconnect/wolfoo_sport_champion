using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WFSport.Gameplay.RelayMode
{
    [CustomEditor(typeof(TrafficCone))]
    public class ConeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var myScript = (TrafficCone)target;
            myScript.OnCreating();
        }
    }
}
