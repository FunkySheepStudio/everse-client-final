using FunkySheep.Buildings.Components;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Systems
{
    public struct Vertex : IComponentData
    {
        public float2 position;
        public bool isReflex;
    }

    public struct Vertices: IBufferElementData
    {
        public Vertex vertex;
    }

    [UpdateAfter(typeof(CalculatePointsCoordinates))]
    public partial class CalculateClippingEars : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, DynamicBuffer<Points> points) =>
            {
                // Set the vertex array
                NativeArray<Vertex> vertices = new NativeArray<Vertex>(points.Length, Allocator.Temp);
                for (int i = 0; i < points.Length; i++)
                {
                    float2 previousPosition = new float2
                    {
                        x = points[ClampListIndex(i - 1, points.Length)].Value.x,
                        y = points[ClampListIndex(i - 1, points.Length)].Value.z
                    };

                    float2 position = new float2
                    {
                        x = points[i].Value.x,
                        y = points[i].Value.z
                    };

                    float2 nextPosition = new float2
                    {
                        x = points[ClampListIndex(i + 1, points.Length)].Value.x,
                        y = points[ClampListIndex(i + 1, points.Length)].Value.z
                    };


                    bool isReflex = !CheckIfReflexOrConvex(
                        previousPosition,
                        position,
                        nextPosition
                    );

                    vertices[i] = new Vertex
                    {
                        position = position,
                        isReflex = isReflex
                    };
                }

                //Find the ears
                DynamicBuffer<Vertices> ears = buffer.AddBuffer<Vertices>(entity);
                for (int i = 0; i < vertices.Length; i++)
                {
                    IsVertexEar(i, vertices, ears);
                }

                vertices.Dispose();

            })
            .WithNone<Vertices>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }

        //Clamp list indices
        //Will even work if index is larger/smaller than listSize, so can loop multiple times
        [BurstCompile]
        public static int ClampListIndex(int index, int listSize)
        {
            index = ((index % listSize) + listSize) % listSize;

            return index;
        }

        //Check if a vertex if reflex or convex, and add to appropriate list
        [BurstCompile]
        private static bool CheckIfReflexOrConvex(float2 previous, float2 v, float2 next)
        {
            return !Geometry.utils.IsTriangleOrientedClockwise(
                previous,
                v,
                next
            );
        }

        //Check if a vertex is an ear
        [BurstCompile]
        private static void IsVertexEar(int index, NativeArray<Vertex> vertices, DynamicBuffer<Vertices> ears)
        {
            //A reflex vertex cant be an ear!
            if (vertices[index].isReflex)
            {
                return;
            }

            bool hasPointInside = false;

            for (int i = 0; i < vertices.Length; i++)
            {
                //We only need to check if a reflex vertex is inside of the triangle
                if (vertices[i].isReflex)
                {
                    float2 p = vertices[i].position;

                    //This means inside and not on the hull
                    if (IsPointInTriangle
                        (
                            vertices[ClampListIndex(index - 1, vertices.Length)].position,
                            vertices[ClampListIndex(index, vertices.Length)].position,
                            vertices[ClampListIndex(index + 1, vertices.Length)].position,
                            p
                        )
                    )
                    {
                        hasPointInside = true;
                        break;
                    }
                }
            }

            if (!hasPointInside)
            {
                ears.Add(new Vertices
                {
                    vertex = vertices[index]
                });
            }
        }

        //From http://totologic.blogspot.se/2014/01/accurate-point-in-triangle-test.html
        //p is the testpoint, and the other points are corners in the triangle
        [BurstCompile]
        public static bool IsPointInTriangle(float2 p1, float2 p2, float2 p3, float2 p)
        {
            bool isWithinTriangle = false;

            //Based on Barycentric coordinates
            float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));

            float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / denominator;
            float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / denominator;
            float c = 1 - a - b;

            //The point is within the triangle or on the border if 0 <= a <= 1 and 0 <= b <= 1 and 0 <= c <= 1
            //if (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f)
            //{
            //    isWithinTriangle = true;
            //}

            //The point is within the triangle
            if (a > 0f && a < 1f && b > 0f && b < 1f && c > 0f && c < 1f)
            {
                isWithinTriangle = true;
            }

            return isWithinTriangle;
        }
    }
}
