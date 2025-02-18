using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace RPG.AI
{
    public class AlertAIEnemy_State : IStateEnemyAI
    {
        AIEnemyController aiEnemyController;

        Transform myTransform;
        Transform player_T;

       
        float timeWaitBeforeAttack;

        public AlertAIEnemy_State(Transform player_T, Transform myTransform, AIEnemyController aiEnemyController, float timeWaitBeforeAttack)
        {
            this.player_T = player_T;
            this.myTransform = myTransform;
            this.aiEnemyController = aiEnemyController;
            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); tokenSource.Dispose(); };

           await Action(ct);
        }

        public void OnFinish()
        {
            Debug.Log($"OnFinish, State : {this.GetType().Name}");
        }

        public Color ColorGUI() => Color.yellow;

        public async Task Action(CancellationToken cancellationToken)
        {
            float timeOnVision = 0;

            while (timeOnVision < timeWaitBeforeAttack) 
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    cancellationToken.ThrowIfCancellationRequested();


                timeOnVision += Time.deltaTime;

                if (aiEnemyController.onVisionToChase)// si en el tiempo de espera se acerca lo suficiente lo persigue.
                {
                    aiEnemyController.ChangeState(State.Chase);
                    return;
                }

                await Task.Yield();
            }

            if (aiEnemyController.onVisionToAlert)//Si cumple con el tiempo de alerta y está en vision cambia a perseguir
            {
                aiEnemyController.ChangeState(State.Chase);
                return;
            }

            //Si se llega a ir de vision antes de terminar el tiempo, cambia al ultimo estado
            aiEnemyController.ChangeState(State.Idle);

        }

        
    }
}
