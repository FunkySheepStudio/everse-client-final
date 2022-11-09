using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;
using FunkySheep.Collections;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Geometry
{
    public struct Vertices : IBufferElementData
    {
        public int Value;
    }

    public struct Ears : IBufferElementData
    {
        public int Value;
    }

    public static class EarClipping
    {
        //Check if a vertex if reflex or convex, and add to appropriate list
        [BurstCompile]
        public static bool IsReflex(float2 previous, float2 v, float2 next)
        {
            return Geometry.utils.IsTriangleOrientedClockwise(
                previous,
                v,
                next
            );
        }

        //Check if a vertex is an ear
        [BurstCompile]
        public static bool IsVertexEar(float2 prev, float2 current, float2 next, DynamicBuffer<Vertices> vertices, DynamicBuffer<Points> points)
        {
            //A reflex vertex cant be an ear!
            if (IsReflex(prev, current, next))
            {
                return false; 
            }

            bool hasPointInside = false;

            for (int i = 0; i < vertices.Length; i++)
            {
                float2 prevVertex = points[vertices[Utils.ClampListIndex(i - 1, vertices.Length)].Value].ToXY();
                float2 vertex = points[vertices[Utils.ClampListIndex(i, vertices.Length)].Value].ToXY();
                float2 nextVertex = points[vertices[Utils.ClampListIndex(i + 1, vertices.Length)].Value].ToXY();
                //We only need to check if a reflex vertex is inside of the triangle
                if (IsReflex(
                        prevVertex,
                        vertex,
                        nextVertex
                    ))
                {
                    //This means inside and not on the hull
                    if (IsPointInTriangle
                        (
                            prev,
                            current,
                            next,
                            vertex
                        )
                    )
                    {
                        hasPointInside = true;
                        break;
                    }
                }
            }

            return !hasPointInside;
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
