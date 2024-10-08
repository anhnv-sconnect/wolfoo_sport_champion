using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterWorldAnimation;

namespace WFSport.Base
{
    [CreateAssetMenu(fileName = "Character_Asset Config", menuName = "Gameplay Data/Character_Asset", order = 1)]
    public class CharacterAsset : ScriptableObject
    {
        public Data[] records;

        [System.Serializable]
        public struct Data
        {
            public CharacterName Name;
            public Sprite icon;
            public CharacterWorldAnimation characterAnimWorld;
            public CharacterUIAnimation characterAnimUi;
        }
    }
}
