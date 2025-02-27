using System.Runtime.InteropServices.WindowsRuntime;
using RPG.AI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace RPG
{
    public class AttackAIEnemy_State : IStateEnemyAI
    {
        private AIEnemyController aiEnemyController;
        private float rangeToAttack;
        private float timeWaitBeforeAttack;

        public AttackAIEnemy_State(AIEnemyController aiEnemyController, float rangeToAttack, float timeWaitBeforeAttack)
        {
            this.aiEnemyController = aiEnemyController;
            this.rangeToAttack = rangeToAttack;
            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); tokenSource.Dispose(); };

            await Action(ct);
        }

        public async Task Action(CancellationToken cancellationToken)
        {
            while (aiEnemyController.onRangeToAttack)
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    cancellationToken.ThrowIfCancellationRequested();

                Debug.Log("Attack");

                await Task.Delay((int)timeWaitBeforeAttack*1000);// Cmabiar esto
            }

            aiEnemyController.ChangeState(State.Chase);
        }

        public Color ColorGUI()
        {
            return Color.red;
        }

        public void OnFinish()
        {
            
        }

        
    }
}
