using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game.Player.Systems
{
    [UpdateInGroup(typeof(PlayerPositionSystemGroup))]
    public partial class SetInitialPlayerPosition : SystemBase
    {
        public Transform playerTransform;
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Translation translation, in Components.Tags.Player player) =>
            {
                translation.Value = playerTransform.position;
            })
            .WithoutBurst()
            .Run();
            Enabled = false;
        }
    }
}
