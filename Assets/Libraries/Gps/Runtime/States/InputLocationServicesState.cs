using UnityEngine;
using FunkySheep.States;
using UnityEngine.Android;

namespace FunkySheep.Gps.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Gps/States/Start Input Location Services State")]
    public class InputLocationServicesState : State
    {
        public State runningState;
        public override void StartedState(FunkySheep.States.Manager manager)
        {
            Input.location.Start();
            base.StartedState(manager);
        }

        public override void Update()
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                SwitchState(runningState);
            }
        }
    }
}
