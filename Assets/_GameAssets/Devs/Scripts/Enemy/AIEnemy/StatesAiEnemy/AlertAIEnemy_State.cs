using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.AI;

namespace RPG.AI
{
    public class AlertAIEnemy_State : IStateAI
    {
        AIEnemyController aiEnemyController;

        Transform myTransform;
        Vector3 lastPosPlayerWatched;
        NavMeshAgent agent;

        Collider[] possibleTargets;

        float timeWaitBeforeChase;

        public AlertAIEnemy_State(AIEnemyController aiEnemyController, Transform myTransform, NavMeshAgent agent, float timeWaitBeforeChase)
        {
            this.myTransform = myTransform;
            this.aiEnemyController = aiEnemyController;
            this.timeWaitBeforeChase = timeWaitBeforeChase;
            this.agent = agent;
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); };

            lastPosPlayerWatched = aiEnemyController.Target.position;
            
            AlertToOtherEnemiesInRange();

            await Action(ct);
        }

        private void AlertToOtherEnemiesInRange()
        {
            possibleTargets = Physics.OverlapSphere(myTransform.position, 10, LayerMask.GetMask("Characters"));

            for (int i = 0; i < possibleTargets.Length; i++)
            {
                if(possibleTargets[i].TryGetComponent<AIEnemyController>(out AIEnemyController aiEnemy))
                {
                    aiEnemy.SetTarget(aiEnemyController.Target);
                    aiEnemy.ChangeState(State.Alert);
                }
                 
            }
        }

        public void OnFinish()
        {
            Debug.Log($"OnFinish, State : {this.GetType().Name}");
            agent.speed = agent.speed * 3;//Cambiar esto
        }

        public Color ColorGUI() => Color.yellow;

        public async Task Action(CancellationToken cancellationToken)
        {
            float timeOnVision = 0;

            agent.speed = agent.speed / 3;//Cambiar esto
            agent.SetDestination(lastPosPlayerWatched);


            while (timeOnVision < timeWaitBeforeChase)
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    return; /*cancellationToken.ThrowIfCancellationRequested();*/

                timeOnVision += Time.deltaTime;

                if (aiEnemyController.OnVisionAndRange(OnVisionAndRangeState.ChaseRange))// si en el tiempo de espera se acerca lo suficiente lo persigue.
                {
                    aiEnemyController.ChangeState(State.Chase);
                    return;
                }

                await Task.Yield();
            }

           if (aiEnemyController.OnVisionAndRange(OnVisionAndRangeState.AlertRange))//Si cumple con el tiempo de alerta y está en vision cambia a perseguir
            {
                aiEnemyController.ChangeState(State.Chase);
                return;
            }

            ////Si se llega a ir de vision antes de terminar el tiempo, cambia al ultimo estado
            aiEnemyController.ChangeState(State.Idle);
        }


    }
}
