using Unity.Burst;

namespace FunkySheep.Collections
{
    public class Utils
    {
        //Clamp list indices
        //Will even work if index is larger/smaller than listSize, so can loop multiple times
        [BurstCompile]
        public static int ClampListIndex(int index, int listSize)
        {
            index = ((index % listSize) + listSize) % listSize;

            return index;
        }
    }
}
