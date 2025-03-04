using UnityEngine;
using RPG.Player;

namespace RPG
{
    public class TestClickeableInteractable : MonoBehaviour, IInteractable
    {
        public bool InteractionEnabled { get; private set; } = true;

        public InteractableType GetInteractionType() => InteractableType.Clickeable;

        public void Interact(GameObject playerObj, PlayerController player)
        {
            if (!InteractionEnabled) return;
            Debug.LogFormat("Player interacted with a {0} object", name);
        }

        public Vector3 GetObjectSize() => transform.localScale;
        public Transform GetTransform() => transform;
    }
}
