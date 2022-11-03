using UnityEngine;
using FunkySheep.States;
using UnityEngine.Android;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Check Device Permissions State")]
    public class CheckDevicePermissionsState : State
    {
        public State inputLocationServicesState;
        public override void StartedState(FunkySheep.States.Manager manager)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }
            base.StartedState(manager);
        }

        public override void Update()
        {
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation) && Input.location.isEnabledByUser)
            {
                SwitchState(inputLocationServicesState);
            }
        }
    }
}
