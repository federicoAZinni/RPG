using RPG.AI;
using UnityEngine;

namespace RPG
{
    public class BowAttack : MonoBehaviour
    {
        [SerializeField] AIEnemyController aiEne;
        [SerializeField] Arrow[] arrows;
        [SerializeField] Transform refPositionArrow;

        private void Awake()
        {
            InitPoolArrow();
        }


        public void Attack()
        {
            if (aiEne.GetCurrentTargetTransform() == null) return;

            Arrow arrowToUse = GetArrow();
            
            Vector3 targetPos = aiEne.GetCurrentTargetTransform().position;

            StartCoroutine(arrowToUse.Trayectoria(arrowToUse.transform.position, targetPos));

        }

        #region PoolArrow
        private void InitPoolArrow()
        {
            foreach (var ar in arrows)
            {
                ar.gameObject.SetActive(false);
                ar.bowAttack = this;
            }
        }

        private Arrow GetArrow()
        {
            foreach (Arrow arrow in arrows)
            {
                if (!arrow.gameObject.activeSelf)
                {
                    arrow.transform.position = refPositionArrow.position;
                    arrow.transform.SetParent(null);
                    arrow.gameObject.SetActive (true);
                    return arrow;
                }
            }

            return null;
        }

        public void SetArrowToPool(Arrow arrow)
        {
            arrow.gameObject.SetActive(false);
            arrow.transform.position = refPositionArrow.position;
            arrow.transform.SetParent(transform);
        }
        #endregion  
    }
}
