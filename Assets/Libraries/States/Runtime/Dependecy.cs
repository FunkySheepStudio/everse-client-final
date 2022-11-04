namespace FunkySheep.States
{
    [System.Serializable]
    public class Dependecy
    {
        public State state;
        public bool start;
        public bool update;
        public bool stop;
    }
}
