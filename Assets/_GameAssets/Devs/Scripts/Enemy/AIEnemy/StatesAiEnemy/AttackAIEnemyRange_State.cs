using RPG.AI;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System.Linq;

namespace RPG
{
    public class AttackAIEnemyRange_State : IStateAI
    {
        AIEnemyController controller;
        NavMeshAgent agent;
        Transform myTranform;
        Transform target;
        Animator animator;
        float coolDownAttack;
        float probabilityToAttack = 100;

        public AttackAIEnemyRange_State(AIEnemyController controller)
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

                    //float attackDuration = animator.runtimeAnimatorController   //PUEDE TRAER PROBLEMAS - Los trajo.
                    //.animationClips
                    //.FirstOrDefault(clip => clip.name == "Attack")?.length ?? 1.0f;

                    int delayMs = (int)3000;

                    await Task.Delay(delayMs, cancellationToken);

                }
                coolDownAttack = 0;
            }

            coolDownAttack += Time.deltaTime;
        }

    } 
}



























//: IStateAI,IAttackType
//    {
//        private AIEnemyController aiEnemyController;
//        private float rangeToAttack;
//        private float timeWaitBeforeAttack, attackDamage;
//        private Animator anim;
//        private NavMeshAgent agent;
//        private float visionOpening;
//        //private int probabilityToAttack = 10;
//        private IDamageable target;
//        private float coolDownAttack;
//        private CancellationTokenSource tokenSource;

//        public AttackAIEnemyRange_State(AIEnemyController aiEnemyController, NavMeshAgent agent, Animator anim, float rangeToAttack, float timeWaitBeforeAttack, float attackDamage, float visionOpening)
//        {
//            this.aiEnemyController = aiEnemyController;
//            this.rangeToAttack = rangeToAttack;
//            this.timeWaitBeforeAttack = timeWaitBeforeAttack;
//            this.anim = anim;
//            this.attackDamage = attackDamage;
//            this.visionOpening = visionOpening;
//            this.agent = agent;
//        }

//        public async void OnStart()
//        {
//            Debug.Log($"OnStart, State : {this.GetType().Name}");

//            target = aiEnemyController.GetCurrentTargetTransform().GetComponent<IDamageable>();
//            if (target != null || target.HP > 0) target.OnTargetDies += OnTargetDies;

//            tokenSource = new CancellationTokenSource();
//            CancellationToken ct = tokenSource.Token;

//            AIEnemyController.OnExitPlayMode += () => { tokenSource.Cancel(); };
//            aiEnemyController.OnTargetDies += () => { tokenSource.Cancel(); };

//            await Action(ct);
//        }

//        public async Task Action(CancellationToken cancellationToken)
//        {
//            if (coolDownAttack == 0) coolDownAttack = timeWaitBeforeAttack + 1;

//            if (cancellationToken.IsCancellationRequested) return;

//            while (aiEnemyController.OnVisionAndRange(OnVisionAndRangeState.AttackRange))
//            {
//                if (cancellationToken.IsCancellationRequested) //Necesario para frenar la Task despues de salir del playmode, sin esto, sigue corriendo hasta darle play devuelta
//                    return; /*cancellationToken.ThrowIfCancellationRequested();*/

//                aiEnemyController.transform.LookAt(new Vector3(target.GetPosition().x, aiEnemyController.transform.position.y, target.GetPosition().z));


//                if (timeWaitBeforeAttack < coolDownAttack)
//                {
//                    coolDownAttack = 0;
//                    anim.SetTrigger("Attack");
//                }

//                coolDownAttack += Time.deltaTime;

//                await Task.Yield();
//            }

//            aiEnemyController.ChangeState(State.Chase);
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
//            tokenSource.Cancel();
//        }


//    }
//}
