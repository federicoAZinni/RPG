using UnityEngine;

namespace RPG
{
    public interface IDamageable
    {
        public float HP { get; }

        public void Damage(float ammount);
        public void GiveHP(float ammount);
        public Vector3 GetPosition();
    }
}
