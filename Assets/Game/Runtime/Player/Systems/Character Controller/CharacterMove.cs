using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Player.Systems
{
    [UpdateInGroup(typeof(CharacterControllerSystemGroup))]
    public partial class CharacterMove : SystemBase
    {
        PlayerInputs inputActions;
        protected override void OnCreate()
        {
            inputActions = new PlayerInputs();
            inputActions.Enable();
        }
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Translation translation, in Player.Components.Tags.Player player ) =>
            {
                translation.Value += (float3)inputActions.PlayerMovements.Move.ReadValue<Vector3>() * 100 * World.Time.DeltaTime;
            })
            .WithoutBurst()
            .Run();
        }
    }
}
