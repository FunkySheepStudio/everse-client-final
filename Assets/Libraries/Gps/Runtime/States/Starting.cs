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

        public override void Start()
        {
            if (emulateGps)
            {
                manager.AddState(emulatingState);
            }
            else
            {
#if UNITY_EDITOR
                manager.AddState(waitingUnityRemoteState);
#else
                manager.AddState(checkDevicePermissionsState);
#endif
            }
            manager.RemoveState(this);
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
        }
    }
}
