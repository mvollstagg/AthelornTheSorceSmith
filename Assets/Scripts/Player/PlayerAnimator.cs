using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    private const string SPEED = "Speed";
    private const string MOTIONSPEED = "MotionSpeed";
    private const string GROUNDED = "Grounded";
    private const string JUMP = "Jump";
    private const string FREEFALL = "FreeFall";
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat(SPEED, PlayerController.Instance._animationBlend);
        animator.SetFloat(MOTIONSPEED, PlayerController.Instance._inputMagnitude);
        animator.SetBool(GROUNDED, PlayerController.Instance.Grounded);
        animator.SetBool(JUMP, InputManager.Instance.jump);
        animator.SetBool(FREEFALL, PlayerController.Instance.FreeFall);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(PlayerController.Instance.GetComponent<CharacterController>().center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(PlayerController.Instance.GetComponent<CharacterController>().center), FootstepAudioVolume);
        }
    }
}
