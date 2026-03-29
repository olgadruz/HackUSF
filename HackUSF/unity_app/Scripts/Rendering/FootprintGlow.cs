using UnityEngine;

public class FootprintGlow : MonoBehaviour
{
    public float delay = 0f;        // задержка старта
    public float glowSpeed = 2f;    // скорость мерцания
    public Color glowColor = new Color(0f, 1f, 0.8f, 1f); // циановый цвет

    private Renderer rend;
    private float timer = 0f;
    private bool started = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        // ставим начальный цвет
        rend.material.color = glowColor;
        rend.material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        timer += Time.deltaTime;

        // ждём своей очереди
        if (timer < delay) return;

        // мерцаем — яркость пульсирует
        float brightness = (Mathf.Sin((timer - delay) * glowSpeed) + 1f) / 2f;
        Color emission = glowColor * brightness * 2f;
        rend.material.SetColor("_EmissionColor", emission);
    }
}
