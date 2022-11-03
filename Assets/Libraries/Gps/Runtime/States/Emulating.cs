using UnityEngine;
using FunkySheep.States;
using Unity.Mathematics;
using System;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Emulating")]
    public class Emulating : State
    {
        public FunkySheep.Earth.Types.GpsCoordinates emulatedCoordinates;
        public FunkySheep.Earth.Types.GpsCoordinates gpsCoordinates;

        public override void StartedState(FunkySheep.States.Manager manager)
        {
            gpsCoordinates.Value = emulatedCoordinates.Value;
            base.StartedState(manager);
        }
    }
}
