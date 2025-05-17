using RPG.AI;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;

namespace RPG
{
    public class AttackAIEnemyRange_State : IStateAI,IAttackType
    {
        private AIEnemyController aiEnemyController;
        private float rangeToAttack;
        private float timeWaitBeforeAttack, attackDamage;
        private Animator anim;
        private NavMeshAgent agent;
        private float visionOpening;
        private int probabilityToAttack = 10;
        private IDamageable target;
        private float coolDownAttack;
        private CancellationTokenSource tokenSource;

        public AttackAIEnemyRange_State(AIEnemyController aiEnemyController, NavMeshAgent agent, Animator anim, float rangeToAttack, float timeWaitBeforeAttack, float attackDamage, float visionOpening)
        {
            this.aiEnemyController = aiEnemyController;
            this.rangeToAttack = rangeToAttack;
            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
            this.anim = anim;
            this.attackDamage = attackDamage;
            this.visionOpening = visionOpening;
            this.agent = agent;
        }

        public async void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");

            target = aiEnemyController.GetCurrentTargetTransform().GetComponent<IDamageable>();
            if (target != null || target.HP > 0) target.OnTargetDies += OnTargetDies;

            tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); };

            await Action(ct);
        }

        public async Task Action(CancellationToken cancellationToken)
        {
            coolDownAttack = timeWaitBeforeAttack + 1;
            if (cancellationToken.IsCancellationRequested) return;

            while (AIUtility.OnVision(aiEnemyController.Target, aiEnemyController.transform, visionOpening, rangeToAttack, aiEnemyController.Target.tag))
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    return; /*cancellationToken.ThrowIfCancellationRequested();*/

                aiEnemyController.transform.LookAt(new Vector3(target.GetPosition().x, aiEnemyController.transform.position.y, target.GetPosition().z));


                if (timeWaitBeforeAttack < coolDownAttack)
                {
                    coolDownAttack = 0;
                    anim.SetTrigger("Attack");
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
            tokenSource.Cancel();
        }


    }
}
