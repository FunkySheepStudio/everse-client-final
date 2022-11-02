using UnityEngine;

namespace FunkySheep.States
{
    [CreateAssetMenu(menuName = "FunkySheep/States/State")]
    public class State : ScriptableObject
    {
        public Events.Event onEnterState;
        public Events.Event onExitState;
        [HideInInspector]
        public Manager manager;

        public virtual void SwitchState(State state)
        {
            ExitState();
            state.EnterState(manager);
        }

        public virtual void EnterState(Manager manager)
        {
            this.manager = manager;
            this.manager.currentState = this;
            if (onEnterState)
                onEnterState.Raise(this);
        }

        public virtual void ExitState()
        {
            if (onExitState)
                onExitState.Raise(this);
        }

        public virtual void Update()
        {
        }
    }
}
