using FunkySheep.States;
using UnityEngine;

namespace FunkySheep.Buildings.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Buildings/States/Starting")]
    public class Starting : FunkySheep.States.State
    {
        public FunkySheep.States.State dependencyCheck;
        public FunkySheep.States.State nextState;

        public override void OnDependencyEnterState(State state)
        {
            base.OnDependencyEnterState(state);
            if (state.Equals(dependencyCheck))
            {
                SwitchState(nextState);
            }
        }
    }
}
