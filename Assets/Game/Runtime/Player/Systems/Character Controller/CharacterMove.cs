using Unity.Entities;

namespace Game.Player.Systems
{
    public partial class CharacterMove : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(() =>
            {
            }).Run();
        }
    }
}
