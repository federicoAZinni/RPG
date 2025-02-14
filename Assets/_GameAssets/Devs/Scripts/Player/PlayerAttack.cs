using UnityEngine;

namespace RPG.Player
{
    public class PlayerAttack : MonoBehaviour, IPlayerModule
    {
        [SerializeField] float bonkDistance, bonkDamage, bonkCooldown;

        bool moduleEnabled;
        float nextBonkTime;
        PlayerController pController;
        PlayerInputListener inputListener;
        IDamageable currentTarget;

        Collider[] possibleTargets;

        void Awake()
        {
            possibleTargets = new Collider[5];
        }

        public void Init(PlayerController controller)
        {
            pController = controller;
            inputListener = pController.GetInputListener();
            inputListener.OnAttack += OnAttack;
            inputListener.OnPower += ThrowPower;
        }

        public void ToggleModule(bool toggle) => moduleEnabled = toggle;


        void OnDestroy()
        {
            inputListener.OnAttack -= OnAttack;
            inputListener.OnPower -= ThrowPower;
        }

        void OnAttack()
        {
            if (!moduleEnabled || Time.time < nextBonkTime) return;

            if (currentTarget == null)
            {
                if (!SelectTarget()) 
                    return;
            }

            if (Vector3.Distance(transform.position, currentTarget.GetPosition()) > bonkDistance) return;
            currentTarget.Damage(bonkDamage);
            nextBonkTime = Time.time + bonkCooldown;
        }

        bool SelectTarget()
        {
            if (!pController.GetCursorWorldPos(out Vector3 mousePos)) return false;

            int quantity = Physics.OverlapSphereNonAlloc(mousePos, .5f, possibleTargets, LayerMask.GetMask("Characters"));
            for (int i = 0; i < quantity; i++)
            {
                IDamageable possibleTarget = possibleTargets[i].GetComponent<IDamageable>();
                if (possibleTarget == null || possibleTarget.HP <= 0) continue;
                currentTarget = possibleTarget;
                return true;
            }

            return false;
        }

        void ThrowPower(PowerTypes type)
        {
            Debug.LogFormat("Throwing power {0}", type);
        }
    }
}
