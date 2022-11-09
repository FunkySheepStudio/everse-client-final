using Unity.Entities;
using UnityEngine;
using FunkySheep.Geometry;
using FunkySheep.Geometry.Components;
using FunkySheep.Collections;

namespace FunkySheep.Buildings.Systems
{
    public struct Triangles : IBufferElementData
    {
        public int Value;
    }

    [UpdateAfter(typeof(CalculateClippingEars))]
    public partial class TriangulateRoof : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, DynamicBuffer<Points> points, DynamicBuffer<Vertices> vertices, DynamicBuffer<Ears> ears, DynamicBuffer<Triangles> triangles) =>
            {
                if (ears.Length == 0)
                    return;

                triangles.Add(new Triangles { Value = vertices[Utils.ClampListIndex(ears[0].Value, vertices.Length)].Value });
                triangles.Add(new Triangles { Value = vertices[Utils.ClampListIndex(ears[0].Value - 1, vertices.Length)].Value });
                triangles.Add(new Triangles { Value = vertices[Utils.ClampListIndex(ears[0].Value + 1, vertices.Length)].Value });

                vertices.RemoveAt(ears[0].Value);
                ears.Clear();
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
        public void OnDrawGizmos()
        {
            Entities.ForEach((DynamicBuffer<Points> points, DynamicBuffer<Triangles> triangles) =>
            {
                for (int i = 0; i < triangles.Length - 2; i+=3)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(
                        points[triangles[i].Value].Value,
                        points[triangles[i + 1].Value].Value
                    );

                    Gizmos.DrawLine(
                        points[triangles[i + 1].Value].Value,
                        points[triangles[i + 2].Value].Value
                    );

                    Gizmos.DrawLine(
                        points[triangles[i + 2].Value].Value,
                        points[triangles[i].Value].Value
                    );
                }
            })
            .WithoutBurst()
            .Run();
        }
    }
}