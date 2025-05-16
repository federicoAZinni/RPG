using System.Runtime.InteropServices.WindowsRuntime;
using RPG.AI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.AI;

namespace RPG.AI
{
    public class AttackAIEnemy_State : IStateAI
    {
        private AIEnemyController aiEnemyController;
        private float rangeToAttack;
        private float timeWaitBeforeAttack, attackDamage;
        private Animator anim;
        private NavMeshAgent agent;
        float visionOpening;
        int probabilityToAttack = 10;
        private IDamageable target;
        float coolDownAttack;
        CancellationTokenSource tokenSource;

        public AttackAIEnemy_State(AIEnemyController aiEnemyController, NavMeshAgent agent, Animator anim,float rangeToAttack, float timeWaitBeforeAttack, float attackDamage, float visionOpening)
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
            coolDownAttack = timeWaitBeforeAttack+1;
            while (AIUtility.OnVision(aiEnemyController.Target, aiEnemyController.transform, visionOpening, rangeToAttack, aiEnemyController.Target.tag))
            {
                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
                    return; /*cancellationToken.ThrowIfCancellationRequested();*/

                aiEnemyController.transform.LookAt(new Vector3(target.GetPosition().x, aiEnemyController.transform.position.y, target.GetPosition().z));
               

                if (timeWaitBeforeAttack < coolDownAttack)
                {
                    if (Random.Range(0, 100) <= probabilityToAttack)
                    {
                        agent.isStopped = true;
                        anim.SetTrigger("Attack");
                        agent.ResetPath();
                        await Task.Delay(1000);
                    }

                    coolDownAttack = 0;

                    await Task.Yield();
                }

                if (!agent.hasPath)
                {
                    agent.isStopped = false;
                    float angle = Random.Range(0, 360);
                    Vector3 posRandomOnPerimeterPlayer = target.GetPosition() - new Vector3(Mathf.Cos(angle) * (rangeToAttack *0.8f), 0, (Mathf.Sin(angle) * (rangeToAttack * 0.8f)));
                    
                    agent.SetDestination(posRandomOnPerimeterPlayer);
                }


                coolDownAttack += Time.deltaTime;

                await Task.Yield();
            }

            aiEnemyController.ChangeState(State.Chase);
        }

        public void Attack()
        {
            if (target != null) target.Damage(attackDamage);
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
