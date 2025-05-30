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
        AIEnemyController controller;
        Transform myTransform;
        
        NavMeshAgent agent; 
        Vector3 lastPosPlayerWatched;
        Collider[] possibleTargets;


        public AlertAIEnemy_State(AIEnemyController controller)
        {
            this.controller = controller;

            this.myTransform = controller.transform;
            this.agent = controller.Agent;
        }

        public override Color ColorGUI() => Color.yellow;

        public override async Task OnStart()
        {
            lastPosPlayerWatched = controller.Target.position;
            await Action(tokenSource);
        }

        public override void OnFinish()
        {
            agent.speed = agent.speed * 3;
        }
        protected override async Task Action(CancellationToken cancellationToken)
        {
            float timeOnVision = 0;

            agent.speed = agent.speed / 3;//Cambiar esto; 
            agent.SetDestination(lastPosPlayerWatched);// Camina hasta el punto donde se vio

            while (true)
            {
                if (cancellationToken.IsCancellationRequested) return;

                timeOnVision += Time.deltaTime;

                if(controller.OnVisionAndRange(OnVisionAndRangeState.ChaseRange)) //Si esta dentro del rango de perseguir
                    controller.ChangeState(State.Chase);

                if(!controller.OnVisionAndRange(OnVisionAndRangeState.AlertRange))//Si esta fuera del rango de Alerta
                    controller.ChangeState(State.Idle);

                if (timeOnVision > controller.TimeWaitBeforeChase)
                    controller.ChangeState(State.Idle);

                await Task.Yield();
            }

        }

        private void AlertToOtherEnemiesInRange()
        {
            possibleTargets = Physics.OverlapSphere(myTransform.position, 10, LayerMask.GetMask("Characters"));

            for (int i = 0; i < possibleTargets.Length; i++)
            {
                if (possibleTargets[i].TryGetComponent<AIEnemyController>(out AIEnemyController aiEnemy))
                {
                    aiEnemy.SetTarget(controller.Target);
                    aiEnemy.ChangeState(State.Alert);
                }

            }
        }
    }
}







