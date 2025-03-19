using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Builders;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace RPG.AI
{
    public class AIEnemyController : MonoBehaviour, IDamageable, IStateMachine
    {

        //Lista de cosas:
        //Hacer que si un enemigo se pone en alerta, se pongan en aletar los enemigos cercanos.

        //Dependencias
        [SerializeField] float rangeToSearchPlayer;
        [SerializeField] Transform target;
        public Transform Target { get => target; set => target = value; }
        [SerializeField] NavMeshAgent agent;
        [SerializeField] MeshFilter meshFilter;
        [SerializeField] Animator anim;

        //All AIState Refes
        IdleAIEnemy_State idleState;
        AlertAIEnemy_State alertState;
        AttackAIEnemy_State attackState;
        ChaseAIEnemy_State chaseState;

        //
        IStateAI lastState;
        IStateAI currentState;


        [Space(5)]
        [Header("Vision Refs")]
        [Range(0f,1f)]
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
        
        
        
        public Transform GetCurrentTargetTransform() => Target;

       
        #region Init
        private void InitStates() //Inicializamos cada estado con las dependencias que tengan.
                                  //IMPORTANTE!! SI agregamos un estado nuevo, hay que agregarlo en la funcion GetStateByEnum
        {
            idleState = new IdleAIEnemy_State(this,transform,agent, meshFilter.mesh, rangeToSearchPlayer);
            alertState = new AlertAIEnemy_State(this,transform,agent,timeWaitBeforeChase);
            chaseState = new ChaseAIEnemy_State(this,transform,agent, visionDistanceToChase, rangeToAttack, visionOpening);
            attackState = new AttackAIEnemy_State(this, anim, rangeToAttack, timeWaitBeforeAttack, attackDamage,visionOpening);
           
        }

        private void Start()
        {
            InitStates();
            ChangeState(State.Idle);
        }

        #endregion

        #region StateMachine

        public void ChangeState(State _newState) //Cambia el estado y guarda el ultimo en el que estuvo.
        {
            IStateAI newState = GetStateByEnum(_newState); //Se obtiene la instancia del estado que corresponda segun el enum del parametro

            if (currentState == newState) return;

            agent.isStopped = true;

            currentState?.OnFinish(); //Se ejecuta el final del estado

            lastState = currentState;
            currentState = newState;

            agent.isStopped = false;

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
                    return attackState;
                default:
                    break;
            }
            return null;
        }

        #endregion

        #region OnVision And Range States
        public bool OnVisionAndRange(OnVisionAndRangeState rangeState)
        {
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
            if (HP == 0) OnDeath();
        }

        public void OnDeath()
        {
            // TODO Handle Enemy death

            Destroy(gameObject);
            OnExitPlayMode?.Invoke();
            OnTargetDies?.Invoke();
        }

        public void GiveHP(float ammount) => HP = Mathf.Clamp(HP + ammount, 0, maxHP);

        public Vector3 GetPosition() => transform.position;

        public void SetTarget(Transform _target) => Target = _target;

        #endregion






        private void OnDrawGizmos() //Crea una esfera y cambia el color dependiendo el estado en que se encuentre.
        {
            if (currentState == null) return;
            Gizmos.color = currentState.ColorGUI();
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), 0.5f);
        }

        private void OnApplicationQuit()
        {
            OnExitPlayMode?.Invoke();
        }

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
