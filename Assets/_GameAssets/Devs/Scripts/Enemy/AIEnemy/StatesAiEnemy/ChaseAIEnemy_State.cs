using RPG.AI;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AI;
using System.Threading;

namespace RPG.AI
{
    public class ChaseAIEnemy_State : IStateAI
    {
        AIEnemyController controller;
        NavMeshAgent agent;

        public ChaseAIEnemy_State(AIEnemyController controller)
        {
            this.controller = controller;

            this.agent = controller.Agent;
        }

        public override Color ColorGUI() => Color.magenta;
        
        public override async Task OnStart()
        {
            await Action(tokenSource);
        }

        public override void OnFinish()
        {
            
        }

        protected override async Task Action(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) return;

                agent.SetDestination(controller.Target.position);

                if (!controller.OnVisionAndRange(OnVisionAndRangeState.ChaseRange)) //Si esta fuera del rango de perseguir
                { await Task.Delay(5000); controller.ChangeState(State.Alert); }

                if (controller.OnVisionAndRange(OnVisionAndRangeState.AttackRange))
                    controller.ChangeState(State.Attack);

                await Task.Yield();
            }
        }
    }
}


