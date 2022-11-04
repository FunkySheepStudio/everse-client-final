using UnityEngine;
using FunkySheep.States;
using UnityEngine.Android;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Check Device Permissions State")]
    public class CheckDevicePermissionsState : State
    {
        public State inputLocationServicesState;

        public override void Start()
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }
        }

        public override void Update()
        {
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation) && Input.location.isEnabledByUser)
            {
                manager.AddState(inputLocationServicesState);
                manager.RemoveState(this);
            }
        }

        public override void Stop()
        {
        }
    }
}
