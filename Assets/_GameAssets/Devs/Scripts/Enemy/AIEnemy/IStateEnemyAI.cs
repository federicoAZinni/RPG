using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

namespace RPG.AI
{
    public interface IStateEnemyAI
    {
        public void OnStart();
        public void OnFinish();
        public Color ColorGUI();
        public void Action();

    }
}
