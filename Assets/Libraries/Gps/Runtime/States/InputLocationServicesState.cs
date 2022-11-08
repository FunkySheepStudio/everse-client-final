using UnityEngine;
using FunkySheep.States;
using UnityEngine.Android;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Start Input Location Services State")]
    public class InputLocationServicesState : State
    {
        public FunkySheep.Earth.Types.GpsCoordinates gpsCoordinates;
        public State runningState;

        public override void Start()
        {
            Input.location.Start();
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                gpsCoordinates.Value = new Unity.Mathematics.double2
                {
                    x = Input.location.lastData.latitude,
                    y = Input.location.lastData.longitude
                };

                manager.AddState(runningState);
                manager.RemoveState(this);
            }
        }

        public override void OnDrawGizmos()
        {
        }
    }
}
