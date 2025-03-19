using RPG.AI;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AI;
using System.Threading;
using UnityEngine.UIElements;

namespace RPG.AI
{
    public class ChaseAIEnemy_State : IStateAI
    {
        AIEnemyController aiEnemyController;
        Transform myTransform;
        NavMeshAgent agent;
        
        float visionDistanceToChase;
        float visionOpening;
        float rangeToAttack;

        public ChaseAIEnemy_State(AIEnemyController aiEnemyController, Transform myTransform, NavMeshAgent agent, float visionDistanceToChase, float rangeToAttack, float visionOpening)
        {
            this.aiEnemyController = aiEnemyController;
            this.agent = agent;
            this.visionDistanceToChase = visionDistanceToChase;
            this.visionOpening= visionOpening;
            this.rangeToAttack = rangeToAttack;
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");
            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); };

            await Action(ct);
        }


        public async Task Action(CancellationToken cancellationToken)
        {
            while (aiEnemyController.OnVisionAndRange(OnVisionAndRangeState.ChaseRange))
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    return; /*cancellationToken.ThrowIfCancellationRequested();*/

                agent.SetDestination(aiEnemyController.Target.position);

                if (aiEnemyController.OnVisionAndRange(OnVisionAndRangeState.AttackRange))
                {
                    int probabilityToAttack = 10;

                    if (probabilityToAttack <= Random.Range(0, 100))
                    {
                        aiEnemyController.ChangeState(State.Attack);
                    }
                    else
                    {
                        //Random.onUnitSphere;
                    }
                    return;
                }

                await Task.Delay(500);
            }

            aiEnemyController.ChangeState(State.Alert);
        }

        public Color ColorGUI() =>  Color.magenta;
        

        public void OnFinish()
        {
            
        }

    
    }
}
