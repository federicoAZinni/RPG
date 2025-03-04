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

        public bool IsLocked { get; private set; }

        Transform objectToFollow;

        void Update()
        {
            if (!IsLocked) return;
            transform.position = objectToFollow.position;
        }

        public void LockCursor(Transform obj, Vector3 objectSize)
        {
            SprRenderer.sprite = sprSelection;
            SprRenderer.size = new Vector2(objectSize.x / normalSize, objectSize.z / normalSize);

            objectToFollow = obj;
            IsLocked = true;
        }

        public void FreeCursor()
        {
            SprRenderer.sprite = sprCursor;
            SprRenderer.size = Vector2.one * normalSize;
            IsLocked = false;
        }

        public void ToggleCursorVis(bool toggle) => SprRenderer.enabled = toggle;

        public bool CheckPointInsideBounds(Vector3 point)
        {
            Bounds sprBounds = new Bounds(objectToFollow.position, new Vector3(SprRenderer.size.x, 1f, SprRenderer.size.y));
            return sprBounds.Contains(point);
        }

        public Vector2 GetSizeOfLockedObject() => SprRenderer.size;
    }
}
