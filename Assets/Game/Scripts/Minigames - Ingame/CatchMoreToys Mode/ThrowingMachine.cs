using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class ThrowingMachine : MonoBehaviour
    {
        [NaughtyAttributes.MinValue(0)] 
        [SerializeField] float spawnTime;
        [SerializeField] SpriteRenderer hand;

        private bool isAutoThrowing;
        private Transform itemHolder;
        private GameObject itemPb;

        private GameplayConfig config;
        private GameplayManager gameManager;
        private CharacterWorldAnimation character;

        private System.Action OnThrew;
        private bool isTempuraryValue;
        private Sequence _sequence;

        internal void ResetDefault()
        {
            hand.sprite = null;
            isTempuraryValue = false;
        }
        internal void Setup(Transform itemHolder, 
            GameplayManager gameplay, 
            GameplayConfig config, 
            CharacterWorldAnimation character)
        {
            this.itemHolder = itemHolder;
            gameManager = gameplay;
            this.config = config;
            this.character = character;

            var boneFollower = hand.gameObject.AddComponent<BoneFollower>();
            boneFollower.SkeletonRenderer = character.SkeletonAnim;
            var isValid = false;
            for (int i = 0; i < config.rightHandBoneNames.Length; i++)
            {
                if (!isValid)
                {
                    isValid = boneFollower.SetBone(config.rightHandBoneNames[i]);
                }
                else break;
            }

            transform.localScale -= Vector3.one * 0.2f;
        }
        internal void SetupTutorial()
        {
            isTempuraryValue = true;
        }
        private void OnDestroy()
        {
            _sequence?.Kill();
        }
        public void StopThrow()
        {
            _sequence?.Pause();
        }
        public void ResumeThrow()
        {
            _sequence?.Play();
        }
        private void GetNextItem()
        {
            var collection = gameManager.GetAutoNextItem();
            itemPb = collection.prefab;
            hand.sprite = collection.sprite;
        }
        internal void StartAutoThrow()
        {
            isAutoThrowing = true;
            GetNextItem();
            _sequence = DOTween.Sequence()
                .AppendInterval(spawnTime)
                .AppendCallback(() =>
                {
                    OnThrowing();
                });
        }
        internal void Throw(System.Action OnCompleted)
        {
            isAutoThrowing = false;
            OnThrew = OnCompleted;
            GetNextItem();

            _sequence?.Kill();
            OnThrowing();
        }
        private void OnThrowing()
        {
            _sequence = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    character.PlayIdleAnim();
                    character.PlayThrowAnim(false);
                })
                .AppendInterval(character.GetTimeAnimation(CharacterWorldAnimation.AnimState.Throw) - 0.5f)
                .AppendCallback(() =>
                {
                    var item = Instantiate(itemPb, itemHolder).GetComponent<Item>();
                    item.transform.position = hand.transform.position;
                    item.Assign(config, isTempuraryValue);
                    item.StartFlying();

                    hand.sprite = null;
                })
                .AppendInterval(config.reloadTime)
                .AppendCallback(() =>
                {
                    OnThrew?.Invoke();
                    if (isAutoThrowing) { StartAutoThrow(); }
                });
        }
    }
}
