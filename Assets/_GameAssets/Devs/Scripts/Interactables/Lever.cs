using RPG.Player;
using UnityEngine;
using UnityEngine.Events;

namespace RPG
{
    public class Lever : MonoBehaviour,IInteractable
    {
        public UnityEvent OnLeverDown;
        public UnityEvent OnLeverUp;

        enum LeverPosition { Up,Down}

        LeverPosition leverCurrentPosition;


        public bool InteractionEnabled => true;

        public InteractableType GetInteractionType() => InteractableType.Clickeable;

        public Vector3 GetObjectSize() => transform.localScale;

        public Transform GetTransform() => transform;

        public void Interact(GameObject playerObj, PlayerController player)
        {
            leverCurrentPosition = leverCurrentPosition == LeverPosition.Up ? LeverPosition.Down : LeverPosition.Up;

            switch (leverCurrentPosition)
            {   
                case LeverPosition.Up:
                    OnLeverUp?.Invoke();
                    break;
                case LeverPosition.Down:
                    OnLeverDown?.Invoke();
                    break;
                default:
                    break;
            }

            
        }

    }
}
