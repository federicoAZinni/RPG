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

        float areaRadius = 5;// Area que va a usar para crear random target asi se mueve cada cierto tiempo.
        float timeToMoveToRandomPos;
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

            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); };

            timeToMoveToRandomPos = 0;

            await Action(ct);

        }

        public void OnFinish()
        {
            Debug.Log($"OnFinish, State : {this.GetType().Name}");
        }

        public Color ColorGUI() => Color.white;

        public async Task Action(CancellationToken cancellationToken)
        {
            float time = 0;

            while (!aiEnemyController.onVisionToAlert ) 
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    cancellationToken.ThrowIfCancellationRequested();
                
                    

                if(timeToMoveToRandomPos < time)//Esto hace que cada random tiempo cree una posicion nueva para que se mueva.
                {
                    timeToMoveToRandomPos = Random.Range(10, 20);
                    time = 0;
                    posRandom = GetPosAvailablePosInArea(mesh);
                    agent.SetDestination(posRandom);
                }

                time += Time.deltaTime;

                await Task.Yield();

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
