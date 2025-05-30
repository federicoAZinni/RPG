using System;
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
        AIEnemyController controller;
        Vector3 startPosRef;
        NavMeshAgent agent;
        Transform myTranform;
        Transform target;
        Mesh mesh;

        //Random Move on area
        float areaRadius = 5;
        float timeToMoveToRandomPos;
        Vector3 posRandom;



        public IdleAIEnemy_State(AIEnemyController controller)
        {
            this.controller = controller;

            agent = controller.Agent;
            myTranform = controller.transform;
            mesh = controller.MeshFilter.sharedMesh;
        }

        public override Color ColorGUI() => Color.cyan;

        public override async Task OnStart()
        {
            startPosRef = controller.transform.position;
            timeToMoveToRandomPos = 0;
            await Action(tokenSource);
        }

        public override void OnFinish()
        {

        }

        protected override async Task Action(CancellationToken cancellationToken)
        {
            float time = 0;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested) return;

                SearchPlayer();

                time = MoveInRandomPosOnArea(time);

                if (controller.OnVisionAndRange(OnVisionAndRangeState.AlertRange))
                    controller.ChangeState(State.Alert);

                await Task.Yield();
            }
        }


        private float MoveInRandomPosOnArea(float time)
        {
            if (timeToMoveToRandomPos < time)//Esto hace que cada random tiempo cree una posicion nueva para que se mueva.
            {
                timeToMoveToRandomPos = UnityEngine.Random.Range(10, 20);
                time = 0;
                posRandom = GetPosAvailablePosInArea(mesh);
                agent.SetDestination(posRandom);
            }

            time += Time.deltaTime;
            return time;
        }

        public bool SearchPlayer()
        {
            Collider[] possibleTargets = Physics.OverlapSphere(myTranform.position, controller.RangeToSearchPlayer, LayerMask.GetMask("Characters"));

            for (int i = 0; i < possibleTargets.Length; i++)
            {
                if (possibleTargets[i].CompareTag("Player"))
                {
                    target = possibleTargets[i].transform;
                    controller.SetTarget(target);
                    return true;
                }
            }
            controller.Target = null;
            return false;
        }

        Vector3 GetPosAvailablePosInArea(Mesh meshPrefab)
        {
            float yHeight = meshPrefab.bounds.size.y / 2;

            for (int i = 0; i < 1500; i++)
            {
                Vector3 randomPosInSphere = (UnityEngine.Random.insideUnitSphere * areaRadius) + startPosRef;
                randomPosInSphere.y = 100;
                if (Physics.SphereCast(randomPosInSphere, (meshPrefab.bounds.size.x / 2), -myTranform.transform.up, out RaycastHit hit))
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



