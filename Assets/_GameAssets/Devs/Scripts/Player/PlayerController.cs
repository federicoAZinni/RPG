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
        [SerializeField] PlayerHUD playerHUD;

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

        public System.Action OnInteraction;

        void Start()
        {
            playerObject = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            playerObject.layer = LayerMask.NameToLayer("Characters");
            stateCamera.LookAt = playerObject.transform;
            stateCamera.Follow = playerObject.transform;

            lastMousePosition = Vector2.negativeInfinity;
            lastCursorWorlPos = Vector3.negativeInfinity;

            playerModules = playerObject.GetComponents<IPlayerModule>();
            PlayerHealth healthModule = null;

            int size = playerModules.Length;
            for (int i = 0; i < size; i++) //Inicia todos los compoenentes que sean modulos del player (IPlayerModule)
            {
                if (playerModules[i] is PlayerHealth) healthModule = playerModules[i] as PlayerHealth;
                playerModules[i].Init(this);
                playerModules[i].ToggleModule(true);
            }

            if (healthModule == null) return;
            playerHUD.OnPlayerSpawns(new PlayerHUD.PlayerHUDInitData
            {
                maxPlayerHP = healthModule.MaxHP,
                maxPlayerMana = 100,
                playerHP = healthModule.HP,
                playerMana = 100,
                cursor = playerSelectionCursor
            });
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

        public void OnPlayerHPChange(float currentHP)
        {
            playerHUD.UpdatePlayerHP(currentHP);
        }

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
