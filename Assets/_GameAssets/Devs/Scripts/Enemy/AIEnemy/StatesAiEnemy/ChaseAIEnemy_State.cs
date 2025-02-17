using RPG.AI;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AI;

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

        public async void Action()
        {
            while (aiEnemyController.onVision) 
            {
                agent.SetDestination(target_T.position);
                await Task.Delay(1000);
            }

            aiEnemyController.ChangeState(State.Alert);
        }

        public Color ColorGUI() =>  Color.magenta;
        

        public void OnFinish()
        {
            
        }

        public void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");
            Action();
        }
    }
}
