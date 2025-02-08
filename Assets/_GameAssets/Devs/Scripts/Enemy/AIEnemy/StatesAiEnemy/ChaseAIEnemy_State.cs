using RPG.AI;
using UnityEngine;

namespace RPG
{
    public class ChaseAIEnemy_State : IStateEnemyAI
    {
        public void Action()
        {
            throw new System.NotImplementedException();
        }

        public Color ColorGUI()
        {
            return Color.magenta;
        }

        public void OnFinish()
        {
            throw new System.NotImplementedException();
        }

        public void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");
        }
    }
}
