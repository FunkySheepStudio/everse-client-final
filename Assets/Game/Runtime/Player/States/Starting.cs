using FunkySheep.States;
using UnityEngine;

namespace Game.Player.States
{
    [CreateAssetMenu(menuName = "Game/Player/States/Starting")]
    public class Starting : FunkySheep.States.State
    {
        public FunkySheep.Maps.CalculateTileSize calculateTileSize;
        public FunkySheep.States.State nextState;

        public override void OnDependencyEnterState(State state)
        {
            base.OnDependencyEnterState(state);
            if (state.Equals(calculateTileSize))
                SwitchState(nextState);
        }
    }
}
