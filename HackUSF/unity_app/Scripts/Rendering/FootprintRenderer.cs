using System.Collections.Generic;
using UnityEngine;

public class FootprintRenderer : MonoBehaviour
{
    // === НАСТРОЙКИ ===
    public GameObject footprintPrefab;   // спрайт следа ноги
    public NavigationManager navManager;
    public float stepDistance = 0.5f;    // расстояние между следами (в метрах)
    public float footprintHeight = 0.05f; // чуть выше пола

    private List<GameObject> activeFootprints = new List<GameObject>();

    // === ГЛАВНАЯ ФУНКЦИЯ ===
    public void ShowPath(List<string> path)
    {
        ClearFootprints();

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 from = navManager.GetNodePosition(path[i]);
            Vector3 to = navManager.GetNodePosition(path[i + 1]);
            SpawnFootprintsBetween(from, to);
        }
    }

    // === РАССТАВЛЯЕМ СЛЕДЫ МЕЖДУ ДВУМЯ ТОЧКАМИ ===
    void SpawnFootprintsBetween(Vector3 from, Vector3 to)
    {
        float totalDistance = Vector3.Distance(from, to);
        int steps = Mathf.FloorToInt(totalDistance / stepDistance);
        Vector3 direction = (to - from).normalized;

        // угол поворота следа — по направлению движения
        Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

        for (int i = 0; i < steps; i++)
        {
            // позиция следа вдоль пути
            Vector3 pos = from + direction * (i * stepDistance);
            pos.y = footprintHeight;

            // чередуем левую и правую ногу — смещение в сторону
            float sideOffset = (i % 2 == 0) ? 0.15f : -0.15f;
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
            pos += right * sideOffset;

            // создаём след
            GameObject fp = Instantiate(footprintPrefab, pos, rotation);

            // добавляем мерцание с задержкой по времени
            var glow = fp.AddComponent<FootprintGlow>();
            glow.delay = i * 0.15f; // каждый след мерцает чуть позже предыдущего

            activeFootprints.Add(fp);
        }
    }

    // === УДАЛЯЕМ ВСЕ СЛЕДЫ ===
    public void ClearFootprints()
    {
        foreach (var fp in activeFootprints)
            Destroy(fp);
        activeFootprints.Clear();
    }
}