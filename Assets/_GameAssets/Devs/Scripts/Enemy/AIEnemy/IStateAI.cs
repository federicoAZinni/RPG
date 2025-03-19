using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace RPG.AI
{
    public interface IStateAI
    {
        public void OnStart();
        public void OnFinish();
        public Color ColorGUI();
        public Task Action(CancellationToken cancellationToken);

    }
}
