using FunkySheep.Buildings.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(CalculatePointsCoordinates))]
    public partial class TriangulateRoof : SystemBase
    {
        protected override void OnUpdate()
        {
           
        }
        public void OnDrawGizmos()
        {
            Entities.ForEach((DynamicBuffer<Points> points, DynamicBuffer<Vertices> vertices) =>
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Gizmos.color = Color.green;
                    float3 position = new float3
                    {
                        x = vertices[i].vertex.position.x,
                        y = 0,
                        z = vertices[i].vertex.position.y
                    };
                    Gizmos.DrawCube(position, Vector3.one * 3);
                }
            })
            .WithoutBurst()
            .Run();
        }
    }
}