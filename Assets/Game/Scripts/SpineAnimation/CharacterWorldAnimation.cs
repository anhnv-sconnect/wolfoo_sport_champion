using UnityEngine;
using DG.Tweening;
using Helper;
using Spine.Unity;
public class CharacterWorldAnimation : MonoBehaviour
{
    [Header("<======== SKIN =======>")]
    [SerializeField, SpineSkin] string[] skinList;

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
    [Header("Throw")]
    [SerializeField, SpineAnimation] private string throwAnim;
    [Header("Skate1")]
    [SerializeField, SpineAnimation] private string skating1Anim;
    [Header("Skate2")]
    [SerializeField, SpineAnimation] private string skating2Anim;
    [Header("Skate3")]
    [SerializeField, SpineAnimation] private string skating3Anim;
    [Header("Skate4")]
    [SerializeField, SpineAnimation] private string skating4Anim;
    [Header("Skate5")]
    [SerializeField, SpineAnimation] private string skating5Anim;
    [Header("JumpWin")]
    [SerializeField, SpineAnimation] private string jumpWinAnim;
    [Header("Throwback")]
    [SerializeField, SpineAnimation] private string throwBackAnim;
    [Header("BackIdle")]
    [SerializeField, SpineAnimation] private string backIdleAnim;

    private AnimState animState;
    private Tween _tween;

    public enum SkinType
    {
        Normal,
        Prince, 
        Sport,
        Christmas
    }
    public void ChangeSkin(SkinType colorType)
    {
        SkeletonAnim.Skeleton.SetSkin(skinList[(int)colorType]);
        SkeletonAnim.Skeleton.SetSlotsToSetupPose();
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
        if (!isLoop)
        {
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
        }
    }
    public void PlayPushAnim(bool isLoop = true)
    {
        _tween?.Kill();
        PlayPush(isLoop);
        if (!isLoop)
        {
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
        }
    }
    public void PlayThrowAnim(bool isLoop = true)
    {
        _tween?.Kill();
        PlayThrow(isLoop);
        if (!isLoop)
        {
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
        }
    }
    public void PlayJumpWinAnim(bool isLoop = true)
    {
        _tween?.Kill();
        PlayJumpWin(isLoop);
        if (!isLoop)
        {
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
        }
    }
    public void PlayThrowbackAnim(bool isLoop = true)
    {
        _tween?.Kill();
        PlayThrowback(isLoop);
        if (!isLoop)
        {
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState) / SkeletonAnim.timeScale,
                () => PlayBackIdle(true));
        }
    }
    public void PlayBackIdleAnim()
    {
        _tween?.Kill();
        PlayBackIdle(true);
    }
    public void PlaySkateAnim(AnimState skateAnim, bool isLoop = true)
    {
        _tween?.Kill();
        switch (skateAnim)
        {
            case AnimState.Skate1:
                PlaySkate1(isLoop);
                break;
            case AnimState.Skate2:
                PlaySkate2(isLoop);
                break;
            case AnimState.Skate3:
                PlaySkate3(isLoop);
                break;
            case AnimState.Skate4:
                PlaySkate4(isLoop);
                break;
            case AnimState.Skate5:
                PlaySkate5(isLoop);
                break;
            default:
                PlaySkate1(isLoop);
                break;
        }
        if (!isLoop)
        {
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
        }
    }
    public AnimState GetRandomSkateAnim()
    {
        var rd = UnityEngine.Random.Range((int)AnimState.Skate1, (int)AnimState.Skate5 + 1);
        switch (rd)
        {
            case (int)AnimState.Skate1:
                return AnimState.Skate1;
            case (int)AnimState.Skate2:
                return AnimState.Skate2;
            case (int)AnimState.Skate3:
                return AnimState.Skate3;
            case (int)AnimState.Skate4:
                return AnimState.Skate4;
            case (int)AnimState.Skate5:
                return AnimState.Skate5;
            default:
                return AnimState.Skate1;
        }
    }
    public void PlayRandomSkateAnim()
    {
        _tween?.Kill();
        var rd = UnityEngine.Random.Range((int)AnimState.Skate1, (int)AnimState.Skate5 + 1);
        switch (rd)
        {
            case (int) AnimState.Skate1:
                PlaySkate1(false);
                break;
            case (int) AnimState.Skate2:
                PlaySkate2(false);
                break;
            case (int) AnimState.Skate3:
                PlaySkate3(false);
                break;
            case (int) AnimState.Skate4:
                PlaySkate4(false);
                break;
            case (int) AnimState.Skate5:
                PlaySkate5(false);
                break;
            default:
                PlaySkate1(false);
                break;
        }

        _tween?.Kill();
        _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayRandomSkateAnim());
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
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, specialAnim, false);
    }
    private void PlayWaveHand()
    {
        if (animState == AnimState.WaveHand) return;
        animState = AnimState.WaveHand;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, wavehandAnim, false);
    }
    private void PlayDizzy(bool isLoop)
    {
        if (animState == AnimState.Dizzy) return;
        animState = AnimState.Dizzy;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, dizzyAnim, isLoop);
    }
    private void PlaySlow(bool isLoop)
    {
        if (animState == AnimState.Slow) return;
        animState = AnimState.Slow;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, slowAnim, isLoop);
        if (!isLoop)
        {
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
        }
    }
    private void PlayRunFast(bool isLoop)
    {
        if (animState == AnimState.RunFast) return;
        animState = AnimState.RunFast;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, runFastAnim, isLoop);
        if (!isLoop)
        {
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
        }
    }
    private void PlayPush(bool isLoop)
    {
        if (animState == AnimState.Push) return;
        animState = AnimState.Push;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, pushAnim, isLoop);
    }
    private void PlayThrow(bool isLoop)
    {
        if (animState == AnimState.Throw) return;
        animState = AnimState.Throw;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, throwAnim, isLoop);
    }
    private void PlaySkate1(bool isLoop)
    {
        if (animState == AnimState.Skate1) return;
        animState = AnimState.Skate1;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, skating1Anim, isLoop);
    }
    private void PlaySkate2(bool isLoop)
    {
        if (animState == AnimState.Skate2) return;
        animState = AnimState.Skate2;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, skating2Anim, isLoop);
    }
    private void PlaySkate3(bool isLoop)
    {
        if (animState == AnimState.Skate3) return;
        animState = AnimState.Skate3;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, skating3Anim, isLoop);
    }
    private void PlaySkate4(bool isLoop)
    {
        if (animState == AnimState.Skate4) return;
        animState = AnimState.Skate4;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, skating4Anim, isLoop);
    }
    private void PlaySkate5(bool isLoop)
    {
        if (animState == AnimState.Skate5) return;
        animState = AnimState.Skate5;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, skating5Anim, isLoop);
    }
    private void PlayJumpWin(bool isLoop)
    {
        if (animState == AnimState.JumpWin) return;
        animState = AnimState.JumpWin;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, jumpWinAnim, isLoop);
    }
    private void PlayThrowback(bool isLoop)
    {
        if (animState == AnimState.Throwback) return;
        animState = AnimState.Throwback;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, throwBackAnim, isLoop);
    }
    private void PlayBackIdle(bool isLoop)
    {
        if (animState == AnimState.BackIdle) return;
        animState = AnimState.BackIdle;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, backIdleAnim, isLoop);
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
            case AnimState.Throw:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(throwAnim);
                break;
            case AnimState.Skate1:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(skating1Anim);
                break;
            case AnimState.Skate2:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(skating2Anim);
                break;
            case AnimState.Skate3:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(skating3Anim);
                break;
            case AnimState.Skate4:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(skating4Anim);
                break;
            case AnimState.Skate5:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(skating5Anim);
                break;
            case AnimState.JumpWin:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(jumpWinAnim);
                break;
            case AnimState.Throwback:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(throwBackAnim);
                break;
            case AnimState.BackIdle:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(backIdleAnim);
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
        Throw,
        /// <summary>
        /// Normal Skating
        /// </summary>
        Skate1,
        /// <summary>
        /// Forward Inclined Skating
        /// </summary>
        Skate2,
        /// <summary>
        /// One leg Skating
        /// </summary>
        Skate3,
        /// <summary>
        /// Behind Inclined Skating
        /// </summary>
        Skate4,
        /// <summary>
        /// Forward Inclined Skating 2
        /// </summary>
        Skate5,
        JumpWin,
        Throwback,
        BackIdle,
    }
    #endregion
   
}
