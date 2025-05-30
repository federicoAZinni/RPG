using System.Runtime.InteropServices.WindowsRuntime;
using RPG.AI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.AI;
using System.Linq;
using TMPro;

namespace RPG.AI
{
    public class AttackAIEnemyMelee_State : IStateAI,IAttackType
    {
        AIEnemyController controller;
        NavMeshAgent agent;
        Transform myTranform;
        Transform target;
        Animator animator;
        float coolDownAttack;
        float probabilityToAttack = 90;

        public AttackAIEnemyMelee_State(AIEnemyController controller)
        {
            this.controller = controller;

            myTranform = controller.transform;
            agent = controller.Agent;
            animator = controller.Anim;
        }

        public override Color ColorGUI() => Color.red;

        public override async Task OnStart()
        {
            agent.updateRotation = false;
            target = controller.Target;
            await Action(tokenSource);
        }

        public override void OnFinish()
        {
            agent.updateRotation = true;
        }

        protected override async Task Action(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) return;

                if (!controller.OnVisionAndRange(OnVisionAndRangeState.AttackRange)) //Si esta fuera del rango de Atacar
                    controller.ChangeState(State.Chase);

                myTranform.LookAt(new Vector3(target.position.x, myTranform.position.y, target.position.z));

                await Attack(cancellationToken);

                if (!agent.hasPath)
                    SetNewPosToAttackAroundTarget();

                await Task.Yield();
            }
        }

        private async Task Attack(CancellationToken cancellationToken)
        {
            if (controller.TimeWaitBeforeAttack < coolDownAttack)
            {
                if (Random.Range(0, 100) <= probabilityToAttack)
                {
                    agent.ResetPath();
                    agent.isStopped = true;
                    animator.SetTrigger("Attack");

                    float attackDuration = animator.runtimeAnimatorController   //PUEDE TRAER PROBLEMAS 
                    .animationClips
                    .FirstOrDefault(clip => clip.name == "Attack")?.length ?? 1.0f;

                    int delayMs = (int)(attackDuration * 1000);

                    await Task.Delay(delayMs, cancellationToken);

                }
                coolDownAttack = 0;
            }

            coolDownAttack += Time.deltaTime;
        }



        private void SetNewPosToAttackAroundTarget()
        {
            agent.isStopped = false;
            
            Vector3 posA = controller.Target.position;
            Vector3 posB = (myTranform.position - posA).normalized;
            
            float angle = -Mathf.Atan2(posB.x, posB.z) - (Mathf.Deg2Rad * (90 + Random.Range(-90, 90)));
            Vector3 posRandomOnPerimeterPlayer = posA - new Vector3(Mathf.Cos(angle) * (controller.RangeToAttack * 0.8f), 0, Mathf.Sin(angle) * (controller.RangeToAttack * 0.8f));
            
            agent.SetDestination(posRandomOnPerimeterPlayer);
        }
    }
}
































//    public class AttackAIEnemyMelee_State { } : IStateAI, IAttackType
//    {
//        private AIEnemyController aiEnemyController;
//        private float timeWaitBeforeAttack, attackDamage, rangeToAttack;
//        private Animator anim;
//        private NavMeshAgent agent;
//        int probabilityToAttack = 90;
//        private IDamageable target;
//        float coolDownAttack;
//        CancellationToken tokenSource;



//        public AttackAIEnemyMelee_State(AIEnemyController aiEnemyController, NavMeshAgent agent, Animator anim, float timeWaitBeforeAttack,float rangeToAttack, float attackDamage, CancellationToken tokenSource)
//        {
//            this.aiEnemyController = aiEnemyController;
//            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
//            this.rangeToAttack = rangeToAttack;
//            this.anim = anim;
//            this.attackDamage = attackDamage;
//            this.agent = agent;
//            this.tokenSource = tokenSource;
//        }

//        override async Task OnStart()
//        {
//            Debug.Log($"OnStart, State : {this.GetType().Name}");

//            target = aiEnemyController.GetCurrentTargetTransform().GetComponent<IDamageable>();
//            if (target != null || target.HP > 0) target.OnTargetDies += OnTargetDies;



//            //AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); };
//            //aiEnemyController.OnTargetDies += () => { tokenSource.Cancel(); };

//            await Action(tokenSource);
//        }

//        public async override Task Action(CancellationToken cancellationToken)
//        {
//            coolDownAttack = coolDownAttack == 0 ? (timeWaitBeforeAttack + 1): coolDownAttack;

//            if (cancellationToken.IsCancellationRequested) return;

//            while (aiEnemyController.OnVisionAndRange(OnVisionAndRangeState.AttackRange)  )
//            {
//                if (cancellationToken.IsCancellationRequested) return; 

//                if (timeWaitBeforeAttack < coolDownAttack) 
//                {
//                    if (Random.Range(0, 100) <= probabilityToAttack)
//                    {
//                        agent.ResetPath();
//                        agent.isStopped = true;
//                        aiEnemyController.transform.LookAt(new Vector3(target.GetPosition().x, aiEnemyController.transform.position.y, target.GetPosition().z));
//                        anim.SetTrigger("Attack");
//                        await Task.Delay(2000,cancellationToken);
//                    }

//                    coolDownAttack = 0;
//                }
//                anim.StopPlayback();
//                SetNewPosToAttackAroundTarget();

//                coolDownAttack += Time.deltaTime;

//                aiEnemyController.transform.LookAt(new Vector3(target.GetPosition().x, aiEnemyController.transform.position.y, target.GetPosition().z));
//                await Task.Yield();
//            }

//            aiEnemyController.ChangeState(State.Chase);
//        }

//        private void SetNewPosToAttackAroundTarget()
//        {
//            if (!agent.hasPath)
//            {
//                agent.isStopped = false;

//                Vector3 posA = target.GetPosition();
//                Vector3 posB = (aiEnemyController.transform.position - posA).normalized;

//                float angle = -Mathf.Atan2(posB.x, posB.z) - (Mathf.Deg2Rad * (90 + Random.Range(-90,90)));
//                Vector3 posRandomOnPerimeterPlayer = posA - new Vector3(Mathf.Cos(angle) * (rangeToAttack * 0.8f), 0, Mathf.Sin(angle) * (rangeToAttack * 0.8f));


//                agent.SetDestination(posRandomOnPerimeterPlayer);
//            }
//        }

//        public void Attack()
//        {
//            if (target != null) target.Damage(attackDamage);
//        }

//        void OnTargetDies()
//        {
//            // In case the Object isn't destroyed, remove event reference to avoid duplicates
//            target.OnTargetDies -= OnTargetDies;
//            // TODO Stop searching for that target
//        }

//        public Color ColorGUI()
//        {
//            return Color.red;
//        }

//        public void OnFinish()
//        {
//            //tokenSource.Cancel();
//        }


//    }
//}
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