using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputListener : MonoBehaviour
    {
        PlayerInput _input;
        PlayerInput PInput
        {
            get
            {
                if (_input == null) _input = GetComponent<PlayerInput>();
                return _input;
            }
        }

        public Vector2 MoveValue { get; private set; }
        public Vector2 MousePosition { get; private set; }

        void Awake()
        {
            PInput.onActionTriggered += OnActionTriggered;
        }

        void OnActionTriggered(InputAction.CallbackContext context)
        {
            switch (context.action.name)
            {
                case "Move":
                    if (context.performed) MoveValue = context.ReadValue<Vector2>();
                    else if (context.canceled) MoveValue = Vector2.zero;
                    break;

                case "MousePosition":
                    MousePosition = context.ReadValue<Vector2>();
                    break;
            }
        }
    }
}
