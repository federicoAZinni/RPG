using RPG.AI;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AI;
using System.Threading;

namespace RPG
{
    public class ChaseAIEnemy_State : IStateEnemyAI
    {
        Transform target_T;
        NavMeshAgent agent;
        AIEnemyController aiEnemyController;

        public ChaseAIEnemy_State(Transform target_T, NavMeshAgent agent, AIEnemyController aiEnemyController)
        {
            this.target_T = target_T;
            this.agent = agent;
            this.aiEnemyController = aiEnemyController;

        }

        public async Task Action(CancellationToken cancellationToken)
        {
            while (aiEnemyController.onVisionToAlert) 
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    cancellationToken.ThrowIfCancellationRequested();

                agent.SetDestination(target_T.position);
                await Task.Delay(1000);
            }

            aiEnemyController.ChangeState(State.Alert);
        }

        public Color ColorGUI() =>  Color.magenta;
        

        public void OnFinish()
        {
            
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");
            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); tokenSource.Dispose(); };

            await Action(ct);
        }
    }
}
