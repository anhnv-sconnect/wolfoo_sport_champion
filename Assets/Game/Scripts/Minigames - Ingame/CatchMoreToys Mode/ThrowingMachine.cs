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
        private Tween _tween;
        private System.Action OnThrew;

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
                if (!isValid) boneFollower.SetBone(config.rightHandBoneNames[i]);
                else break;
            }

            transform.localScale -= Vector3.one * 0.2f;
        }
        private void OnDestroy()
        {
            _tween?.Kill();
        }
        public void StopThrow()
        {
            StopCoroutine("OnThrowing");
            StopCoroutine("StartThrow");
        }
        private void GetNextItem()
        {
            var collection = gameManager.GetAutoNextItem();
            itemPb = collection.prefab;
            hand.sprite = collection.sprite;
        }
        internal void StartThrow()
        {
            isAutoThrowing = true;
            GetNextItem();
            _tween = DOVirtual.DelayedCall(spawnTime, () =>
            {
                StartCoroutine("OnThrowing");
            });
        }
        internal void Throw(System.Action OnCompleted)
        {
            isAutoThrowing = false;
            OnThrew = OnCompleted;
            GetNextItem();
            StartCoroutine("OnThrowing");
        }
        private IEnumerator OnThrowing()
        {
            character.PlayThrowAnim(false);
            yield return new WaitForSeconds(character.GetTimeAnimation(CharacterWorldAnimation.AnimState.Throw) - 0.5f);

            OnThrew?.Invoke();

            var item = Instantiate(itemPb, itemHolder).GetComponent<Item>();
            item.transform.position = hand.transform.position;
            item.StartFlying();

            hand.sprite = null;

            if(isAutoThrowing)
            {
                yield return new WaitForSeconds(config.reloadTime);
                StartThrow();
            }
        }
    }
}
