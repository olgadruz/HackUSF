using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{

    public BuildingMap map;

    void Start()
    {
        string path = Application.streamingAssetsPath + "/building_map.json";
        string json = File.ReadAllText(path);
        map = JsonUtility.FromJson<BuildingMap>(json);
        Debug.Log("Map loaded! Nodes: " + map.nodes.Length);
    }

    public List<string> GetPath(string startId, string endId)
    {
        return Pathfinder.FindPath(startId, endId, map);
    }

    public Vector3 GetNodePosition(string nodeId)
    {
        foreach (var node in map.nodes)
        {
            if (node.id == nodeId)
            {
                return new Vector3(node.x, 0, node.y);
            }
        }
        return Vector3.zero;
    }
}