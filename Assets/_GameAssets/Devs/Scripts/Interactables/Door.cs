using UnityEngine;

namespace RPG
{
    public class Door : MonoBehaviour
    {
        public void OpenDoorAnim()
        {
            LeanTween.moveLocalY(gameObject, 5, 1).setEaseInCubic();
        }
        public void CloseDoorAnim()
        {
            LeanTween.moveLocalY(gameObject, 0, 1).setEaseInCubic();
        }
    }
}
