using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    [System.Serializable]
    public class CreatedToyData
    {
        public Vector3 Position;
        public int Id;
        public Topic.Kind TopicKind;

        public CreatedToyData()
        {
        }

        public CreatedToyData(Vector3 position, int id, Topic.Kind topicKind)
        {
            Position = position;
            Id = id;
            TopicKind = topicKind;
        }
    }
}
