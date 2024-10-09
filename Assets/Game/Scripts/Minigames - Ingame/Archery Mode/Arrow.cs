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
        public Player AssignPlayer { get => myPlayer; }

        private Player myPlayer;

        private void OnDestroy()
        {
            _tweenShoot?.Kill();
            StopCoroutine("DelaySpawn");
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
        internal void Setup(Player player, float flyTime, float delaySpawnTime)
        {
            this.flyTime = flyTime;
            this.delaySpawnTime = delaySpawnTime;
            transform.SetParent(initParent);
            transform.localPosition = initPos;
            transform.localRotation = initRotation;

            myPlayer = player;
            IsAttached = false;
            SetupUI();

            StopCoroutine("DelaySpawn");
            StartCoroutine("DelaySpawn");
        }
        private IEnumerator DelaySpawn()
        {
            arrowHolder.gameObject.SetActive(false);
            yield return new WaitForSeconds(delaySpawnTime);
            arrowHolder.gameObject.SetActive(true);
        }
        private void SetupUI()
        {
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
        }
        internal void SetupNormal()
        {
            isSpecial = false;
        }
        internal void SetupSpecial(float activeTime)
        {
            isSpecial = true;
            specialTime = activeTime;
            SetupUI();
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
            StopCoroutine("DelaySpawn");
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
                    if(!IsAttached)
                    {
                        arrowHolder.gameObject.SetActive(false);
                        if(isFlyToSky)
                        {
                            lightingFx.Play();
                        }
                    }
                })
                .AppendInterval(0.25f);
            _tweenShoot.OnComplete(() =>
            {
                arrowHolder.gameObject.SetActive(false);
            });

        }
    }
}
