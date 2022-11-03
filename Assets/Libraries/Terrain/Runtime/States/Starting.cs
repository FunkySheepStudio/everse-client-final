using FunkySheep.States;
using UnityEngine;

namespace FunkySheep.Terrain.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Terrain/States/Starting")]
    public class Starting : FunkySheep.States.State
    {
        public FunkySheep.States.State checkDependency;
        public FunkySheep.States.State nextState;

        public override void OnDependencyEnterState(State state)
        {
            base.OnDependencyEnterState(state);
            if (state.Equals(checkDependency))
                SwitchState(nextState);
        }
    }
}
