using RPG.Player;
using UnityEngine;

namespace RPG
{
    public enum InteractableType { Clickeable, ByCollision }
    
    public interface IInteractable
    {
        public bool InteractionEnabled { get; }

        public InteractableType GetInteractionType();
        public void Interact(GameObject playerObj, PlayerController player);
        public Vector3 GetObjectSize();
        public Transform GetTransform();
    }
}
