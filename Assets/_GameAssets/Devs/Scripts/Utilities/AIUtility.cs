using UnityEngine;

namespace RPG.AI
{
    public static class AIUtility 
    {
        public static bool OnVision(Transform target, Transform myTransform,float visionOpening, float distance,string tagTarget)
        {
            Vector3 dir = (target.position - myTransform.position).normalized;


            if (Physics.Raycast(myTransform.position, dir, out RaycastHit hit))// si hay una pared o algo que no lo deje ver, devuelve false
                if (!hit.transform.CompareTag(tagTarget)) return false;


            if (Vector3.Distance(target.position, myTransform.position) < distance)//Cono de vision.
            {
                float dot = Vector3.Dot(myTransform.forward, dir);
                if (dot > -visionOpening)
                    return true;

            }
            return false;
        }
    }
}
