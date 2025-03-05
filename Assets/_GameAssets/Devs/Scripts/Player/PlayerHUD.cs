using UnityEngine;
using RPG.UI.HUD;

namespace RPG.Player
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PlayerHUD : MonoBehaviour
    {
        public struct PlayerHUDInitData
        {
            public float maxPlayerHP, maxPlayerMana;
            public float playerHP, playerMana;
            public SelectionCursor cursor;
        }

        [SerializeField] UIStatusDisplay hpDisplay, manaDisplay;
        [SerializeField] UICursorArrow cursorArrow;

        CanvasGroup mainCanvasGroup;

        void Awake()
        {
            mainCanvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnPlayerSpawns(PlayerHUDInitData data)
        {
            hpDisplay.SetMaxStatusValue(data.maxPlayerHP);
            hpDisplay.UpdateStatus(data.playerHP);

            manaDisplay.SetMaxStatusValue(data.maxPlayerMana);
            manaDisplay.UpdateStatus(data.playerMana);

            cursorArrow.GetCursor(data.cursor);

        }

        public void ToggleHUD(bool toggle) => mainCanvasGroup.alpha = toggle ? 1 : 0;

        public void UpdatePlayerHP(float ammount) => hpDisplay.UpdateStatus(ammount);
        public void UpdatePlayerMana(float ammount) => manaDisplay.UpdateStatus(ammount);
    }
}
