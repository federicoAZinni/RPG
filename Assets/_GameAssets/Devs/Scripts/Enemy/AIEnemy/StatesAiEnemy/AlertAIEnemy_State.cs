using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace RPG.AI
{
    public class AlertAIEnemy_State : IStateEnemyAI
    {
        AIEnemyController aiEnemyController;

        Transform myTransform;
        Transform player_T;
        Vector3 lastPosPlayerWatched;

        Collider[] possibleTargets;

        float timeWaitBeforeChase;

        public AlertAIEnemy_State(Transform player_T, Transform myTransform, AIEnemyController aiEnemyController, float timeWaitBeforeChase)
        {
            this.player_T = player_T;
            this.myTransform = myTransform;
            this.aiEnemyController = aiEnemyController;
            this.timeWaitBeforeChase = timeWaitBeforeChase;
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); };

            lastPosPlayerWatched = player_T.position;
            
            AlertToOtherEnemiesInRange();

            await Action(ct);
        }

        private void AlertToOtherEnemiesInRange()
        {
            possibleTargets = Physics.OverlapSphere(myTransform.position, 10, LayerMask.GetMask("Characters"));

            for (int i = 0; i < possibleTargets.Length; i++)
            {
                Debug.Log(possibleTargets[i].name);
                possibleTargets[i].GetComponent<AIEnemyController>().ChangeState(State.Alert);
            }
        }

        public void OnFinish()
        {
            Debug.Log($"OnFinish, State : {this.GetType().Name}");
        }

        public Color ColorGUI() => Color.yellow;

        public async Task Action(CancellationToken cancellationToken)
        {
            float timeOnVision = 0;

            while (timeOnVision < timeWaitBeforeChase)
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    cancellationToken.ThrowIfCancellationRequested();

                Vector3 relativePos = player_T.position - myTransform.position;
                Quaternion posWithOutY = Quaternion.LookRotation(relativePos, Vector3.up);
                myTransform.rotation = Quaternion.Lerp(myTransform.rotation, posWithOutY, timeOnVision);

                timeOnVision += Time.deltaTime;

                if (aiEnemyController.onVisionToChase)// si en el tiempo de espera se acerca lo suficiente lo persigue.
                {
                    aiEnemyController.ChangeState(State.Chase);
                    return;
                }

                await Task.Yield();
            }

            if (aiEnemyController.onVisionToAlert)//Si cumple con el tiempo de alerta y está en vision cambia a perseguir
            {
                aiEnemyController.ChangeState(State.Chase);
                return;
            }

            //Si se llega a ir de vision antes de terminar el tiempo, cambia al ultimo estado
            aiEnemyController.ChangeState(State.Idle);

        }

        
    }
}
