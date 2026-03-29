[System.Serializable]
public class MapNode
{
    public string id;
    public string label;
    public float x;
    public float y;
}

[System.Serializable]
public class MapEdge
{
    public string from;
    public string to;
    public float dist;
}

[System.Serializable]
public class BuildingMap
{
    public MapNode[] nodes;
    public MapEdge[] edges;
}