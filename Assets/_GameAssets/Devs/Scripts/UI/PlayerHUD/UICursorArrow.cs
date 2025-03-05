using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.HUD
{
    [RequireComponent(typeof(Image))]
    public class UICursorArrow : MonoBehaviour
    {
        [SerializeField] RectTransform canvasRect;

        RectTransform _rectTransform;
        RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        Camera _mainCamera;
        Camera MainCamera
        {
            get
            {
                if (_mainCamera == null) _mainCamera = Camera.main;
                return _mainCamera;
            }
        }

        Image _imgArrow;
        Image ImgArrow
        {
            get
            {
                if (_imgArrow == null) _imgArrow = GetComponent<Image>();
                return _imgArrow;
            }
        }

        SelectionCursor cursor;

        public void GetCursor(SelectionCursor cursorRef) => cursor = cursorRef;

        void Update()
        {
            if (!cursor.IsVisible) return;

            Vector2 screenPos = MainCamera.WorldToScreenPoint(cursor.transform.position);
            if (screenPos.x <= Screen.width && screenPos.x >= 0 && screenPos.y <= Screen.height && screenPos.y >= 0)
            {
                ImgArrow.enabled = false;
                return;
            }

            ImgArrow.enabled = true;
            screenPos.x = (screenPos.x / Screen.width) * canvasRect.sizeDelta.x;
            screenPos.y = (screenPos.y / Screen.height) * canvasRect.sizeDelta.y;

            RectTransform.anchoredPosition = new Vector2(Mathf.Clamp(screenPos.x, 5, canvasRect.sizeDelta.x - 5), Mathf.Clamp(screenPos.y, 5, canvasRect.sizeDelta.y - 5));
            RectTransform.up = screenPos - RectTransform.anchoredPosition;
        }
    }
}
