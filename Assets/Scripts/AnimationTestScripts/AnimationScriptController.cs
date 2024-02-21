using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptController : MonoBehaviour
{
    [SerializeField] private List<SoMyAnimation> farmingAnimations;

    public SoMyAnimation currentAnimation;
    
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) StartCoroutine(SwitchAnimationRoutine(null));
        if(Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(SwitchAnimationRoutine(farmingAnimations[0]));
        if(Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(SwitchAnimationRoutine(farmingAnimations[1]));
        if(Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(SwitchAnimationRoutine(farmingAnimations[2]));
        if(Input.GetKeyDown(KeyCode.Alpha4)) StartCoroutine(SwitchAnimationRoutine(farmingAnimations[3]));
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (animator.GetCurrentAnimatorClipInfo(1)[0].clip.name == currentAnimation.idle.name)
            {
                Debug.Log(currentAnimation.idle.name);
                StartCoroutine(SwitchToActiveAnimation(currentAnimation));
            }
        }
    }

    public void ActivateAnimation()
    {
        animator.Play(currentAnimation.active.name, 1);
    }
    
    private IEnumerator SwitchAnimationRoutine(SoMyAnimation newAnimation)
    {
        if(currentAnimation != null)
        {
            animator.CrossFadeInFixedTime(currentAnimation.finish.name, 0.1f);
            yield return new WaitForSeconds(currentAnimation.finish.length);
        }

        if (newAnimation == null)
        {
            currentAnimation = null;
            yield break;

        }
        currentAnimation = newAnimation;
            
        animator.CrossFadeInFixedTime(currentAnimation.start.name, 0.1f);
        yield return new WaitForSeconds(currentAnimation.start.length);

        animator.CrossFadeInFixedTime(currentAnimation.idle.name, 0.1f);
    }

    private IEnumerator SwitchToActiveAnimation(SoMyAnimation newAnimation)
    {
        if (currentAnimation == null) yield break;
        
        animator.CrossFadeInFixedTime(currentAnimation.active.name, 0.1f);
        yield return new WaitForSeconds(currentAnimation.active.length);
        
        animator.CrossFadeInFixedTime(currentAnimation.idle.name, 0.1f);
    }
    
}