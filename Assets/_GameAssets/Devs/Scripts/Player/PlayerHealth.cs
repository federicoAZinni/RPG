using UnityEngine;

namespace RPG.Player
{
    public class PlayerHealth : MonoBehaviour, IPlayerModule, IDamageable
    {
        [SerializeField] float maxHP;

        public float HP { get; private set; }

        bool moduleEnabled;
        PlayerController pController;

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
            if (HP == 0) OnDeath();
        }

        public void GiveHP(float ammount)
        {
            if (!moduleEnabled) return;
            HP = Mathf.Clamp(HP + ammount, 0, maxHP);
        }

        void OnDeath()
        {
            pController.OnPlayerCharacterDies();
            print("The player died!");
        }

        public Vector3 GetPosition() => transform.position;
    }
}
