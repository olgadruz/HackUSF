using UnityEngine;
using ZXing;
using System;

public class QRScanner : MonoBehaviour
{
    // === НАСТРОЙКИ ===
    public NavigationManager navManager;
    public UIController uiController;

    private WebCamTexture camTexture;  // поток с камеры Quest
    private BarcodeReader reader;      // ZXing — библиотека для чтения QR
    private bool isScanning = true;    // сканируем или нет
    private float scanInterval = 0.5f; // проверяем каждые 0.5 сек
    private float timer = 0f;

    void Start()
    {
        // запускаем камеру
        camTexture = new WebCamTexture();
        camTexture.Play();

        // создаём читалку QR кодов
        reader = new BarcodeReader();

        Debug.Log("QR Scanner started");
    }

    void Update()
    {
        if (!isScanning) return;

        // проверяем не каждый кадр а раз в 0.5 сек — экономим CPU
        timer += Time.deltaTime;
        if (timer < scanInterval) return;
        timer = 0f;

        ScanFrame();
    }

    void ScanFrame()
    {
        try
        {
            // берём текущий кадр с камеры
            Color32[] pixels = camTexture.GetPixels32();
            int width = camTexture.width;
            int height = camTexture.height;

            // пробуем найти QR код в кадре
            var result = reader.Decode(pixels, width, height);

            if (result != null)
            {
                string nodeId = result.Text; // например "entrance"
                Debug.Log("QR scanned: " + nodeId);
                OnQRScanned(nodeId);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Scan error: " + e.Message);
        }
    }

    void OnQRScanned(string nodeId)
    {
        // останавливаем сканирование — уже знаем где мы
        isScanning = false;

        // говорим UI показать меню выбора назначения
        uiController.ShowDestinationMenu(nodeId);

        Debug.Log("Current position: " + nodeId);
    }

    // вызывается когда пользователь хочет пересканировать
    public void RestartScanning()
    {
        isScanning = true;
        timer = 0f;
        Debug.Log("Scanning restarted");
    }

    void OnDestroy()
    {
        // выключаем камеру когда выходим
        if (camTexture != null)
            camTexture.Stop();
    }
}
