using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    public class Chair : DecorItem
    {
        // Start is called before the first frame update
        void Start()
        {

        }
        public override void Replace(Sprite icon)
        {
            PlayAnimReplace(icon);
        }
    }
}
