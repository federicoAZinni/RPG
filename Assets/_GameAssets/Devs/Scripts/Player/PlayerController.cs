using Unity.Cinemachine;
using UnityEngine;

namespace RPG.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Transform playerSpawnPoint;
        [SerializeField] GameObject playerPrefab;
        [SerializeField] CinemachineStateDrivenCamera stateCamera;

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
            for (int i = 0; i < size; i++) playerModules[i].Init(this);
        }

        public PlayerInputListener GetInputListener() => InputListener;
    }
}
