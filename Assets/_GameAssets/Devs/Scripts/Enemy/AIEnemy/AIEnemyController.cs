using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Builders;
using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

namespace RPG.AI
{
    public class AIEnemyController : MonoBehaviour, IDamageable, IStateMachine
    {

        //Dependencias
        [SerializeField] float rangeToSearchPlayer;
        [SerializeField] Transform target;
        public Transform Target { get => target; set => target = value; }
        [SerializeField] NavMeshAgent agent;
        [SerializeField] SkinnedMeshRenderer meshFilter;
        [SerializeField] Animator anim;
        [SerializeField] TypeAttack typeAttack;

        //All AIState Refes
        IdleAIEnemy_State idleState;
        AlertAIEnemy_State alertState;
        ChaseAIEnemy_State chaseState;
        IAttackType attackState;
        

        //
        IStateAI lastState;
        IStateAI currentState;


        [Space(5)]
        [Header("Vision Refs")]
        [Range(0f, 1f)]
        [SerializeField] float visionOpening = 0.9f;
        [SerializeField] float visionDistanceToAlert;
        [SerializeField] float visionDistanceToChase;
        [SerializeField] float timeWaitBeforeChase;



        [Space(5)]
        [Header("Attack Refs")]
        [SerializeField] float rangeToAttack;
        [SerializeField] float timeWaitBeforeAttack;
        [SerializeField] float attackDamage;


        [Space(5)]
        [Header("Health Refs")]
        [SerializeField] float maxHP;

        public float HP { get; private set; }


        //Events
        public event Action OnTargetDies;
        public static Action OnExitPlayMode;
        [Header("Events")]
        public UnityEvent <float, float> OnTakeDamage;



        public Transform GetCurrentTargetTransform() => Target;
        public Vector3 GetPosition() => transform.position;
        public void SetTarget(Transform _target) => Target = _target;


        #region Init
        private void InitStates() //Inicializamos cada estado con las dependencias que tengan.
                                  //IMPORTANTE!! SI agregamos un estado nuevo, hay que agregarlo en la funcion GetStateByEnum
        {
            idleState = new IdleAIEnemy_State(this, transform, agent, meshFilter.sharedMesh, rangeToSearchPlayer);
            alertState = new AlertAIEnemy_State(this, transform, agent, timeWaitBeforeChase);
            chaseState = new ChaseAIEnemy_State(this, transform, agent);
           
            if(typeAttack == TypeAttack.Melee)
                attackState = new AttackAIEnemyMelee_State(this, agent, anim, timeWaitBeforeAttack, rangeToAttack,attackDamage);
            else
                attackState = new AttackAIEnemyRange_State(this, agent, anim, rangeToAttack, timeWaitBeforeAttack, attackDamage, visionOpening);
        }

        private void Start()
        {
            InitStates();
            ChangeState(State.Idle);
            GiveHP(maxHP);
        }

        #endregion

        #region StateMachine

        public void ChangeState(State _newState) //Cambia el estado y guarda el ultimo en el que estuvo.
        {
            IStateAI newState = GetStateByEnum(_newState); //Se obtiene la instancia del estado que corresponda segun el enum del parametro

            if (currentState == newState) return;

            if(agent.hasPath) agent.ResetPath();

            currentState?.OnFinish(); //Se ejecuta el final del estado

            lastState = currentState;
            currentState = newState;

            currentState.OnStart(); //Se ejecuta el inicio del nuevo estado
        }

        public void ChangeToLastState()
        {
            IStateAI temp = currentState;
            currentState?.OnFinish();
            currentState = lastState;
            lastState = temp;
            currentState.OnStart();
        }


        IStateAI GetStateByEnum(State state)
        {
            switch (state)
            {
                case State.Idle:
                    return idleState;
                case State.Alert:
                    return alertState;
                case State.Chase:
                    return chaseState;
                case State.Attack:
                    return (IStateAI)attackState;
                default:
                    break;
            }
            return null;
        }

        #endregion

        #region OnVision And Range States
        public bool OnVisionAndRange(OnVisionAndRangeState rangeState)
        {
            if (target == null) return false;

            float distance = 0;

            switch (rangeState)
            {
                case OnVisionAndRangeState.AlertRange:
                    distance = visionDistanceToAlert;
                    break;
                case OnVisionAndRangeState.ChaseRange:
                    distance = visionDistanceToChase;
                    break;
                case OnVisionAndRangeState.AttackRange:
                    distance = rangeToAttack;
                    break;
                default:
                    distance = 0;
                    break;
            }


            Vector3 dir = (target.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit))// si hay una pared o algo que no lo deje ver, devuelve false
                if (!hit.transform.CompareTag("Player")) return false;

            if (Vector3.Distance(target.position, transform.position) < distance)//Cono de vision.
            {
                float dot = Vector3.Dot(transform.forward, dir);
                if (dot > -visionOpening)
                    return true;

            }
            return false;
        }
        #endregion

        #region IDamageable

        

        public void Damage(float ammount)
        {
            HP = Mathf.Clamp(HP - ammount, 0, maxHP);
            OnTakeDamage?.Invoke(maxHP , HP);
            if (HP == 0) OnDeath();
        }

        public void OnDeath()
        {
            // TODO Handle Enemy death
            anim.SetTrigger("Die");
            LeanTween.delayedCall(1, () => {
                Destroy(gameObject);
                OnExitPlayMode?.Invoke();
                OnTargetDies?.Invoke();
            });

        }

        public void GiveHP(float ammount) => HP = Mathf.Clamp(HP + ammount, 0, maxHP);

        #endregion


        public void Attack() { if(target != null) target.GetComponent<IDamageable>().Damage(attackDamage); }
        


        private void Update() 
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);//DOTO: Change this.

            if(Input.GetKeyDown(KeyCode.Space)) Damage(1);
        }


        private void OnDrawGizmos() //Crea una esfera y cambia el color dependiendo el estado en que se encuentre.
        {
            if (currentState == null) return;
            Gizmos.color = currentState.ColorGUI();
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), 0.5f);


            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z), rangeToAttack);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z), visionDistanceToAlert);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z), visionDistanceToChase);

            //Gizmos.color = Color.yellow;
            //float angle = UnityEngine.Random.Range(0, 360);
            //Gizmos.DrawWireSphere(target.position-new Vector3(Mathf.Cos(angle) * (rangeToAttack / 2), 0, (Mathf.Sin(angle) * (rangeToAttack / 2))), 0.5f);

        }

        private void OnApplicationQuit()
        {
            OnExitPlayMode?.Invoke();
        }

    }



    public enum TypeAttack
    {
        Melee,
        Range
    }
    public enum State
    {
        Idle,Alert,Chase,Attack
    }
    public enum OnVisionAndRangeState
    {
        AlertRange, ChaseRange, AttackRange
    }

}
