using UnityEngine;

namespace RPG
{
    public class CreateFloorTest : MonoBehaviour
    {
        public GameObject floor;
        public Vector3 floorOffset;


        public Transform target;

        float xtemp;
        float ztemp;

        [ContextMenu("Floor")]
        public void CreateFloor()
        {

            //xtemp = floorOffset.x;
            //ztemp = floorOffset.z;

            //for (int j = 0; j < 25; j++)
            //{
            //    for (int i = 0; i < 25; i++)
            //    {
            //        Instantiate(floor, floorOffset, Quaternion.identity).transform.SetParent(transform);
            //        floorOffset.x += 4;
            //    }
            //    floorOffset.z += 4;
            //    floorOffset.x = xtemp;
            //}

          

        }
    }
}
