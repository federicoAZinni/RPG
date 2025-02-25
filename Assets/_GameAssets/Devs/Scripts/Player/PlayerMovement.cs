using UnityEngine;

namespace RPG.Player
{
    public class PlayerMovement : MonoBehaviour, IPlayerModule
    {
        [SerializeField] float baseMovementSpeed;
        [SerializeField] float rotationSpeed;

        bool moduleEnabled;
        PlayerInputListener inputListener;
        PlayerController pController;

        public void Init(PlayerController controller)
        {
            pController = controller;
            inputListener = pController.GetInputListener();
        }

        public void ToggleModule(bool toggle) => moduleEnabled = toggle;


        void Update()
        {
            if (!moduleEnabled) return;
            float currentMovementSpeed = baseMovementSpeed * Time.deltaTime;
            if (inputListener.MoveValue.x != 0 && inputListener.MoveValue.y != 0) currentMovementSpeed *= .5f;

            transform.position += currentMovementSpeed * inputListener.MoveValue.y * Vector3.forward;
            transform.position += currentMovementSpeed * inputListener.MoveValue.x * Vector3.right;

            //if (!pController.GetCursorWorldPos(out Vector3 cursorPos)) return;
            Vector3 dirVector = pController.GetCursor().transform.position - transform.position;
            transform.forward = new Vector3(dirVector.x, 0, dirVector.z);
        }
    }
}
