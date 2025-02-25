using UnityEngine;

namespace RPG
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SelectionCursor : MonoBehaviour
    {
        const float normalSize = .64f;

        [SerializeField] Sprite sprCursor, sprSelection;

        SpriteRenderer _sprRenderer;
        SpriteRenderer SprRenderer
        {
            get
            {
                if (_sprRenderer == null) _sprRenderer = GetComponent<SpriteRenderer>();
                return _sprRenderer;
            }
        }

        public void OnSelection(Vector3 objectSize)
        {
            SprRenderer.sprite = sprSelection;
            SprRenderer.size = new Vector2(objectSize.x / normalSize, objectSize.z / normalSize);
        }

        public void FreeCursor()
        {
            SprRenderer.sprite = sprCursor;
            SprRenderer.size = Vector2.one * normalSize;
        }

        public void ToggleCursorVis(bool toggle) => SprRenderer.enabled = toggle;
    }
}
