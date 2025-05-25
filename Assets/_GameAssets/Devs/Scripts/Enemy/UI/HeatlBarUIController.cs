using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class HeatlBarUIController : MonoBehaviour
    {
        [SerializeField] Canvas c;
        [SerializeField] Image healthBar;

        public void AnimHealthBarDecre(float hpMax , float hp)
        {
            LeanTween.rotateAround(c.gameObject, Vector3.forward, 20, 0.1f).setEaseShake().setRepeat(5);

            float temp = hp / hpMax;
            LeanTween.value(gameObject, healthBar.rectTransform.localScale.x, temp, 0.5f).setOnUpdate((value) => { healthBar.rectTransform.localScale = new Vector3(value, 1, 1); });
        }

    }
}
