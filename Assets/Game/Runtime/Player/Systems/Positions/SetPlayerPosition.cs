using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game.Player.Systems
{
    [UpdateInGroup(typeof(PlayerPositionSystemGroup))]
    [UpdateAfter(typeof(SetInitialPlayerPosition))]
    public partial class SetPlayerPosition : SystemBase
    {
        public Transform playerTransform;
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Translation translation, in Components.Tags.Player player) =>
            {
                playerTransform.position = translation.Value;
            })
            .WithoutBurst()
            .Run();
        }
    }
}
