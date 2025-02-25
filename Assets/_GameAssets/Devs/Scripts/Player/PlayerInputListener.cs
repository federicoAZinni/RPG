using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Player
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
        public Vector2 JoyLookValue { get; private set; }
        
        public System.Action OnAttack;
        public System.Action<PowerTypes> OnPower;

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

                case "Look":
                    if (context.performed) JoyLookValue = context.ReadValue<Vector2>();
                    else if (context.canceled) JoyLookValue = Vector2.zero;
                    break;

                case "MousePosition":
                    if (context.canceled) break;
                    MousePosition = context.ReadValue<Vector2>();
                    break;

                case "Attack":
                    if (context.performed) OnAttack?.Invoke();
                    break;

                case "Power1":
                    if (context.performed) OnPower?.Invoke(PowerTypes.Power1);
                    break;

                case "Power2":
                    if (context.performed) OnPower?.Invoke(PowerTypes.Power2);
                    break;

                case "Power3":
                    if (context.performed) OnPower?.Invoke(PowerTypes.Power3);
                    break;

                case "Power4":
                    if (context.performed) OnPower?.Invoke(PowerTypes.Power4);
                    break;
            }
        }
    }
}
