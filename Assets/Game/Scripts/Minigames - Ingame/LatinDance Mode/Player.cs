using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.LatinDanceMode
{
    public class Player : Base.Player
    {
        [SerializeField] CharacterWorldAnimation wolfooPb;
        [SerializeField] CharacterWorldAnimation partnerPb;
        [SerializeField] Transform[] circles;
        [SerializeField] BoxCollider2D limited;
        [SerializeField] float time;
        [SerializeField] SortingGroup sorting;
        private IMinigame.GameState gameState;
        private Vector3 inititalPos;
        private Vector2 inittialLimited;
        private Camera cam;
        private Vector3 initialCamPos;
        private bool isCalculating = false;
        private Sequence _introduceSequence;
        private bool canDrag;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }

        CharacterWorldAnimation[] characters;
        CharacterWorldAnimation wolfoo;
        private Tween _tweenSkate;

        #region UNITY METHODS

        private void OnDestroy()
        {
            _introduceSequence?.Kill();
            _tweenSkate?.Kill();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (GameplayState != IMinigame.GameState.Playing) return;
            if (collision.CompareTag(TAG.BONUSITEM))
            {
                EventManager.OnPlayerClaimNewStar?.Invoke(this);
            }
        }
        #endregion

        #region METHODS 
        internal void PlayWining()
        {
            Pause(false);
            foreach (var item in characters)
            {
                item.PlayJumpWinAnim();
            }
        }
        internal void IntroduceWFPartner()
        {
            canDrag = false;

            var partner = CreatePartner();
            partner.PlaySkateAnim(CharacterWorldAnimation.AnimState.Skate1);
            partner.gameObject.SetActive(false);
            wolfoo.PlaySkateAnim(CharacterWorldAnimation.AnimState.Skate1);
            wolfoo.transform.rotation = Quaternion.Euler(Vector3.up * 0);

            _introduceSequence = DOTween.Sequence()
                .Append(transform.DOMove(Vector3.zero, 1))
                .AppendCallback(() =>
                {
                    partner.transform.rotation = Quaternion.Euler(Vector3.up * 180);
                    partner.gameObject.SetActive(true);
                    EventManager.OnHighlight?.Invoke(wolfoo.transform, false);
                    EventManager.OnHighlight?.Invoke(partner.transform, false);
                    circles[0].gameObject.SetActive(true);
                    circles[1].gameObject.SetActive(true);
                })
                .Append(partner.transform.DOMoveY(0, 1).SetEase(Ease.Linear))
                .Join(wolfoo.transform.DOLocalMove(new Vector3(2, 0, 0), 0.5f))
                .AppendCallback(() =>
                {
                    partner.PlaySkateAnim(CharacterWorldAnimation.AnimState.Skate2);
                    wolfoo.PlaySkateAnim(CharacterWorldAnimation.AnimState.Skate2);
                })
                .Append(wolfoo.transform.DOMoveX(4.6f, 1).SetEase(Ease.Linear).OnStart(() =>
                {
                    wolfoo.transform.rotation = Quaternion.Euler(Vector3.up * 180);
                }))
                .Join(partner.transform.DOMoveX(-4.6f, 1).SetEase(Ease.Linear).OnStart(() =>
                {
                    partner.transform.rotation = Quaternion.Euler(Vector3.up * 0);
                }))
                .AppendCallback(() =>
                {
                    wolfoo.PlayJumpWinAnim(false);
                    partner.PlayJumpWinAnim(false);
                })
                .AppendInterval(2)
                .Append(wolfoo.transform.DOMoveX(1.25f, 1).SetEase(Ease.Linear).OnStart(() =>
                {
                    wolfoo.transform.rotation = Quaternion.Euler(Vector3.up * 0);
                }))
                .Join(partner.transform.DOMoveX(-1.25f, 1).SetEase(Ease.Linear).OnStart(() =>
                {
                    partner.transform.rotation = Quaternion.Euler(Vector3.up * 180);
                }));

            _introduceSequence.OnComplete(() =>
            {
                EventManager.OnStopHighlight?.Invoke(wolfoo.transform);
                EventManager.OnStopHighlight?.Invoke(partner.transform);
                circles[0].gameObject.SetActive(false);
                circles[1].gameObject.SetActive(false);

                _tweenSkate?.Kill();
                foreach (var item in characters) { item.PlayIdleAnim(); }
                canDrag = true;
            });
        }
        internal void IntroduceWolfoo(System.Action OnCompleted)
        {

            CreateWolfoo();
            wolfoo.transform.position = Vector3.up * 10;

            _introduceSequence = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    EventManager.OnHighlight?.Invoke(wolfoo.transform, true);
                    circles[0].gameObject.SetActive(true);
                    wolfoo.PlayRandomSkateAnim();
                })
                .Append(wolfoo.transform.DOMoveY(-2, 1).SetEase(Ease.Linear))
                .AppendCallback(() => wolfoo.PlayJumpWinAnim(false))
                .AppendInterval(1)
                .Append(wolfoo.transform.DOMoveY(0, 1).SetEase(Ease.Linear).OnStart(() =>
                {
                    wolfoo.PlayRandomSkateAnim();
                }));

            _introduceSequence.OnComplete(() =>
            {
                EventManager.OnStopHighlight?.Invoke(wolfoo.transform);

                _tweenSkate?.Kill();
                wolfoo.PlayIdleAnim();
                circles[0].gameObject.SetActive(false);

                OnCompleted?.Invoke();
            });
        }
        private CharacterWorldAnimation CreatePartner()
        {
            var partner = Instantiate(partnerPb, transform);
            partner.gameObject.layer = LAYER.CHARACTER;
            partner.ChangeSkin(CharacterWorldAnimation.SkinType.Prince);
            partner.transform.localPosition = new Vector3(-2, 10, 0);
            characters[1] = partner;
            circles[1].SetParent(partner.transform);
            circles[1].localPosition = Vector3.zero;


            return partner;
        }
        private CharacterWorldAnimation CreateWolfoo()
        {
            wolfoo = Instantiate(wolfooPb, transform);
            wolfoo.ChangeSkin(CharacterWorldAnimation.SkinType.Prince);
            wolfoo.gameObject.layer = LAYER.CHARACTER;
            wolfoo.transform.localPosition = Vector3.zero;
            characters[0] = wolfoo;
            circles[0].SetParent(wolfoo.transform);
            circles[0].localPosition = Vector3.zero;

            return wolfoo;
        }
        internal void Setup(int layer)
        {
            sorting.sortingOrder = layer;
        }
        #endregion

        #region OVERRIDE METHODS

        public override void Init()
        {
            inititalPos = transform.position;
            inittialLimited = limited.size / 2;
            cam = Camera.main;
            initialCamPos = cam.transform.position;

            foreach (var item in circles) { item.gameObject.SetActive(false); }
            characters = new CharacterWorldAnimation[2];
        }

        public override void Lose()
        {
        }
        public bool IsPointInCapsule(Vector2 point, Vector2 capsuleStart, Vector2 capsuleEnd, float radius)
        {
            // Vector from start to end of the capsule (capsule's axis)
            Vector2 capsuleAxis = capsuleEnd - capsuleStart;

            // Length of the capsule axis
            float capsuleLength = capsuleAxis.magnitude;

            // Normalize the capsule axis to unit length
            Vector2 capsuleAxisNormalized = capsuleAxis / capsuleLength;

            // Project the point onto the capsule axis
            Vector2 pointProjection = Vector2.Dot(point - capsuleStart, capsuleAxisNormalized) * capsuleAxisNormalized + capsuleStart;

            // Distance from the start to the projected point
            float projectionLength = Vector2.Dot(pointProjection - capsuleStart, capsuleAxisNormalized);

            // Check if the point projection lies within the central rectangle part of the capsule
            if (projectionLength >= 0 && projectionLength <= capsuleLength)
            {
                // Check if the perpendicular distance to the capsule axis is within the capsule's radius
                float distanceToAxis = Vector2.Distance(point, pointProjection);
                if (distanceToAxis <= radius)
                {
                    return true; // Point is within the central rectangular part
                }
            }

            // If not in the rectangle, check if it is in either semicircle
            if (Vector2.Distance(point, capsuleStart) <= radius || Vector2.Distance(point, capsuleEnd) <= radius)
            {
                return true; // Point is within one of the semicircles
            }

            return false; // Point is outside the capsule
        }

        public override void OnDragging(Vector3 force)
        {
            if (!canDrag) return;
            if (isCalculating) return;

            isCalculating = true;
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (pos.x > inittialLimited.x) pos.x = inittialLimited.x;
            if (pos.y > inittialLimited.y) pos.y = inittialLimited.y;
            if (pos.x < -inittialLimited.x) pos.x = -inittialLimited.x;
            if (pos.y < -inittialLimited.y) pos.y = -inittialLimited.y;
            transform.position = pos;

            if (force.x < 0)
            {
                foreach (var character in characters)
                {
                    if (character == null) continue;
                    character.transform.rotation = Quaternion.Euler(Vector3.zero);
                }
            }
            else {

                foreach (var character in characters)
                {
                    if (character == null) continue;
                    character.transform.rotation = Quaternion.Euler(Vector3.up * 180);
                }
            } 

            isCalculating = false;
        }
        private void PlaySkate()
        {
            var skateAnim = characters[0].GetRandomSkateAnim();
            foreach (var character in characters)
            {
                if (character == null) continue;
                character.PlayRandomSkateAnim();
            }
            _tweenSkate?.Kill();
            _tweenSkate = DOVirtual.DelayedCall(wolfoo.GetTimeAnimation(skateAnim), () =>
            {
                PlaySkate();
            });
        }
        protected override void OnBeginDrag()
        {
            base.OnBeginDrag();
            if (!canDrag) return;
            PlaySkate();
        }
        protected override void OnEndDrag()
        {
            base.OnEndDrag();
            if (!canDrag) return;
            _tweenSkate?.Kill();
            foreach (var character in characters)
            {
                if (character == null) continue;
                character.PlayIdleAnim();
            }
        }

        public override void OnSwipe()
        {
        }

        public override void OnTouching(Vector3 position)
        {
        }

        private void CamFollowing()
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position,
                new Vector3(transform.position.x, transform.position.y + 2, initialCamPos.z),
                time);
        }

        public override void OnUpdate()
        {
            if (GameplayState != IMinigame.GameState.Playing) return;
            CamFollowing();
        }
        public override void Pause(bool isSystem)
        {
            if (isSystem) GameplayState = IMinigame.GameState.Pausing;
            else
            {
                canDrag = false;
                _tweenSkate?.Kill();
                foreach (var item in characters)
                {
                    item.PlayIdleAnim();
                }
            }
        }

        public override void Play()
        {
            canDrag = true;
            GameplayState = IMinigame.GameState.Playing;
        }

        public override void ResetDefault()
        {
        }
        #endregion
    }
}
