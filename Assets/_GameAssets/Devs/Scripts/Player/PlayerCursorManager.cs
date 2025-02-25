using UnityEngine;

namespace RPG.Player
{
    public class PlayerCursorManager : MonoBehaviour, IPlayerModule
    {
        [SerializeField] float joyCursorSpeed;

        bool moduleEnabled;
        Vector2 lastMousePos;
        Vector3 lastCursorPos;
        SelectionCursor cursor;
        PlayerController pController;
        PlayerInputListener inputListener;

        public void Init(PlayerController controller)
        {
            pController = controller;
            inputListener = pController.GetInputListener();
            cursor = pController.GetCursor();

            lastCursorPos = transform.position + transform.forward * 5;
            lastCursorPos.y += .1f;
            cursor.transform.position = lastCursorPos;
        }

        public void ToggleModule(bool toggle) => moduleEnabled = toggle;

        void Update()
        {
            if (!moduleEnabled || !pController.GetCursorWorldPos(out Vector3 cursorPos)) return;
            cursorPos.y = transform.position.y + .1f;

            if (inputListener.MousePosition != lastMousePos && (lastCursorPos - cursorPos).magnitude > .85f)
            {
                lastCursorPos = cursorPos; 
                cursor.transform.position = lastCursorPos;
                cursor.ToggleCursorVis(false);
            }

            if (inputListener.JoyLookValue.magnitude > 0)
                cursor.ToggleCursorVis(true);

            Vector3 newPos = cursor.transform.position;
            newPos.x += inputListener.JoyLookValue.x * Time.deltaTime * joyCursorSpeed;
            newPos.z += inputListener.JoyLookValue.y * Time.deltaTime * joyCursorSpeed;
            cursor.transform.position = newPos;

            lastMousePos = inputListener.MousePosition;
        }
    }
}
