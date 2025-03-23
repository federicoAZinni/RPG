using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace RPG
{
    public class AnimationEventReciever : MonoBehaviour
    {
        [SerializeField] List<AnimationEvent> events = new();

        public void OnAnimationEventTriggered(string nameEvent)
        {
            AnimationEvent eventMatch = events.Find(se => se.functionName == nameEvent);
            eventMatch?.eventUnity?.Invoke();
        }
    }

    [Serializable]
    public class AnimationEvent
    {
        public string functionName;
        public UnityEvent eventUnity;
    }
}
