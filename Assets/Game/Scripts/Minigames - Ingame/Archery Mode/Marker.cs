using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public abstract class Marker : MonoBehaviour
    {
        [SerializeField] protected GameObject markedHole;
        [SerializeField] protected CircleCollider2D myCollider;
        [SerializeField] protected SpriteRenderer myRenderer;

        internal abstract void Hide();
        internal abstract void Show();
        internal abstract void Init();
        internal abstract bool IsInside(Vector3 position);

        public bool IsShowing { get; protected set; }
    }
}
