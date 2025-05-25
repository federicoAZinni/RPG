using System.Runtime.InteropServices.WindowsRuntime;
using RPG.AI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.AI;

namespace RPG.AI
{
    public class AttackAIEnemyMelee_State : IStateAI, IAttackType
    {
        private AIEnemyController aiEnemyController;
        private float timeWaitBeforeAttack, attackDamage, rangeToAttack;
        private Animator anim;
        private NavMeshAgent agent;
        int probabilityToAttack = 25;
        private IDamageable target;
        float coolDownAttack;
        CancellationTokenSource tokenSource;

        public AttackAIEnemyMelee_State(AIEnemyController aiEnemyController, NavMeshAgent agent, Animator anim, float timeWaitBeforeAttack,float rangeToAttack, float attackDamage)
        {
            this.aiEnemyController = aiEnemyController;
            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
            this.rangeToAttack = rangeToAttack;
            this.anim = anim;
            this.attackDamage = attackDamage;
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
            coolDownAttack = coolDownAttack == 0 ? (timeWaitBeforeAttack + 1): coolDownAttack;

            if (cancellationToken.IsCancellationRequested) return;

            while (aiEnemyController.OnVisionAndRange(OnVisionAndRangeState.AttackRange))
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
                    }

                    SetNewPosToAttackAroundTarget();

                    coolDownAttack = 0;
                }

                coolDownAttack += Time.deltaTime;

                await Task.Yield();
            }

            aiEnemyController.ChangeState(State.Chase);
        }

        private void SetNewPosToAttackAroundTarget()
        {
            if (!agent.hasPath)
            {
                agent.isStopped = false;
                float angle = Random.Range(0, 360);
                Vector3 posRandomOnPerimeterPlayer = target.GetPosition() - new Vector3(Mathf.Cos(angle) * (rangeToAttack * 0.8f), 0, (Mathf.Sin(angle) * (rangeToAttack * 0.8f)));

                agent.SetDestination(posRandomOnPerimeterPlayer);
            }
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
//if (timeWaitBeforeAttack < coolDownAttack)
//{
//    if (Random.Range(0, 100) <= probabilityToAttack)
//    {
//        agent.isStopped = true;
//        anim.SetTrigger("Attack");
//        agent.ResetPath();
//    }

//    coolDownAttack = 0;

//    await Task.Yield();
//}

//if (!agent.hasPath)
//{
//    await Task.Delay(2000);
//    agent.isStopped = false;
//    float angle = Random.Range(0, 360);
//    Vector3 posRandomOnPerimeterPlayer = target.GetPosition() - new Vector3(Mathf.Cos(angle) * (rangeToAttack *0.8f), 0, (Mathf.Sin(angle) * (rangeToAttack * 0.8f)));

//    agent.SetDestination(posRandomOnPerimeterPlayer);
//}

//coolDownAttack += Time.deltaTime;


//aiEnemyController.transform.LookAt(new Vector3(target.GetPosition().x, aiEnemyController.transform.position.y, target.GetPosition().z));

//if (Random.Range(0, 100) <= probabilityToAttack)
//{
//    agent.isStopped = true;
//    anim.SetTrigger("Attack");
//    //agent.ResetPath();
//}
//await Task.Delay((int)timeWaitBeforeAttack * 1000);
////await Task.Yield();