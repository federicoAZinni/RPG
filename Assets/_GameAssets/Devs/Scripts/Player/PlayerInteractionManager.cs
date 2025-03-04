using UnityEngine;

namespace RPG.Player
{
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerInteractionManager : MonoBehaviour, IPlayerModule
    {
        bool moduleEnabled;
        PlayerController pController;
        PlayerInputListener inputListener;
        SelectionCursor cursor;

        IInteractable currentICandidate;
        Collider[] possibleInteractables;
        BoxCollider playerCollider;

        void Awake()
        {
            possibleInteractables = new Collider[10];
            playerCollider = GetComponent<BoxCollider>();
        }

        public void Init(PlayerController controller)
        {
            pController = controller;
            cursor = pController.GetCursor();
            inputListener = pController.GetInputListener();
            inputListener.OnAttack += OnInteract;
        }

        public void ToggleModule(bool toggle) => moduleEnabled = toggle;

        void OnInteract()
        {
            if (!moduleEnabled || currentICandidate == null || !cursor.IsLocked) return;

            currentICandidate.Interact(gameObject, pController);
            pController.OnInteractionPerformed();
        }

        void Update()
        {
            if (!moduleEnabled) return;
            CheckCursor();
            CheckCollision();
        }

        void CheckCursor()
        {
            int quantity = Physics.OverlapSphereNonAlloc(cursor.transform.position, .15f, possibleInteractables, LayerMask.GetMask("Characters", "Interactables"));

            for (int i = 0; i < quantity; i++)
            {
                if (possibleInteractables[i].GetComponent<IDamageable>() != null)
                {
                    currentICandidate = null;
                    return;
                }

                IInteractable possibleObj = possibleInteractables[i].GetComponent<IInteractable>();
                if (possibleObj == null || currentICandidate == possibleObj || possibleObj.GetInteractionType() == InteractableType.ByCollision || !possibleObj.InteractionEnabled) continue;

                currentICandidate = possibleObj;
                cursor.LockCursor(currentICandidate.GetTransform(), currentICandidate.GetObjectSize());
                cursor.ToggleCursorVis(true);
                pController.UpdateCursorWorldPos(cursor.transform.position);
                return;
            }

            if (cursor.IsLocked) return;
            currentICandidate = null;
        }

        void CheckCollision()
        {
            int quantity = Physics.OverlapBoxNonAlloc(transform.position, playerCollider.bounds.extents, possibleInteractables, Quaternion.identity, LayerMask.GetMask("Interactables"));

            for (int i = 0; i < quantity; i++)
            {
                IInteractable possibleObj = possibleInteractables[i].GetComponent<IInteractable>();
                if (possibleObj == null || possibleObj.GetInteractionType() == InteractableType.Clickeable || !possibleObj.InteractionEnabled) continue;
                possibleObj.Interact(gameObject, pController);
                pController.OnInteractionPerformed();
            }
        }
    }
}
