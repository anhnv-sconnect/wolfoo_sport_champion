using UnityEngine;
using DG.Tweening;
using Helper;
using Spine.Unity;
public class CharacterUIAnimation : MonoBehaviour
{
    [Header("<======== SPINE =======>")]
    [SerializeField] private SkeletonGraphic skeletonAnim;
    [Header("Idle")]
    [SerializeField, SpineAnimation] private string idleAnim;
    [Header("Run")]
    [SerializeField, SpineAnimation] private string runAnim;
    [Header("Jump")]
    [SerializeField, SpineAnimation] private string jumpAnim;
    [Header("Happy")]
    [SerializeField, SpineAnimation] private string happyAnim;
    [Header("Sad")]
    [SerializeField, SpineAnimation] private string sadAnim;
    [Header("Special")]
    [SerializeField, SpineAnimation] private string specialAnim;
    [Header("WaveHand")]
    [SerializeField, SpineAnimation] private string wavehandAnim;
    private AnimState animState;
    private Tween _tween;

    public SkeletonGraphic SkeletonAnim { get => skeletonAnim; set => skeletonAnim = value; }

    private void Awake()
    {
    }
    private void OnDestroy()
    {
        _tween?.Kill();
    }

    public void PlayJumpAnim()
    {
        PlayJump();
        if (_tween.IsActive()) return;
        _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }
    public void PlayWaveHandAnim()
    {
        PlayWaveHand();
        if (_tween.IsActive()) return;
        _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }
    public void PlaySadAnim()
    {
        PlaySad();
        if (_tween.IsActive()) return;
        _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }
    public void PlayHappyAnim()
    {
        PlaySpecial();
        if (_tween.IsActive()) return;
        _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }
    #region Anim by Spine
    private void PlayMove()
    {
        if (animState == AnimState.Run) return;
        animState = AnimState.Run;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, runAnim, true);
    }
    private void PlayIdle()
    {
        if (animState == AnimState.Idle)
            return;
        animState = AnimState.Idle;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, idleAnim, true);
    }
    private void PlayJump()
    {
        if (animState == AnimState.Jump)
            return;
        animState = AnimState.Jump;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, jumpAnim, false);
    }
    private void PlayHappy()
    {
        if (animState == AnimState.Happy)
            return;
        animState = AnimState.Happy;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, happyAnim, false);
    }
    private void PlaySad()
    {
        if (animState == AnimState.Sad)
            return;
        animState = AnimState.Sad;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, sadAnim, false);
    }
    private void PlaySpecial()
    {
        if (animState == AnimState.Special)
            return;
        animState = AnimState.Special;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, specialAnim, false);
    }
    private void PlayWaveHand()
    {
        if (animState == AnimState.WaveHand) return;
        animState = AnimState.WaveHand;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, wavehandAnim, false);
    }
    public float GetTimeAnimation(AnimState animState)
    {
        var myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(happyAnim);
        switch (animState)
        {
            case AnimState.None:
                break;
            case AnimState.Idle:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(idleAnim);
                break;
            case AnimState.Run:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(runAnim);
                break;
            case AnimState.Jump:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(jumpAnim);
                break;
            case AnimState.Happy:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(happyAnim);
                break;
            case AnimState.Sad:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(sadAnim);
                break;
            case AnimState.Special:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(specialAnim);
                break;
            case AnimState.WaveHand:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(wavehandAnim);
                break;
        }

        float animLength = myAnimation.Duration;
        return animLength;
    }

    public enum AnimState
    {
        None,
        Idle,
        Run,
        Jump,
        Happy,
        Sad,
        Special,
        WaveHand,
    }
    #endregion
   
}
