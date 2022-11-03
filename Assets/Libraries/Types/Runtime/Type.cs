using UnityEngine;
namespace FunkySheep.Types
{
    public abstract class Type : ScriptableObject
    {
    }
    public abstract class Type<T> : Type
    {
        public T Value;

        public void Set(T Value)
        {
            this.Value = Value;
        }
    }
}