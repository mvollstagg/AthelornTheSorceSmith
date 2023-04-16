using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class ActionTrigger : MonoBehaviour
    {
        public enum TriggerType
        {
            None,
            Start,
            KeyDown,
            KeyUp,
            ButtonDown,
            ButtonUp,
            TriggerEnter,
            TriggerExit,
            CollisionEnter,
            CollisionExit
        }

        public TriggerType Trigger;
        public float Delay;
        public KeyCode Key;
        public string Button;
        public string ColliderTag;

        public UnityEngine.Events.UnityEvent Action;

        void Start()
        {
            if (Trigger == TriggerType.Start)
            {
                ExecuteAction();
                enabled = false;
            }
            else if (!(Trigger == TriggerType.KeyDown || Trigger == TriggerType.KeyUp || Trigger == TriggerType.ButtonDown || Trigger == TriggerType.ButtonUp))
            {
                enabled = false;
            }
        }

        void Update()
        {
            if (Trigger == TriggerType.KeyDown && Input.GetKeyDown(Key))
            {
                ExecuteAction();
            }
            else if (Trigger == TriggerType.KeyUp && Input.GetKeyUp(Key))
            {
                ExecuteAction();
            }
            else if (Trigger == TriggerType.ButtonDown && Input.GetButtonDown(Button))
            {
                ExecuteAction();
            }
            else if (Trigger == TriggerType.ButtonUp && Input.GetButtonUp(Button))
            {
                ExecuteAction();
            }
        }

        public void ExecuteAction()
        {
            if (Delay > 0f)
            {
                StartCoroutine(DelayedAction());
            }
            else
            {
                Action.Invoke();
            }
        }

        IEnumerator DelayedAction()
        {
            yield return new WaitForSeconds(Delay);
            Action.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Trigger == TriggerType.TriggerEnter)
            {
                if (string.IsNullOrEmpty(ColliderTag) || other.CompareTag(ColliderTag))
                {
                    ExecuteAction();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (Trigger == TriggerType.TriggerExit)
            {
                if (string.IsNullOrEmpty(ColliderTag) || other.CompareTag(ColliderTag))
                {
                    ExecuteAction();
                }
            }
        }

        private void OnCollisionEnter (Collision collision)
        {
            if (Trigger == TriggerType.CollisionEnter)
            {
                if (string.IsNullOrEmpty(ColliderTag) || collision.gameObject.CompareTag(ColliderTag))
                {
                    ExecuteAction();
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (Trigger == TriggerType.CollisionExit)
            {
                if (string.IsNullOrEmpty(ColliderTag) || collision.gameObject.CompareTag(ColliderTag))
                {
                    ExecuteAction();
                }
            }
        }
    }
}