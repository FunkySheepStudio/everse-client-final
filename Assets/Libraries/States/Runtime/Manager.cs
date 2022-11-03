using System.Collections.Generic;
using UnityEngine;

namespace FunkySheep.States
{
    [AddComponentMenu("FunkySheep/States/Manager")]
    public class Manager : MonoBehaviour
    {
        public State startingState;
        public State currentState;
        public List<Manager> consumers;

        private void Awake()
        {
            currentState = startingState;
        }

        private void Start()
        {
            startingState.StartedState(this);
        }

        private void Update()
        {
            if (currentState)
                currentState.Update();
        }
    }
}
