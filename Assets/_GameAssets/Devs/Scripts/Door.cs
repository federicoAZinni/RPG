using UnityEngine;

namespace RPG
{
    public class Door : MonoBehaviour
    {
        //[SerializedField] DoorMaster doorMaster;
        [SerializeField] Transform doorModel;

        public bool DoorIsOpen { get; private set; }
        public bool DoorIsLocked { get; private set; }

        [Header("DEBUG"), Space(5)]
        [SerializeField] bool startOpen;
        [SerializeField] bool startLocked;
        [SerializeField] Transform openPos, closedPos;
        [SerializeField] float movementSpeed;

        void Start()
        {
            if (startOpen)
            {
                doorModel.position = openPos.position;
                DoorIsOpen = true;
            }

            DoorIsLocked = startLocked;
        }

        void Update()
        {
            if (DoorIsOpen)
            {
                MoveDoor(openPos);
                return;
            }

            MoveDoor(closedPos);
        }

        void MoveDoor(Transform target)
        {
            if (Vector3.Distance(doorModel.position, target.position) < Vector3.kEpsilon) return;
            doorModel.position = Vector3.MoveTowards(doorModel.position, target.position, movementSpeed * Time.deltaTime);
        }

        public void ToggleDoor(bool toggle)
        {
            if (DoorIsLocked) return;
            DoorIsOpen = toggle;
        }

        public void LockDoor(bool toggle) => DoorIsLocked = toggle;
    }
}
