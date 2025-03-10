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
        float visionDistanceToAlert;

        //Random Pos
        float areaRadius = 5;
        float timeToMoveToRandomPos;
        Vector3 posRandom;

        public IdleAIEnemy_State(AIEnemyController aiEnemyController, float visionDistanceToAlert, NavMeshAgent agent, Mesh mesh)
        {
            this.aiEnemyController = aiEnemyController;
            this.agent = agent;
            this.mesh = mesh;
            this.visionDistanceToAlert = visionDistanceToAlert;
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            startPosRef = aiEnemyController.GetPosition();

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

            while (!aiEnemyController.OnVision(visionDistanceToAlert))
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    return;//cancellationToken.ThrowIfCancellationRequested();

                SearchPlayer();

                time = MoveInRandomPosOnArea(time);

                await Task.Yield();

            }

            aiEnemyController.ChangeState(State.Alert);

        }

        private float MoveInRandomPosOnArea(float time)
        {
            if (timeToMoveToRandomPos < time)//Esto hace que cada random tiempo cree una posicion nueva para que se mueva.
            {
                timeToMoveToRandomPos = Random.Range(10, 20);
                time = 0;
                posRandom = GetPosAvailablePosInArea(mesh);
                agent.SetDestination(posRandom);
            }

            time += Time.deltaTime;
            return time;
        }

        public void SearchPlayer()
        {
            Collider[] possibleTargets = Physics.OverlapSphere(aiEnemyController.GetPosition(), 20, LayerMask.GetMask("Characters"));

            for (int i = 0; i < possibleTargets.Length; i++)
            {
                if (possibleTargets[i].CompareTag("Player"))
                {
                    aiEnemyController.Player_T = possibleTargets[i].transform;
                }
            }
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
                    else Debug.LogError("Hit on object without tag Ground");
                }
            }

            return Vector3.zero;

        }

    }

}
