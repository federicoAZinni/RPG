using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.AI
{
    public class AIEnemyController : MonoBehaviour
    {

        //Dependencias
        [SerializeField] Transform player_T;

        //All AIState Refes
        IdleAIEnemy_State idleState;
        AlertAIEnemy_State alertState;

        //
        IStateEnemyAI lastState;
        IStateEnemyAI currentState;


        private void Awake()
        {
            InitStates();
        }

        private void InitStates()
        {
            idleState = new IdleAIEnemy_State(transform.position);
            alertState = new AlertAIEnemy_State(player_T,transform,this);
        }



        private void Start()
        {
            ChangeState(State.Idle);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) ChangeState(State.Alert);
        }


        public void ChangeState(State _newState)
        {
            IStateEnemyAI newState = GetStateByEnum(_newState);
            if (currentState == newState) return;

            currentState?.OnFinish();
            lastState = currentState;
            currentState = newState;
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
                    break;
                case State.Attack:
                    break;
                default:
                    break;
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            if (currentState == null) return;
            Gizmos.color = currentState.ColorGUI();
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), 0.5f);
        }
    }

    public enum State
    {
        Idle,Alert,Chase,Attack
    }
}
