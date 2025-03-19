using System.Runtime.InteropServices.WindowsRuntime;
using RPG.AI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace RPG.AI
{
    public class AttackAIEnemy_State : IStateAI
    {
        private AIEnemyController aiEnemyController;
        private float rangeToAttack;
        private float timeWaitBeforeAttack, attackDamage;
        private Animator anim;
        float visionOpening;

        private IDamageable target;
        float coolDownAttack;

        public AttackAIEnemy_State(AIEnemyController aiEnemyController, Animator anim,float rangeToAttack, float timeWaitBeforeAttack, float attackDamage, float visionOpening)
        {
            this.aiEnemyController = aiEnemyController;
            this.rangeToAttack = rangeToAttack;
            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
            this.anim = anim;
            this.attackDamage = attackDamage;
            this.visionOpening = visionOpening;
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
            if(coolDownAttack==0) coolDownAttack = timeWaitBeforeAttack;

            while (AIUtility.OnVision(aiEnemyController.Target, aiEnemyController.transform, visionOpening, rangeToAttack, aiEnemyController.Target.tag))
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    return; /*cancellationToken.ThrowIfCancellationRequested();*/

                if(timeWaitBeforeAttack < coolDownAttack)
                {
                    coolDownAttack = 0;
                    anim.SetTrigger("Attack");
                    if (target != null) target.Damage(attackDamage);
                    Debug.Log("Attack");
                }

                coolDownAttack += Time.deltaTime;

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
