using UnityEngine;

namespace RPG.Player
{
    public class PlayerHealth : MonoBehaviour, IPlayerModule, IDamageable
    {
        [SerializeField] float maxHP;

        public float HP { get; private set; }
        public float MaxHP => maxHP;

        bool moduleEnabled;
        PlayerController pController;

        public event System.Action OnTargetDies;

        public void Init(PlayerController controller)
        {
            pController = controller;

            HP = maxHP;
        }

        public void ToggleModule(bool toggle) => moduleEnabled = toggle;

        public void Damage(float ammount)
        {
            if (!moduleEnabled) return;
            HP = Mathf.Clamp(HP - ammount, 0, maxHP);
            pController.OnPlayerHPChange(HP);
            Debug.LogFormat("Player loss {0} HP!", ammount);
            if (HP == 0) OnDeath();
        }

        public void GiveHP(float ammount)
        {
            if (!moduleEnabled) return;
            HP = Mathf.Clamp(HP + ammount, 0, maxHP);
            pController.OnPlayerHPChange(HP);
            Debug.LogFormat("Player got {0} HP!", ammount);
        }

        void OnDeath()
        {
            OnTargetDies?.Invoke();
            pController.OnPlayerCharacterDies();
            print("The player died!");
        }

        public Vector3 GetPosition() => transform.position;
    }
}
