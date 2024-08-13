using UnityEngine;
using DG.Tweening;
using Helper;
using Spine.Unity;
public class CharacterWorldAnimation : MonoBehaviour
{
    [Header("<======== SPINE =======>")]
    public SkeletonAnimation SkeletonAnim;
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
    [Header("Dizzy")]
    [SerializeField, SpineAnimation] private string dizzyAnim;
    [Header("Slow")]
    [SerializeField, SpineAnimation] private string slowAnim;
    [Header("RunFast")]
    [SerializeField, SpineAnimation] private string runFastAnim;
    [Header("Push")]
    [SerializeField, SpineAnimation] private string pushAnim;

    private AnimState animState;
    private Tween _tween;

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
        if (_tween.IsActive() && _tween.IsPlaying()) return;
        _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }
    public void PlayWaveHandAnim()
    {
        PlayWaveHand();
        if (_tween.IsActive() && _tween.IsPlaying()) return;
        _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }
    public void PlaySadAnim()
    {
        PlaySad();
        if (_tween.IsActive() && _tween.IsPlaying()) return;
        _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }
    public void PlayHappyAnim()
    {
        PlaySpecial();
        if (_tween.IsActive() && _tween.IsPlaying()) return;
        _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }
    public void PlayRunAnim()
    {
        _tween?.Kill();
        PlayMove();
    }
    public void PlaySlowAnim()
    {
        _tween?.Kill();
        PlaySlow(true);
    }
    public void PlayRunFastAnim()
    {
        _tween?.Kill();
        PlayRunFast(true);
    }
    public void PlayIdleAnim()
    {
        _tween?.Kill();
        PlayIdle();
    }
    public void PlayDizzyAnim(bool isLoop = true)
    {
        _tween?.Kill();
        PlayDizzy(isLoop);
    }
    public void PlayPushAnim(bool isLoop = true)
    {
        _tween?.Kill();
        PlayPush(isLoop);
    }
    #region Anim by Spine
    private void PlayMove()
    {
        if (animState == AnimState.Run) return;
        animState = AnimState.Run;
        if (runAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, runAnim, true);
    }
    private void PlayIdle()
    {
        if (animState == AnimState.Idle)
            return;
        animState = AnimState.Idle;
        if (idleAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, idleAnim, true);
    }
    private void PlayJump()
    {
        if (animState == AnimState.Jump)
            return;
        animState = AnimState.Jump;
        if (jumpAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, jumpAnim, false);
    }
    private void PlayHappy()
    {
        if (animState == AnimState.Happy)
            return;
        animState = AnimState.Happy;
        if (happyAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, happyAnim, false);
    }
    private void PlaySad()
    {
        if (animState == AnimState.Sad)
            return;
        animState = AnimState.Sad;
        if (sadAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, sadAnim, false);
    }
    private void PlaySpecial()
    {
        if (animState == AnimState.Special)
            return;
        animState = AnimState.Special;
        if (specialAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, specialAnim, false);
    }
    private void PlayWaveHand()
    {
        if (animState == AnimState.WaveHand) return;
        animState = AnimState.WaveHand;
        if (wavehandAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, wavehandAnim, false);
    }
    private void PlayDizzy(bool isLoop)
    {
        if (animState == AnimState.Dizzy) return;
        animState = AnimState.Dizzy;
        if (dizzyAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, dizzyAnim, isLoop);
    }
    private void PlaySlow(bool isLoop)
    {
        if (animState == AnimState.Slow) return;
        animState = AnimState.Slow;
        if (slowAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, slowAnim, isLoop);
    }
    private void PlayRunFast(bool isLoop)
    {
        if (animState == AnimState.RunFast) return;
        animState = AnimState.RunFast;
        if (runFastAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, runFastAnim, isLoop);
    }
    private void PlayPush(bool isLoop)
    {
        if (animState == AnimState.Push) return;
        animState = AnimState.Push;
        if (runFastAnim == null) return;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, pushAnim, isLoop);
    }
    public float GetTimeAnimation(AnimState animState)
    {
        Spine.Animation myAnimation = null;
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
            case AnimState.Dizzy:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(dizzyAnim);
                break;
            case AnimState.Slow:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(slowAnim);
                break;
            case AnimState.RunFast:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(runFastAnim);
                break;
            case AnimState.Push:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(pushAnim);
                break;
        }

        if (myAnimation == null) return 0;

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
        Dizzy,
        Slow,
        RunFast,
        Push,
    }
    #endregion
   
}
