using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : Singleton<CharacterAnimator>
{
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        //EventManager.Instance.AddListener(GameEvents.ON_CHARACTER_ATTACK_MOUSE, OnCharacterAttackMouse);
        //EventManager.Instance.AddListener(GameEvents.ON_CHARACTER_ATTACK_MOUSE_CANCELED, OnCharacterAttackMouseCanceled);
    }

    void Update()
    {
        Debug.Log($"Speed: {CharacterMovement.Instance._animationBlend}, MotionSpeed: {CharacterMovement.Instance._inputMagnitude}");
        // Must be checked every frame 
        animator.SetFloat(AnimatorParameters.SPEED, CharacterMovement.Instance._animationBlend);
        animator.SetFloat(AnimatorParameters.MOTION_SPEED, CharacterMovement.Instance._inputMagnitude);

        // TODO: Convert to Events
        animator.SetBool(AnimatorParameters.GROUNDED, CharacterGroundCheck.Instance.Grounded);
        animator.SetBool(AnimatorParameters.JUMP, InputManager.Instance.jump);
        animator.SetBool(AnimatorParameters.FREE_FALL, CharacterJump.Instance.FreeFall);

        // Remapped move inputs
        animator.SetFloat(AnimatorParameters.REMAPPED_MOVE_INPUT_X, CharacterOrientation.Instance.GetInputDirection().x);
        animator.SetFloat(AnimatorParameters.REMAPPED_MOVE_INPUT_Y, CharacterOrientation.Instance.GetInputDirection().y);

        // Equipped weapon id
        animator.SetFloat(AnimatorParameters.EQUIPPED_WEAPON_ID, CharacterInventory.Instance.EquippedWeaponId);

        // Set Locomotion State
        if (CharacterMovement.Instance._animationBlend > 0.1f)
        {
            if (CharacterMovement.Instance._animationBlend < 2.0f)
            {
				animator.SetBool(AnimatorParameters.WALKING, true);
			}
            else
            {
				animator.SetBool(AnimatorParameters.RUNNING, true);
			}
		}
		else
        {
			animator.SetBool(AnimatorParameters.IDLE, true);
		}
    }

    private void OnCharacterAttackMouseCanceled(object sender, EventArgs e)
    {
        Debug.Log("OnCharacterAttackMouseCanceled");
    }

   
    private void OnCharacterAttackMouse(object sender, EventArgs e)
    {
        Debug.Log("OnCharacterAttackMouse");
        // Set combat mode true for next 10 seconds with a coroutine in lambda

        // TODO : Delete after testing
        StartCoroutine(SetFlagTemporaryCoroutine(true, 10f));
    }

    // TODO: Delete after testing.
    public LocomotionModeType LocomotionMode { get; set; } = LocomotionModeType.Combat;
    private IEnumerator SetFlagTemporaryCoroutine(bool value, float duration)
    {
        Debug.Log("SetFlagTemporaryCoroutine");
        animator.SetFloat(AnimatorParameters.LOCOMOTIOM_MODE, LocomotionModeType.Combat.GetValue<LocomotionModeType, float>());
        LocomotionMode = LocomotionModeType.Combat;
        yield return new WaitForSeconds(duration);
        animator.SetFloat(AnimatorParameters.LOCOMOTIOM_MODE, LocomotionModeType.Idle.GetValue<LocomotionModeType, float>());
        LocomotionMode = LocomotionModeType.Idle;
    }
    //


    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(Character.Instance.GetComponent<CharacterController>().center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(Character.Instance.GetComponent<CharacterController>().center), FootstepAudioVolume);
        }
    }

    private void OnFootL(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(Character.Instance.GetComponent<CharacterController>().center), FootstepAudioVolume);
            }
        }
    }

    private void OnStrike(AnimationEvent animationEvent)
    {
        Debug.Log("OnStrike");
    }
}
