using UnityEngine;

namespace RPG
{
    public class InfiniteRotation : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] Vector3 dir;

        void Update()
        {
            transform.eulerAngles += speed * Time.deltaTime * dir;
        }
    }
}
