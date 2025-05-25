using System.Collections;
using JetBrains.Annotations;
using NUnit;
using UnityEngine;

namespace RPG
{
    public class Arrow : MonoBehaviour
    {
        public BowAttack bowAttack;
        public float dmg;
        [SerializeField] TrailRenderer trilRenderer;
        float speed = 4.0f;

        private  void OnEnable()
        {
            Invoke("BackToPool", 4);
        }


        public IEnumerator Trayectoria(Vector3 posA , Vector3 posB)
        {
            float speedT = speed/Vector3.Distance(posA, posB);
            float t = 0;

            while (t < speedT)
            {
                Vector3 linear = Vector3.Lerp(posA, posB, t/ speedT);
                float parabolicOffset = 4 * 2 * t * (1 - t/ speedT); //hermoso esto que me mande aca

                linear.y += parabolicOffset;

                transform.LookAt(linear);

                transform.position = linear;

                t += Time.deltaTime;
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.Damage(dmg);
                BackToPool();
            }
        }

        

        void BackToPool()
        {
            trilRenderer.Clear();
            bowAttack.SetArrowToPool(this);
        }
    }
}
