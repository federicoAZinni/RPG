using System;
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
        [SerializeField] SelectionCursor playerSelectionCursor;

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
        Vector2 lastMousePosition;
        Vector3 lastCursorWorlPos;

        public Action OnInteraction;

        void Start()
        {
            playerObject = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            stateCamera.LookAt = playerObject.transform;

            lastMousePosition = Vector2.negativeInfinity;
            lastCursorWorlPos = Vector3.negativeInfinity;

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

            if ((InputListener.MousePosition - lastMousePosition).magnitude <= Vector3.kEpsilon)
            {
                worldPos = lastCursorWorlPos;
                return true;
            }

            Ray ray = MainCamera.ScreenPointToRay(InputListener.MousePosition);
            lastMousePosition = InputListener.MousePosition;

            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
            lastCursorWorlPos = worldPos = hit.point;
            return true;
        }

        public void UpdateCursorWorldPos(Vector3 newPos) => lastCursorWorlPos = newPos;

        public void OnPlayerCharacterDies()
        {
            int size = playerModules.Length;
            for (int i = 0; i < size; i++)
                playerModules[i].ToggleModule(false);
        }

        public void OnInteractionPerformed() => OnInteraction?.Invoke();

        public PlayerInputListener GetInputListener() => InputListener;
        public SelectionCursor GetCursor() => playerSelectionCursor;
    }
}
