using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public class Arrow : MonoBehaviour
    {
        private float flyTime;
        private float delaySpawnTime;
        private Vector3 initPos;
        private Transform initParent;
        private Quaternion initRotation;
        private bool isInit;
        private Sequence _tweenShoot;

        private void OnDestroy()
        {
            _tweenShoot?.Kill();
            CancelInvoke();
        }
        internal void Init()
        {
            if (isInit) return;
            isInit = true;

            initPos = transform.localPosition;
            initParent = transform.parent;
            initRotation = transform.localRotation;
        }
        internal void Setup(float flyTime, float delaySpawnTime)
        {
            this.flyTime = flyTime;
            this.delaySpawnTime = delaySpawnTime;
            transform.SetParent(initParent);
            transform.localPosition = initPos;
            transform.localRotation = initRotation;

            gameObject.SetActive(false);
            Invoke("DelaySpawn", this.delaySpawnTime);
        }
        private void DelaySpawn()
        {
            gameObject.SetActive(true);
        }

        internal void Release(Vector3 endPos, Transform parent)
        {
            CancelInvoke("DelaySpawn"); 
            gameObject.SetActive(true);
            transform.SetParent(parent);

            _tweenShoot?.Kill();
            _tweenShoot = DOTween.Sequence()
                .Append(transform.DOMove(endPos, flyTime).SetEase(Ease.Flash))
                .AppendCallback(() =>
                {
                    EventManager.OnShooting?.Invoke(this);
                })
                .AppendInterval(0.25f);
            _tweenShoot.OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        }
    }
}
