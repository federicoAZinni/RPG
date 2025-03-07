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
        private float timeWaitBeforeAttack, attackDamage;
        private Animator anim;

        private IDamageable target;

        public AttackAIEnemy_State(AIEnemyController aiEnemyController, Animator anim,float rangeToAttack, float timeWaitBeforeAttack, float attackDamage)
        {
            this.aiEnemyController = aiEnemyController;
            this.rangeToAttack = rangeToAttack;
            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
            this.anim = anim;
            this.attackDamage = attackDamage;
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");

            target = aiEnemyController.GetCurrentTargetTransform().GetComponent<IDamageable>();
            if (target != null || target.HP > 0) target.OnTargetDies += OnTargetDies;

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
                    if (target != null) target.Damage(attackDamage);
                    Debug.Log("Attack");
                }

                time += Time.deltaTime;

                await Task.Yield();
            }

            aiEnemyController.ChangeState(State.Chase);
        }

        void OnTargetDies()
        {
            // In case the Object isn't destroyed, remove event reference to avoid duplicates
            target.OnTargetDies -= OnTargetDies;
            // TODO Stop searching for that target
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
