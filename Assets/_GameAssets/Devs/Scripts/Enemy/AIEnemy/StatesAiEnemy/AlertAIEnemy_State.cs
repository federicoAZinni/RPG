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

        float visionOpening;
        float visionDistance;
        float timeWaitBeforeAttack;

        public AlertAIEnemy_State(Transform player_T, Transform myTransform, AIEnemyController aiEnemyController,float visionOpening,float visionDistance,float timeWaitBeforeAttack)
        {
            this.player_T = player_T;
            this.myTransform = myTransform;
            this.aiEnemyController = aiEnemyController;
            this.visionOpening = visionOpening;
            this.visionDistance = visionDistance;
            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
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

            while (aiEnemyController.onVision) //Siempre y cuando el player este en vision va a contar
            {
                if (timeOnVision > timeWaitBeforeAttack)
                { 
                    aiEnemyController.ChangeState(State.Chase);//Si cumple con el tiempo de alerta, cambia a atacar
                    return;
                }

                timeOnVision += Time.deltaTime;
                await Task.Yield();
            }
            //Si se llega air de vision antes de terminar el tiempo, cambia al ultimo estado
            aiEnemyController.ChangeToLastState();

        }

        
    }
}
