using System.Collections;
using UnityEngine;
using FunkySheep.States;
using UnityEngine.Android;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Starting")]
    public class Starting : State
    {
        public State waitingUnityRemoteState;
        public override void EnterState(FunkySheep.States.Manager manager)
        {
            base.EnterState(manager);
#if UNITY_EDITOR
            SwitchState(waitingUnityRemoteState);
#endif
        }
    }
}
