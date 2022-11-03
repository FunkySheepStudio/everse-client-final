using System.Collections.Generic;
using UnityEngine;

namespace FunkySheep.States
{
    [CreateAssetMenu(menuName = "FunkySheep/States/State")]
    public class State : ScriptableObject
    {
        public List<State> dependencies;
        [HideInInspector]
        public Manager manager;

        public virtual void SwitchState(State state)
        {
            ExitedState();
            state.StartedState(manager);
        }

        public virtual void StartedState(Manager manager)
        {
            this.manager = manager;
            this.manager.currentState = this;
            foreach (Manager consumer in manager.consumers)
            {
                consumer.currentState.OnDependencyEnterState(this);
            }
        }

        public virtual void OnDependencyEnterState(State state)
        {
            if (!dependencies.Contains(state))
                return;
        }

        public virtual void OnDependencyExitState(State state)
        {
            if (!dependencies.Contains(state))
                return;
        }

        public virtual void ExitedState()
        {
            foreach (Manager consumer in manager.consumers)
            {
                consumer.currentState.OnDependencyExitState(this);
            }
        }

        public virtual void Update()
        {
        }
    }
}
