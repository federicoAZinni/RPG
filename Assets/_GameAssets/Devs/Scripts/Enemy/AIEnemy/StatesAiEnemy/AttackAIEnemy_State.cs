using System.Runtime.InteropServices.WindowsRuntime;
using RPG.AI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace RPG.AI
{
    public class AttackAIEnemy_State : IStateEnemyAI
    {
        private AIEnemyController aiEnemyController;
        private float rangeToAttack;
        private float timeWaitBeforeAttack;
        private Animator anim;

        public AttackAIEnemy_State(AIEnemyController aiEnemyController, Animator anim,float rangeToAttack, float timeWaitBeforeAttack)
        {
            this.aiEnemyController = aiEnemyController;
            this.rangeToAttack = rangeToAttack;
            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
            this.anim = anim;
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
            float time = timeWaitBeforeAttack;

            while (aiEnemyController.onRangeToAttack)
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    cancellationToken.ThrowIfCancellationRequested();

                if(timeWaitBeforeAttack < time)
                {
                    time = 0;
                    anim.SetTrigger("Attack");
                    Debug.Log("Attack");
                }

                time += Time.deltaTime;

                await Task.Yield();
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
