using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
namespace RPG.AI
{
    public class IdleAIEnemy_State : IStateEnemyAI
    {
        AIEnemyController aiEnemyController;
        Vector3 startPosRef;
        NavMeshAgent agent;
        Mesh mesh;

        float areaRadius = 5;
        Vector3 posRandom;

        public IdleAIEnemy_State(Vector3 startPosRef, AIEnemyController aiEnemyController,NavMeshAgent agent, Mesh mesh)
        {
            this.startPosRef = startPosRef;
            this.aiEnemyController = aiEnemyController;
            this.agent = agent;
            this.mesh = mesh;
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

        public Color ColorGUI() => Color.white;

        public async Task Action(CancellationToken cancellationToken)
        {
            while (!aiEnemyController.onVisionToAlert ) 
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    cancellationToken.ThrowIfCancellationRequested();
                
                    
                posRandom = GetPosAvailablePosInArea(mesh);
                agent.SetDestination(posRandom);
                await Task.Delay(4000);
            }

            aiEnemyController.ChangeState(State.Alert);

        }

        Vector3 GetPosAvailablePosInArea(Mesh meshPrefab)
        {
            float yHeight = meshPrefab.bounds.size.y / 2;

            for (int i = 0; i < 1500; i++)
            {
                Vector3 randomPosInSphere = (Random.insideUnitSphere * areaRadius) + startPosRef;
                randomPosInSphere.y = 100;
                if (Physics.SphereCast(randomPosInSphere, (meshPrefab.bounds.size.x / 2), -aiEnemyController.transform.up, out RaycastHit hit))
                {
                    if (hit.transform.CompareTag("Ground"))
                        return new Vector3(randomPosInSphere.x, hit.point.y + yHeight, randomPosInSphere.z);
                }
            }

            return Vector3.zero;

        }

    }

}
