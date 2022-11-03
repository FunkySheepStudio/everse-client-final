using UnityEngine;
using FunkySheep.States;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Running")]
    public class Running : State
    {
        public FunkySheep.Earth.Types.GpsCoordinates gpsCoordinates;
        public override void StartedState(FunkySheep.States.Manager manager)
        {
            gpsCoordinates.Value = new Unity.Mathematics.double2
            {
                x = Input.location.lastData.latitude,
                y = Input.location.lastData.longitude
            };

            base.StartedState(manager);
        }
    }
}
