using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    public static List<string> FindPath(string startId, string endId, BuildingMap map)
    {
        var graph = new Dictionary<string, List<(string, float)>>();

        foreach (var edge in map.edges)
        {
            if (!graph.ContainsKey(edge.from)) graph[edge.from] = new List<(string, float)>();
            if (!graph.ContainsKey(edge.to)) graph[edge.to] = new List<(string, float)>();
            graph[edge.from].Add((edge.to, edge.dist));
            graph[edge.to].Add((edge.from, edge.dist));
        }

        var queue = new SortedSet<(float, string, List<string>)>(
            Comparer<(float, string, List<string>)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));
        var visited = new HashSet<string>();

        queue.Add((0, startId, new List<string> { startId }));

        while (queue.Count > 0)
        {
            var (cost, node, path) = queue.Min;
            queue.Remove(queue.Min);

            if (visited.Contains(node)) continue;
            visited.Add(node);

            if (node == endId) return path;

            foreach (var (neighbor, dist) in graph.GetValueOrDefault(node, new()))
            {
                if (!visited.Contains(neighbor))
                {
                    var newPath = new List<string>(path) { neighbor };
                    queue.Add((cost + dist, neighbor, newPath));
                }
            }
        }
        return new List<string>();
    }
}