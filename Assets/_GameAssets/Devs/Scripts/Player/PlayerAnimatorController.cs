using System.Net.Sockets;
using UnityEngine;

namespace RPG.Player
{
    public class PlayerAnimatorController : MonoBehaviour, IPlayerModule

    {
        [SerializeField] Animator animatorController;
        PlayerInputListener inputListener;
        bool moduleEnabled;

        [Space(10)]
        [Header("Mov Variables")]
        [SerializeField] Vector2 movSpeed;

        public void Init(PlayerController controller)
        {
            inputListener = controller.GetInputListener();
        }

        private void Update()
        {
            movSpeed.x = inputListener.MoveValue.x * Vector3.Dot(Vector3.forward,transform.forward);
            movSpeed.y = inputListener.MoveValue.y * Vector3.Dot(Vector3.forward, transform.forward); 

            animatorController.SetFloat("MovX", movSpeed.x);
            animatorController.SetFloat("MovZ", movSpeed.y);

            animatorController.SetFloat("Speed", movSpeed.normalized.magnitude);
        }

        public void ToggleModule(bool toggle) => moduleEnabled = toggle; 


    }
}
