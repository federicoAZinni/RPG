using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.AI
{
    public class IdleAIEnemy_State : IStateAI
    {
        AIEnemyController aiEnemyController;
        Vector3 startPosRef;
        NavMeshAgent agent;
        Mesh mesh;
        Transform myTranform , target;
        float rangeToSearchPlayer;


        //Random Pos
        float areaRadius = 5;
        float timeToMoveToRandomPos;
        Vector3 posRandom;

        CancellationTokenSource tokenSource;
        public IdleAIEnemy_State(IStateMachine aiEnemyController, Transform myTranform,NavMeshAgent agent, Mesh mesh, float rangeToSearchPlayer)
        {
            this.aiEnemyController = (AIEnemyController)aiEnemyController;
            this.agent = agent;
            this.mesh = mesh;
            this.myTranform = myTranform;
            this.rangeToSearchPlayer = rangeToSearchPlayer;
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");

            tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            startPosRef = myTranform.position;

            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); };

            timeToMoveToRandomPos = 0;

            await Action(ct);

        }

        public void OnFinish()
        {
            Debug.Log($"OnFinish, State : {this.GetType().Name}");
            tokenSource.Cancel();
        }

        public Color ColorGUI() => Color.white;

        public async Task Action(CancellationToken cancellationToken)
        {
            float time = 0;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                     return;

                if(SearchPlayer() && aiEnemyController.OnVisionAndRange(OnVisionAndRangeState.AlertRange))
                {
                    aiEnemyController.ChangeState(State.Alert);
                    return;
                }else
                    time = MoveInRandomPosOnArea(time);

                await Task.Yield();
            }


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

        public bool SearchPlayer()
        {
            Collider[] possibleTargets = Physics.OverlapSphere(myTranform.position, rangeToSearchPlayer, LayerMask.GetMask("Characters"));

            for (int i = 0; i < possibleTargets.Length; i++)
            {
                if (possibleTargets[i].CompareTag("Player"))
                {
                    target = possibleTargets[i].transform;
                    aiEnemyController.SetTarget(target);
                    return true;
                }
            }
            aiEnemyController.Target = null;
            return false;
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
            Debug.LogError("Hit on object without tag Ground");
            return Vector3.zero;

        }

    }

}
