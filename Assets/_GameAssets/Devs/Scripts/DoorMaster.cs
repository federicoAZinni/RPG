using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "DoorMaster_Action", menuName = "Scriptables/DoorMaster_Action")]
    public class DoorMaster_Action : ScriptableObject
    {

    }

    public class DoorMaster : MonoBehaviour
    {
        [SerializeField] Door[] doorSlaves;
        [SerializeField] GenericTrigger[] linkedTriggers;

    }
}
