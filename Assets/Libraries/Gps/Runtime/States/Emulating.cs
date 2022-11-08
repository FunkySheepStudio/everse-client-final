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
        public FunkySheep.States.State runningState;

        public override void Start()
        {
            gpsCoordinates.Value = emulatedCoordinates.Value;
            manager.AddState(runningState);
            manager.RemoveState(this);
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
        }

        public override void OnDrawGizmos()
        {
        }
    }
}
