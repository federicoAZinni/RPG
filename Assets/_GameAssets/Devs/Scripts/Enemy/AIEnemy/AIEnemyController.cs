using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Builders;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace RPG.AI
{
    public class AIEnemyController : MonoBehaviour, IDamageable
    {

        //Lista de cosas:
        //Hacer que si un enemigo se pone en alerta, se pongan en aletar los enemigos cercanos.

        //Dependencias
        [SerializeField] Transform player_T;
        public Transform Player_T { get => player_T; set => player_T = value; }
        [SerializeField] NavMeshAgent agent;
        [SerializeField] MeshFilter meshFilter;
        [SerializeField] Animator anim;

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
        [Range(0f,1f)]
        [SerializeField] float visionOpening = 0.9f;
        [SerializeField] float visionDistanceToAlert;
        [SerializeField] float visionDistanceToChase;
        [SerializeField] float timeWaitBeforeChase;

        public bool onVisionToAlert;
        public bool onVisionToChase;
        

        [Space(5)]
        [Header("Attack Refs")]
        [SerializeField] float rangeToAttack;
        [SerializeField] float timeWaitBeforeAttack;
        [SerializeField] float attackDamage;

        public bool onRangeToAttack;

        [Space(5)]
        [Header("Health Refs")]
        [SerializeField] float maxHP;

        public float HP { get; private set; }
       

        //Events
        public event Action OnTargetDies;
        public static Action OnExitPlayMode;

        private void InitStates() //Inicializamos cada estado con las dependencias que tengan.
                                  //IMPORTANTE!! SI agregamos un estado nuevo, hay que agregarlo en la funcion GetStateByEnum
        {
            idleState = new IdleAIEnemy_State(this,visionDistanceToAlert,agent, meshFilter.mesh);
            alertState = new AlertAIEnemy_State(Player_T,transform,agent,this,timeWaitBeforeChase);
            chaseState = new ChaseAIEnemy_State(Player_T,agent,this);
            attackState = new AttackAIEnemy_State(this, anim,rangeToAttack, timeWaitBeforeAttack, attackDamage);
           
        }



        //private IEnumerator Start()
        //{
        //    yield return new WaitForSeconds(0.1f);// cambiar esto
        //    player_T = GameObject.FindGameObjectWithTag("Player").transform;// cambiar esto

        //    InitStates();
        //    ChangeState(State.Idle);
        //}


        //private void Update()
        //{
        //    if (player_T == null) return;// cambiar esto
        //    onVisionToAlert = OnVision(visionDistanceToAlert); //Detecta si el player se enceuntra dentro del cono de vision de alerta
        //    onVisionToChase = OnVision(visionDistanceToChase); //Detecta si el player se enceuntra dentro del cono de vision de perseguir
        //    onRangeToAttack = OnVision(rangeToAttack);
        //}


        public void ChangeState(State _newState) //Cambia el estado y guarda el ultimo en el que estuvo.
        {
            IStateEnemyAI newState = GetStateByEnum(_newState); //Se obtiene la instancia del estado que corresponda segun el enum del parametro

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

        public bool OnVision(float distance)
        {
            Vector3 dir = (Player_T.position - transform.position).normalized;


            if (Physics.Raycast(transform.position, dir, out RaycastHit hit))// si hay una pared o algo que no lo deje ver, devuelve false
                if (!hit.transform.CompareTag("Player")) return false;


            if (Vector3.Distance(Player_T.position, transform.position) < distance)//Cono de vision.
            {
                float dot = Vector3.Dot(transform.forward, dir);
                if (dot > -visionOpening)
                    return true;
                    
            }
            return false;
        }

        public Transform GetCurrentTargetTransform() => Player_T;

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




        #region test
       
       

        #endregion

    }

    public enum State
    {
        Idle,Alert,Chase,Attack
    }


}
