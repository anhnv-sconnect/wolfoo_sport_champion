using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] GameObject arrowHolder;
        [SerializeField] ParticleSystem lightingFx;
        private float flyTime;
        private float delaySpawnTime;
        private Vector3 initPos;
        private Transform initParent;
        private Quaternion initRotation;
        private bool isInit;
        private Sequence _tweenShoot;
        private bool isSpecial;
        private float specialTime;
        private Transform arrow1;
        private Transform arrow2;
        private float width;

        public bool IsAttached { get; set; }

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
            gameObject.SetActive(true);

            arrow1 = arrowHolder.transform.GetChild(0);
            arrow2 = arrowHolder.transform.GetChild(1);
            width = arrow1.GetComponent<SpriteRenderer>().sprite.rect.width;
        }
        internal void Setup(float flyTime, float delaySpawnTime)
        {
            this.flyTime = flyTime;
            this.delaySpawnTime = delaySpawnTime;
            transform.SetParent(initParent);
            transform.localPosition = initPos;
            transform.localRotation = initRotation;

            IsAttached = false;

            if (isSpecial)
            {
                arrow1.gameObject.SetActive(true);
                arrow2.gameObject.SetActive(true);

                arrow1.localPosition = new Vector3(-width / 200, arrow1.localPosition.y, 0);
                arrow2.localPosition = new Vector3(width / 200, arrow2.localPosition.y, 0);
            }
            else
            {
                arrow1.gameObject.SetActive(true);
                arrow2.gameObject.SetActive(false);

                arrow1.localPosition = new Vector3(0, arrow1.localPosition.y, 0);
            }
            arrowHolder.gameObject.SetActive(false);

            Invoke("DelaySpawn", this.delaySpawnTime);
        }
        private void DelaySpawn()
        {
            arrowHolder.gameObject.SetActive(true);
        }
        internal void SetupSpecial(float activeTime)
        {
            isSpecial = true;
            specialTime = activeTime;

            StopCoroutine("CountTimeAliveSpecial");
            StartCoroutine("CountTimeAliveSpecial");
        }
        private IEnumerator CountTimeAliveSpecial()
        {
            yield return new WaitForSeconds(specialTime);
            isSpecial = false;
        }

        internal void Release(Vector3 endPos, Transform parent)
        {
            CancelInvoke("DelaySpawn"); 
            arrowHolder.gameObject.SetActive(true);
            transform.SetParent(parent);

            lightingFx.Stop();

            bool isFlyToSky = endPos.y >= 3f;

            _tweenShoot?.Kill();
            _tweenShoot = DOTween.Sequence()
                .Append(transform.DOMove(endPos, flyTime).SetEase(Ease.Flash))
                .AppendCallback(() =>
                {
                    EventManager.OnShooting?.Invoke(this);
                })
                .AppendInterval(isFlyToSky ? 0 : 0.25f);
            _tweenShoot.OnComplete(() =>
            {
                arrowHolder.gameObject.SetActive(false);
                if (isFlyToSky && !IsAttached) lightingFx.Play();
            });

        }
    }
}
