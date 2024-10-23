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
        EventManager.Instance.AddListener(GameEvents.ON_CHARACTER_JUMP, OnCharacterJump);
        EventManager.Instance.AddListener(GameEvents.ON_CHARACTER_GROUNDED, OnCharacterGrounded);
    }

	private void Update()
    {
        // Must be checked every frame 
        animator.SetFloat(AnimatorParameters.SPEED, Character.Instance.GetAbility<CharacterMovementAbility>().GetSpeed());
        animator.SetFloat(AnimatorParameters.MOTION_SPEED, Character.Instance.GetAbility<CharacterMovementAbility>().GetInputMagnitude());
        animator.SetFloat(AnimatorParameters.ANIMATION_BLEND, Character.Instance.GetAbility<CharacterMovementAbility>().GetAnimationBlendMagnitude());

        animator.SetFloat("RemappedMoveInputX", Character.Instance.GetAbility<CharacterOrientationAbility>().GetInputDirection().x);
        animator.SetFloat("RemappedMoveInputY", Character.Instance.GetAbility<CharacterOrientationAbility>().GetInputDirection().y);
        animator.SetBool("Moving", Character.Instance.GetAbility<CharacterOrientationAbility>().GetInputDirection().x != 0f || Character.Instance.GetAbility<CharacterOrientationAbility>().GetInputDirection().y != 0f);

        // TODO: Convert to Events
        animator.SetBool(AnimatorParameters.GROUNDED, Character.Instance.GetAbility<CharacterGroundCheckAbility>().Grounded);
        animator.SetBool(AnimatorParameters.FREE_FALL, CharacterJump.Instance.FreeFall);

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

	private void OnCharacterGrounded(object sender, EventArgs e)
	{
		if (Character.Instance.GetAbility<CharacterGroundCheckAbility>().Grounded)
		{
			animator.SetTrigger("Grounded");
		}
	}

	private void OnCharacterJump(object sender, EventArgs e)
	{
		//animator.SetBool(AnimatorParameters.JUMP, InputManager.Instance.jump);

        if (Character.Instance.GetAbility<CharacterGroundCheckAbility>().Grounded)
        {
            animator.SetTrigger("Jumping");
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

        // Set target under RFX1_TransformMotion script on vfxInstance child GameObject
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (groundPlane.Raycast(ray, out float position))
        {
            Vector3 targetPosition = ray.GetPoint(position);
            // Create an empty GameObject at the target position
            GameObject target = new GameObject("Target");
            target.transform.position = targetPosition;
            // Assign the target GameObject to the RFX1_TransformMotion script
            vfxInstance.GetComponentInChildren<RFX1_TransformMotion>().Target = target;

            // Start the destruction countdown
            Destroy(target, 2.5f); // Destroy after 2.5 seconds if it hasn't collided
        }
    }
}