using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    public class Toy : DecorItem
    {
        public override void Replace(Sprite icon)
        {
            PlayAnimReplace(icon);
            
        }
    }
}
