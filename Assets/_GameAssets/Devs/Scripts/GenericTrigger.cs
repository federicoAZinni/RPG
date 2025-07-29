using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG
{
    [System.Flags]
    public enum TriggerTarget { Player, AI, Objects }

    public class GenericTrigger : MonoBehaviour
    {
        class ObjectOnBounds
        {
            public Collider obj;
            public bool insideTrigger;
        }

        [SerializeField] UniqueID triggerID;
        [SerializeField] Bounds triggerBounds;
        [SerializeField] TriggerTarget targets;
        [SerializeField] int maxElementsToTrigger;
        [SerializeField] bool enableTriggerOnStart;
        [SerializeField] bool triggerOnce;

        public bool TriggerEnabled { get; private set; }
        public bool HasTriggered { get; private set; }

        Collider[] overlapColliders;
        List<ObjectOnBounds> collidersOnTrigger;

        public UnityEvent OnTrigger;

        void Awake()
        {
            overlapColliders = new Collider[5 + maxElementsToTrigger];
            collidersOnTrigger = new List<ObjectOnBounds>();
        }

        void Start()
        {
            TriggerEnabled = enableTriggerOnStart;
        }

        void Update()
        {
            int quantity = Physics.OverlapBoxNonAlloc(transform.position + triggerBounds.center, triggerBounds.extents, overlapColliders, Quaternion.identity);

            for (int i = 0; i < quantity; i++)
            {
                if (!CheckCollider(overlapColliders[i])) continue;
                if (CheckIfColliderInsideTrigger(overlapColliders[i])) continue;
                if (HasTriggered && !triggerOnce) HasTriggered = true;
                collidersOnTrigger.Add(new ObjectOnBounds { obj = overlapColliders[i], insideTrigger = true });
            }

            RemoveObjectsOutsideTrigger();
            if (collidersOnTrigger.Count >= maxElementsToTrigger) Trigger();
        }

        public void ToggleTrigger(bool toggle) => TriggerEnabled = toggle;

        bool CheckCollider(Collider collider)
        {
            if (HastFlag(TriggerTarget.Player) && collider.CompareTag("Player")) return true;
            if (HastFlag(TriggerTarget.Objects) && collider.gameObject.layer == LayerMask.NameToLayer("Characters")) return true;
            if (HastFlag(TriggerTarget.AI) && collider.gameObject.layer == LayerMask.NameToLayer("Default")) return true;
            return false;
        }

        bool CheckIfColliderInsideTrigger(Collider collider)
        {
            int size = collidersOnTrigger.Count;
            for (int i = 0; i < size; i++)
            {
                if (collidersOnTrigger[i].obj != collider) continue;
                collidersOnTrigger[i].insideTrigger = true;
                return true;
            }

            return false;
        }

        void RemoveObjectsOutsideTrigger()
        {
            int size = collidersOnTrigger.Count;
            for (int i = 0; i < size; i++)
            {
                if (collidersOnTrigger[i].insideTrigger) continue;
                collidersOnTrigger.RemoveAt(i--);
                size--;
            }
        }

        bool HastFlag(TriggerTarget expectedTarget) => (targets & expectedTarget) == expectedTarget;

        void Trigger()
        {
            if (HasTriggered) return;

            OnTrigger?.Invoke();
            HasTriggered = true;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + triggerBounds.center, triggerBounds.size);    
            Gizmos.DrawCube(transform.position + triggerBounds.center, triggerBounds.size);    
        }
    }
}
