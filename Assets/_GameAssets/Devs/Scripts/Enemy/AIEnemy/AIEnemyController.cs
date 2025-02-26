using NUnit.Framework.Constraints;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace RPG.AI
{
    public class AIEnemyController : MonoBehaviour
    {

        //Dependencias
        [SerializeField] Transform player_T;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] MeshFilter meshFilter;

        //All AIState Refes
        IdleAIEnemy_State idleState;
        AlertAIEnemy_State alertState;
        AttackAIEnemy_State attackState;
        ChaseAIEnemy_State chaseState;

        //
        IStateEnemyAI lastState;
        IStateEnemyAI currentState;


        [Space(5)]
        [Header("Vision Refs")]
        [SerializeField] float visionOpening = 0.9f;
        [SerializeField] float visionDistanceToAlert;
        [SerializeField] float visionDistanceToChase;
        public bool onVisionToAlert;
        public bool onVisionToChase;

        [Space(5)]
        [Header("Attack Refs")]
        [SerializeField] float timeWaitBeforeAttack;

        bool onVisionflag;

        public static Action OnExitPlayMode;

        private void Awake()
        {
            InitStates();
        }

        private void InitStates() //Inicializamos cada estado con las dependencias que tengan.
        {
            idleState = new IdleAIEnemy_State(transform.position,this,agent, meshFilter.mesh);
            alertState = new AlertAIEnemy_State(player_T,transform,this, timeWaitBeforeAttack);
            attackState = new AttackAIEnemy_State();
            chaseState = new ChaseAIEnemy_State(player_T,agent,this);
        }



        private void Start()
        {
            ChangeState(State.Idle);
        }


        private void Update()
        {
            onVisionToAlert = OnVision(visionDistanceToAlert); //Detecta si el player se enceuntra dentro del cono de vision de alerta
            onVisionToChase = OnVision(visionDistanceToChase); //Detecta si el player se enceuntra dentro del cono de vision de perseguir

        }


        public void ChangeState(State _newState) //Cambia el estado y guarda el ultimo en el que estuvo.
        {
            IStateEnemyAI newState = GetStateByEnum(_newState); //Se obtiene la instancia del estado que corresponda segun el enum del parametro
            if (currentState == newState) return;
            agent.isStopped = true;
            currentState?.OnFinish(); //Se ejecuta el final del estado
            lastState = currentState;
            currentState = newState;
            agent.isStopped = false;
            currentState.OnStart(); //Se ejecuta el inicio del estado
        }

        public void ChangeToLastState()
        {
            IStateEnemyAI temp = currentState;
            currentState?.OnFinish();
            currentState = lastState;
            lastState = temp;
            currentState.OnStart();
        }


        IStateEnemyAI GetStateByEnum(State state)
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

        bool OnVision(float distance)
        {
            Vector3 dir = (player_T.position - transform.position).normalized;

            if (Vector3.Distance(player_T.position, transform.position) < distance)
            {
                float dot = Vector3.Dot(transform.right, dir);
                if (dot < visionOpening && dot > -visionOpening)
                    return true;
            }
            return false;
        }



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
}
