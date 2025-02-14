using Unity.Cinemachine;
using UnityEngine;

namespace RPG.Player
{
    public enum PowerTypes { Power1, Power2, Power3, Power4 }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Transform playerSpawnPoint;
        [SerializeField] GameObject playerPrefab;
        [SerializeField] CinemachineStateDrivenCamera stateCamera;

        Camera _mainCamera;
        public Camera MainCamera
        {
            get
            {
                if (_mainCamera == null) _mainCamera = Camera.main;
                return _mainCamera;
            }
        }

        PlayerInputListener _inputListener;
        PlayerInputListener InputListener
        {
            get 
            {
                if (_inputListener == null) _inputListener = GetComponent<PlayerInputListener>();
                return _inputListener;
            }
        }

        GameObject playerObject;
        IPlayerModule[] playerModules;

        void Start()
        {
            playerObject = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            stateCamera.LookAt = playerObject.transform;

            playerModules = playerObject.GetComponents<IPlayerModule>();
            
            int size = playerModules.Length;
            for (int i = 0; i < size; i++)
            {
                playerModules[i].Init(this);
                playerModules[i].ToggleModule(true);
            }
        }

        public bool GetCursorWorldPos(out Vector3 worldPos)
        {
            worldPos = Vector3.positiveInfinity;
            Ray ray = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue)) return false;

            worldPos = MainCamera.ScreenToWorldPoint(new Vector3(InputListener.MousePosition.x, InputListener.MousePosition.y, hit.distance));
            return true;
        }

        public void OnPlayerCharacterDies()
        {
            int size = playerModules.Length;
            for (int i = 0; i < size; i++)
                playerModules[i].ToggleModule(false);
        }

        public PlayerInputListener GetInputListener() => InputListener;
    }
}
