using RPG.AI;
using UnityEngine;

namespace RPG
{
    public interface IStateMachine 
    {
        public void ChangeState(IStateAI _newState) { }
        public void ChangeToLastState() { }
    }
}
