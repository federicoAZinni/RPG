using UnityEngine;

namespace RPG
{
    public class AnimationEventStateBehavior : StateMachineBehaviour
    {
        public string eventName;
        [Range(0f, 1f)] public float triggerTime;
        bool triggered;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            triggered = false;
        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float currentTime = stateInfo.normalizedTime % 1f;

            if (!triggered && triggerTime >= currentTime)
            {
                NotifyEvent(animator);
                triggered = true;
            }
        }

        public void NotifyEvent(Animator anim)
        {
            if(anim.transform.root.TryGetComponent<AnimationEventReciever>(out AnimationEventReciever animationEventReciever))
            {
                animationEventReciever.OnAnimationEventTriggered(eventName);
            }
        }
    }
}
