using System.Collections;
using UnityEngine;
using FunkySheep.States;
using UnityEngine.Android;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Waiting for Unity Remote")]
    public class WaitingForUnityRemote : State
    {
        public State checkDevicePermissionsState;
        public override void Update()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isRemoteConnected)
            {
                SwitchState(checkDevicePermissionsState);
            }
#endif
        }
    }
}
