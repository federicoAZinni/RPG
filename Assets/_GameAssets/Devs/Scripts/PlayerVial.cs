using UnityEngine;
using RPG.Player;

namespace RPG
{

    public class PlayerVial : MonoBehaviour, IInteractable
    {
        enum VialType { HP, Mana }

        [SerializeField] VialType type;
        [SerializeField] float ammountToGive;
        
        public bool InteractionEnabled { get; private set; } = true;

        public void Interact(GameObject playerObj, PlayerController player)
        {
            if (!InteractionEnabled) return;

            if (type == VialType.HP)
            {
                PlayerHealth ph = playerObj.GetComponent<PlayerHealth>();
                ph.GiveHP(ammountToGive);
                gameObject.SetActive(false);
                return;
            }
        }

        public Vector3 GetObjectSize() => transform.localScale;
        public InteractableType GetInteractionType() => InteractableType.ByCollision;
        public Transform GetTransform() => transform;

    }
}
