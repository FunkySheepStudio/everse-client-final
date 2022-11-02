using UnityEngine;

namespace FunkySheep.States
{
    [AddComponentMenu("FunkySheep/States/Manager")]
    public class Manager : MonoBehaviour
    {
        public State startingState;
        public State currentState;

        private void Start()
        {
            startingState.EnterState(this);
        }

        private void Update()
        {
            if (currentState)
                currentState.Update();
        }
    }
}
