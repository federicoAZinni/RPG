using UnityEngine;

namespace RPG.AI
{
    public static class AIUtilities 
    {
        public static bool OnVision(Transform target_T, Transform myTransform, float visionDistance, float visionOpening)
        {
            Vector3 dir = (target_T.position - myTransform.position).normalized;

            if (Vector3.Distance(target_T.position, myTransform.position) < visionDistance)
            {
                float dot = Vector3.Dot(myTransform.right, dir);
                if (dot < visionOpening && dot > -visionOpening)
                    return true;
            }
            return false;
        }
    }
}
