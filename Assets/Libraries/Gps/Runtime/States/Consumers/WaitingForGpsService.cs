using UnityEngine;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Consumers/Waiting for Gps Service")]
    public class WaitingForGpsService : FunkySheep.States.State
    {
        public FunkySheep.States.State gpsRunningState;
        public FunkySheep.States.State gpsEmulatingState;
        public FunkySheep.States.State nextState;

        public override void OnDependencyEnterState(FunkySheep.States.State state)
        {
            base.OnDependencyEnterState(state);
            if (state.Equals(gpsRunningState) || state.Equals(gpsEmulatingState))
                SwitchState(nextState);
        }
    }
}
