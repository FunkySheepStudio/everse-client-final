using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Geometry
{
    public struct utils
    {
        //Is a triangle in 2d space oriented clockwise or counter-clockwise
        //https://math.stackexchange.com/questions/1324179/how-to-tell-if-3-connected-points-are-connected-clockwise-or-counter-clockwise
        //https://en.wikipedia.org/wiki/Curve_orientation
        [BurstCompile]
        public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            bool isClockWise = true;

            float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

            if (determinant > 0f)
            {
                isClockWise = false;
            }

            return isClockWise;
        }

        [BurstCompile]
        public static bool IsTriangleOrientedClockwise(float2 p1, float2 p2, float2 p3)
        {
            bool isClockWise = true;

            float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

            if (determinant > 0f)
            {
                isClockWise = false;
            }
            return isClockWise;
        }

        [BurstCompile]
        public static bool IsTriangleOrientedClockwise(double2 p1, double2 p2, double2 p3)
        {
            bool isClockWise = true;

            double determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

            if (determinant > 0f)
            {
                isClockWise = false;
            }

            return isClockWise;
        }

        [BurstCompile]
        public static bool IsCollinear(float2 p1, float2 p2, float2 p3)
        {
            float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

            if (determinant == 0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [BurstCompile]
        public static bool IsCollinear(double2 p1, double2 p2, double2 p3)
        {
            double determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

            if (determinant == 0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
