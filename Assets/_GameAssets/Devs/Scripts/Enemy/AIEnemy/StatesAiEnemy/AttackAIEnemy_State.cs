using System.Runtime.InteropServices.WindowsRuntime;
using RPG.AI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace RPG
{
    public class AttackAIEnemy_State : IStateEnemyAI
    {
        public Task Action(CancellationToken cancellationToken)
        {
            return null;
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
