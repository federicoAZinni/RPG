using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPG.UI.HUD
{
    public class UIStatusDisplay : MonoBehaviour
    {
        [SerializeField] Image imgFill;
        [SerializeField] TMP_Text lblDebug;

        float maxStatusValue;

        public void SetMaxStatusValue(float ammount) => maxStatusValue = ammount;

        public void UpdateStatus(float status)
        {
            if (maxStatusValue == 0) return;
            imgFill.fillAmount = status / maxStatusValue;
            lblDebug.text = status.ToString();
        }
    }
}
