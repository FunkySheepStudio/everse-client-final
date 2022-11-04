using System.Collections.Generic;
using UnityEngine;

namespace FunkySheep.States
{
    public abstract class State : ScriptableObject
    {
        public List<Dependecy> parents;
        public List<Dependecy> children;
        public bool started;
        [HideInInspector]
        public Manager manager;

        private void OnEnable()
        {
            started = false;
            children = new List<Dependecy>();
        }

        public void Init(Manager manager)
        {
            this.manager = manager;
            this.started = false;
            foreach (Dependecy parent in parents)
            {
                parent.state.children.Add(new Dependecy
                {
                    state = this,
                    start = parent.start,
                    update = parent.update,
                    stop = parent.stop
                });
            }
        }

        public void PreStart()
        {
            if (started)
                return;

            foreach (Dependecy parent in parents)
            {
                if (parent.start && !parent.state.started)
                    return;
            }

            Start();
            PostStart();
        }

        public abstract void Start();

        void PostStart()
        {
            this.started = true;
            foreach (Dependecy child in children)
            {
                if (child.start)
                    child.state.PreStart();
            }
        }

        public abstract void Update();

        public void PreStop()
        {
            if (!started)
                return;

            Stop();
            PostStop();
        }

        public abstract void Stop();

        void PostStop()
        {
            this.started = false;
            foreach (Dependecy child in children)
            {
                if (child.stop)
                    child.state.PreStop();
            }
        }
    }
}
