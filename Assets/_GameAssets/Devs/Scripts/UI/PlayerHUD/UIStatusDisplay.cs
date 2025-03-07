using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPG.UI.HUD
{
    public class UIStatusDisplay : MonoBehaviour
    {
        [SerializeField] Image imgFill;
        [SerializeField] TMP_Text lblDebug;

        float maxStatusValue, targetValue, lerpSpeed;

        void Update()
        {
            if (imgFill.fillAmount - targetValue < Mathf.Epsilon) return;
            imgFill.fillAmount = Mathf.Lerp(imgFill.fillAmount, targetValue, Time.deltaTime * lerpSpeed);
        }

        public void SetMaxStatusValue(float ammount) => targetValue = maxStatusValue = ammount;

        public void UpdateStatus(float status)
        {
            if (maxStatusValue == 0) return;
            targetValue = status / maxStatusValue;
            lerpSpeed = 1 + (targetValue - imgFill.fillAmount);
            lblDebug.text = status.ToString();
        }
    }
}
