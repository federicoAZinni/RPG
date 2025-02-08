using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System;

namespace RPG.AI
{
    public class AlertAIEnemy_State : IStateEnemyAI
    {
        AIEnemyController aiEnemyController;
        Transform myTransform;
        Transform player_T;

        float visionOpening = 0.9f;
        float visionDistance = 5;

        float timeWaitBeforeAttack = 2;
        public AlertAIEnemy_State(Transform player_T, Transform myTransform, AIEnemyController aiEnemyController)
        {
            this.player_T = player_T;
            this.myTransform = myTransform;
            this.aiEnemyController = aiEnemyController;
        }

        public void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");
            Action();
        }

        public void OnFinish()
        {
            Debug.Log($"OnFinish, State : {this.GetType().Name}");
        }

        public Color ColorGUI() => Color.yellow;

        public async void Action()
        {
            float timeOnVision = 0;

            while (true)
            {
                if(OnVision())
                    timeOnVision += Time.deltaTime;

                if (timeOnVision > timeWaitBeforeAttack) break;

                await Task.Yield();
            }
        }

        bool OnVision()
        {
            Vector3 dir = (player_T.position - myTransform.position).normalized;

            if (Vector3.Distance(player_T.position, myTransform.position) < visionDistance)
            {
                float dot = Vector3.Dot(myTransform.right, dir);
                    if (dot < visionOpening && dot > -visionOpening)
                        return true;
            }
            return false;
        }
    }
}
