using UnityEngine;
using FunkySheep.States;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Starting")]
    public class Starting : State
    {
        public bool emulateGps;
        public State waitingUnityRemoteState;
        public State checkDevicePermissionsState;
        public State emulatingState;
        public override void StartedState(FunkySheep.States.Manager manager)
        {
            base.StartedState(manager);
            if (emulateGps)
            {
                SwitchState(emulatingState);
                return;
            }

#if UNITY_EDITOR
            SwitchState(waitingUnityRemoteState);
#else
            SwitchState(checkDevicePermissionsState);
#endif
        }
    }
}
