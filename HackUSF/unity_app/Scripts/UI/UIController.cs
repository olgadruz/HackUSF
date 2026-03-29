using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    // === ССЫЛКИ (напарник подключит в Unity инспекторе) ===
    public NavigationManager navManager;
    public FootprintRenderer footprintRenderer;
    public QRScanner qrScanner;

    // === UI ЭЛЕМЕНТЫ ===
    public GameObject scanPanel;        // экран "сканируй QR"
    public GameObject menuPanel;        // экран выбора комнаты
    public GameObject navigatingPanel;  // экран "вы идёте к..."
    public Transform buttonContainer;   // куда добавляем кнопки комнат
    public GameObject buttonPrefab;     // шаблон кнопки
    public TextMeshProUGUI statusText;  // текст статуса

    private string currentNodeId;       // где мы сейчас стоим

    void Start()
    {
        // в начале показываем только экран сканирования
        ShowScanPanel();
    }

    // === ВЫЗЫВАЕТСЯ ИЗ QRScanner когда QR отсканирован ===
    public void ShowDestinationMenu(string scannedNodeId)
    {
        currentNodeId = scannedNodeId;

        // прячем экран сканирования
        scanPanel.SetActive(false);
        navigatingPanel.SetActive(false);

        // очищаем старые кнопки
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        // создаём кнопку для каждой комнаты
        foreach (var node in navManager.map.nodes)
        {
            // не показываем текущую позицию как цель
            if (node.id == currentNodeId) continue;

            // создаём кнопку
            GameObject btn = Instantiate(buttonPrefab, buttonContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = node.label;

            // при нажатии — идём к этой комнате
            string destinationId = node.id;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                NavigateTo(destinationId);
            });
        }

        // показываем меню
        menuPanel.SetActive(true);
        statusText.text = "Вы у: " + navManager.map.nodes[0].label;
    }

    // === НАЧИНАЕМ НАВИГАЦИЮ ===
    void NavigateTo(string destinationId)
    {
        // строим путь
        List<string> path = navManager.GetPath(currentNodeId, destinationId);

        if (path.Count == 0)
        {
            statusText.text = "Путь не найден!";
            return;
        }

        // рисуем следы на полу
        footprintRenderer.ShowPath(path);

        // прячем меню, показываем статус навигации
        menuPanel.SetActive(false);
        navigatingPanel.SetActive(true);

        // находим label назначения
        string destLabel = destinationId;
        foreach (var node in navManager.map.nodes)
            if (node.id == destinationId) destLabel = node.label;

        statusText.text = "Идите к: " + destLabel;

        Debug.Log("Navigating: " + string.Join(" → ", path));
    }

    // === КНОПКА "ПЕРЕСКАНИРОВАТЬ" ===
    public void OnRescanPressed()
    {
        footprintRenderer.ClearFootprints();
        navigatingPanel.SetActive(false);
        menuPanel.SetActive(false);
        ShowScanPanel();
        qrScanner.RestartScanning();
    }

    void ShowScanPanel()
    {
        scanPanel.SetActive(true);
        statusText.text = "Наведите камеру на QR код";
    }
}
