using UnityEngine;

namespace RPG.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour, IPlayerModule
    {
        [SerializeField] float baseMovementSpeed;
        [SerializeField] float rotationSpeed;

        CharacterController _charctrl;
        CharacterController CharCtrl
        {
            get
            {
                if (_charctrl == null) _charctrl = GetComponent<CharacterController>();
                return _charctrl;
            }
        }

        bool moduleEnabled;
        PlayerInputListener inputListener;
        PlayerController pController;
        SelectionCursor cursor;

        public void Init(PlayerController controller)
        {
            pController = controller;
            inputListener = pController.GetInputListener();
            cursor = pController.GetCursor();
        }

        public void ToggleModule(bool toggle) => moduleEnabled = toggle;


        void Update()
        {
            if (!moduleEnabled) return;
            float currentMovementSpeed = baseMovementSpeed * Time.deltaTime;
            if (inputListener.MoveValue.x != 0 && inputListener.MoveValue.y != 0) currentMovementSpeed *= .5f;

            Vector3 dir = transform.TransformDirection(new Vector3(inputListener.MoveValue.x, 0, inputListener.MoveValue.y));
            CharCtrl.Move(currentMovementSpeed * Time.deltaTime * dir);

            if (!cursor.IsLocked) cursor.transform.position += Time.deltaTime * dir * currentMovementSpeed;

            //if (!pController.GetCursorWorldPos(out Vector3 cursorPos)) return;
            Vector3 dirVector = pController.GetCursor().transform.position - transform.position;
            transform.forward = new Vector3(dirVector.x, 0, dirVector.z);
        }
    }
}
