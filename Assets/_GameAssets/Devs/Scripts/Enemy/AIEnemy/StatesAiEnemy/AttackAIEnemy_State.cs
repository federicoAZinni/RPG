using System.Runtime.InteropServices.WindowsRuntime;
using RPG.AI;
using UnityEngine;

namespace RPG
{
    public class AttackAIEnemy_State : IStateEnemyAI
    {
        public void Action()
        {
            
        }

        public Color ColorGUI()
        {
            return Color.red;
        }

        public void OnFinish()
        {
            
        }

        public void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");
        }
    }
}
