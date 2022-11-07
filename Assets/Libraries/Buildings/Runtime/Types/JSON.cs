using System;

namespace FunkySheep.Buildings.Types
{
    [System.Serializable]
    public struct JsonOsmWays
    {
        public JsonOsmWay[] elements;
    }

    [System.Serializable]
    public struct JsonOsmRelations
    {
        public JsonOsmRelation[] elements;
    }

    [System.Serializable]
    public struct JsonOsmWay
    {
        public string type;
        public JsonOsmGeometry[] geometry;
    }

    [System.Serializable]
    public struct JsonOsmRelation
    {
        public JsonOsmWay[] members;
    }

    [System.Serializable]
    public struct JsonOsmGeometry
    {
        public double lat;
        public double lon;
    }
}