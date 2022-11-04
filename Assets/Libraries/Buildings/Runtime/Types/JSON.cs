using System;

namespace FunkySheep.Buildings.Types
{
    [System.Serializable]
    public struct JsonOsmRoot
    {
        public JsonOsmElement[] elements;
    }

    [System.Serializable]
    public struct JsonOsmElement
    {
        public string type;
        public JsonOsmGeometry[] geometry;
    }

    [System.Serializable]
    public struct JsonOsmGeometry
    {
        public double lat;
        public double lon;
    }
}