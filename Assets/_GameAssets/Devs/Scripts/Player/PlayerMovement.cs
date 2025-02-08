using UnityEngine;

namespace RPG.Player
{
    public class PlayerMovement : MonoBehaviour, IPlayerModule
    {
        Camera _mainCamera;
        Camera MainCamera
        {
            get
            {
                if (_mainCamera == null) _mainCamera = Camera.main;
                return _mainCamera;
            }
        }

        [SerializeField] float baseMovementSpeed;
        [SerializeField] float rotationSpeed;

        PlayerInputListener inputListener;

        public void Init(PlayerController controller)
        {
            inputListener = controller.GetInputListener();
        }

        void Update()
        {
            float currentMovementSpeed = baseMovementSpeed * Time.deltaTime;
            if (inputListener.MoveValue.x != 0 && inputListener.MoveValue.y != 0) currentMovementSpeed *= .5f;

            transform.position += currentMovementSpeed * inputListener.MoveValue.y * Vector3.forward;
            transform.position += currentMovementSpeed * inputListener.MoveValue.x * Vector3.right;

            Ray ray = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue)) return;
            Vector3 dirVector = MainCamera.ScreenToWorldPoint(new Vector3(inputListener.MousePosition.x, inputListener.MousePosition.y, hit.distance)) - transform.position;
            transform.forward = new Vector3(dirVector.x, 0, dirVector.z);
        }
    }
}
