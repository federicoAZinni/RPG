using System.Collections;
using UnityEngine;

namespace RPG.AI
{
    public class IdleAIEnemy_State : IStateEnemyAI
    {
        Vector3 startPosRef;

        public IdleAIEnemy_State(Vector3 startPosRef)
        {
            this.startPosRef = startPosRef;
        }

        public void OnStart()
        {
            Debug.Log($"OnStart, State : {this.GetType().Name}");
        }

        public void OnFinish()
        {
            Debug.Log($"OnFinish, State : {this.GetType().Name}");
        }

        public Color ColorGUI() => Color.white;

        public async void Action()
        {
            
        }
    }

}
