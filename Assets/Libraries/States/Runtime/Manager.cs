using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FunkySheep.States
{
    [AddComponentMenu("FunkySheep/States/Manager")]
    public class Manager : MonoBehaviour
    {
        public List<State> states;

        private void Awake()
        {
            foreach (State state in states)
            {
                state.Init(this);
            }
        }

        private void Start()
        {
            foreach (State state in states.ToList())
            {
                state.PreStart();
            }
        }

        private void Update()
        {
            foreach (State state in states.ToList())
            {
                if (state.started)
                    state.Update();
            }
        }

        public void AddState(State state)
        {
            states.Add(state);
            state.Init(this);
            state.PreStart();
        }

        public void RemoveState(State state)
        {
            state.PreStop();
            states.Remove(state);
        }
    }
}
