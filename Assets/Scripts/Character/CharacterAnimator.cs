using System;
using System.Collections;
using Scripts.Core;
using Scripts.Entities.Class;
using Scripts.Entities.Enum;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterAnimator : Singleton<CharacterAnimator>
{
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    private Animator animator;
    public GameObject vfxPrefab; // Assign your VFX prefab in the Inspector
    public Transform launchPoint; // Assign a child GameObject as the launch point

    public LocomotionModeType LocomotionMode { get; private set; } = LocomotionModeType.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        EventManager.Instance.AddListener(GameEvents.ON_CHARACTER_ATTACK_MOUSE, OnCharacterAttackMouse);
        EventManager.Instance.AddListener(GameEvents.ON_CHARACTER_ATTACK_MOUSE_CANCELED, OnCharacterAttackMouseCanceled);

        EventManager.Instance.AddListener<OnCharacterAttackSpellEventArgs>(GameEvents.ON_CHARACTER_ATTACK_SPELL, OnCharacterAttackSpell);

        EventManager.Instance.AddListener<OnCharacterLocomotionChangedEventArgs>(GameEvents.ON_CHARACTER_LOCOMOTION_MODE_CHANGED, OnCharacterLocomotionModeChanged);
    }

    private void Update()
    {
        // Must be checked every frame 
        animator.SetFloat(AnimatorParameters.SPEED, CharacterMovement.Instance.GetSpeed());
        animator.SetFloat(AnimatorParameters.MOTION_SPEED, CharacterMovement.Instance.GetInputMagnitude());
        animator.SetFloat(AnimatorParameters.ANIMATION_BLEND, CharacterMovement.Instance.GetAnimationBlendMagnitude());

        // TODO: Convert to Events
        animator.SetBool(AnimatorParameters.GROUNDED, CharacterGroundCheck.Instance.Grounded);
        animator.SetBool(AnimatorParameters.JUMP, InputManager.Instance.jump);
        animator.SetBool(AnimatorParameters.FREE_FALL, CharacterJump.Instance.FreeFall);

        //// Remapped move inputs
        //animator.SetFloat(AnimatorParameters.REMAPPED_MOVE_INPUT_X,
        //    CharacterOrientation.Instance.GetInputDirection().x);
        //animator.SetFloat(AnimatorParameters.REMAPPED_MOVE_INPUT_Y,
        //    CharacterOrientation.Instance.GetInputDirection().y);



        animator.SetFloat("RemappedMoveInputX", CharacterOrientation.Instance.GetInputDirection().x);
        animator.SetFloat("RemappedMoveInputY", CharacterOrientation.Instance.GetInputDirection().y);
        animator.SetBool("Moving", CharacterOrientation.Instance.GetInputDirection().x != 0f || CharacterOrientation.Instance.GetInputDirection().y != 0f);

        // Set Locomotion State
        //if (CharacterMovement.Instance.GetSpeed() > 0.1f)
        //{
        //    animator.SetBool(AnimatorParameters.IDLE, false);
        //    animator.SetBool(AnimatorParameters.MOVING, true);
        //}
        //else
        //{
        //    animator.SetBool(AnimatorParameters.IDLE, true);
        //    animator.SetBool(AnimatorParameters.MOVING, false);
        //}

        // Equipped weapon 
        if (CharacterInventory.Instance.EquippedWeaponId > 0)
        {
            animator.SetBool(AnimatorParameters.WEAPON_EQUIPPED, true);
            animator.SetInteger(AnimatorParameters.EQUIPPED_WEAPON_ID, CharacterInventory.Instance.EquippedWeaponId);
        }
        else
        {
            animator.SetBool(AnimatorParameters.WEAPON_EQUIPPED, false);
            animator.SetInteger(AnimatorParameters.EQUIPPED_WEAPON_ID, 0);
        }
    }

    private void OnCharacterAttackSpell(object sender, OnCharacterAttackSpellEventArgs e)
    {
        if (e.SpellId == 0)
        {
            animator.SetInteger(AnimatorParameters.CASTING_SPELL_ID, 0);
            return;
        }

        LocomotionMode = LocomotionModeType.Combat;
        animator.SetInteger(AnimatorParameters.LOCOMOTIOM_MODE, LocomotionMode.GetValue<LocomotionModeType, int>());
        animator.SetInteger(AnimatorParameters.CASTING_SPELL_ID, e.SpellId);
        animator.SetTrigger(AnimatorParameters.ATTACK);

    }

    private void OnCharacterLocomotionModeChanged(object sender, OnCharacterLocomotionChangedEventArgs e)
    {
        Debug.Log("OnCharacterLocomotionModeChanged");
        LocomotionMode = e.LocomotionMode;
        animator.SetInteger(AnimatorParameters.LOCOMOTIOM_MODE, LocomotionMode.GetValue<LocomotionModeType, int>());
    }

    [ContextMenu("Combat Mode")]
    public void SetCombatMode()
    {
        LocomotionMode = LocomotionModeType.Combat;
        animator.SetInteger(AnimatorParameters.LOCOMOTIOM_MODE, LocomotionMode.GetValue<LocomotionModeType, int>());
    }
    
    [ContextMenu("Idle Mode")]
    public void SetIdleMode()
    {
        LocomotionMode = LocomotionModeType.Idle;
        animator.SetInteger(AnimatorParameters.LOCOMOTIOM_MODE, LocomotionMode.GetValue<LocomotionModeType, int>());
    }

    private void OnCharacterAttackMouseCanceled(object sender, EventArgs e)
    {
        Debug.Log("OnCharacterAttackMouseCanceled");
    }

    private void OnCharacterAttackMouse(object sender, EventArgs e)
    {
        Debug.Log("OnCharacterAttackMouse");

        EventManager.Instance.Trigger(GameEvents.ON_CHARACTER_LOCOMOTION_MODE_CHANGED, this, new OnCharacterLocomotionChangedEventArgs
        {
            LocomotionMode = LocomotionModeType.Combat
        });

        animator.SetTrigger(AnimatorParameters.ATTACK);
    }

    private IEnumerator SetFlagTemporaryCoroutine(bool value, float duration)
    {
        Debug.Log("SetFlagTemporaryCoroutine");
        animator.SetFloat(AnimatorParameters.LOCOMOTIOM_MODE, LocomotionModeType.Combat.GetValue<LocomotionModeType, float>());
        LocomotionMode = LocomotionModeType.Combat;
        yield return new WaitForSeconds(duration);
        animator.SetFloat(AnimatorParameters.LOCOMOTIOM_MODE, LocomotionModeType.Idle.GetValue<LocomotionModeType, float>());
        LocomotionMode = LocomotionModeType.Idle;
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index],
                    transform.TransformPoint(Character.Instance.GetComponent<CharacterController>().center),
                    FootstepAudioVolume);
            }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            AudioSource.PlayClipAtPoint(LandingAudioClip,
                transform.TransformPoint(Character.Instance.GetComponent<CharacterController>().center),
                FootstepAudioVolume);
    }

    private void FootL(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index],
                    transform.TransformPoint(Character.Instance.GetComponent<CharacterController>().center),
                    FootstepAudioVolume);
            }
    }

    private void FootR(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index],
                    transform.TransformPoint(Character.Instance.GetComponent<CharacterController>().center),
                    FootstepAudioVolume);
            }
    }

    private void OnStrike(AnimationEvent animationEvent)
    {
        Debug.Log("OnStrike");
    }

    public void Hit(AnimationEvent animationEvent)
    {
        // Instantiate the fireball prefab
        GameObject vfxInstance = Instantiate(vfxPrefab, launchPoint.position, Quaternion.identity);

        // Start the destruction countdown
        Destroy(vfxInstance, 2.5f); // Destroy after 2.5 seconds if it hasn't collided
    }
}