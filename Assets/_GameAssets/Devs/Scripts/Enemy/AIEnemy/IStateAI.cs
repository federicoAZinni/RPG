using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace RPG.AI
{
    public abstract class IStateAI
    {
        public CancellationToken tokenSource;
        public abstract Task OnStart();
        public abstract  void OnFinish();
        public abstract Color ColorGUI();
        protected abstract Task Action(CancellationToken cancellationToken);
        
    }
}
