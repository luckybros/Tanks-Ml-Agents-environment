using UnityEngine;

public class TimeController : MonoBehaviour
{
    void Start()
    {
        PrintEngineStats("AVVIO (Valori di Default di Unity)");
    }

    void Update()
    {
        // Premi P per stampare lo stato attuale in qualsiasi momento
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 1f;
        }
    }

    void PrintEngineStats(string context)
    {
        Debug.Log($"\n=== STATISTICHE MOTORE: {context} ===");
        Debug.Log($"[Time] TimeScale: {Time.timeScale}");
        Debug.Log($"[Time] CaptureFrameRate: {Time.captureFramerate}");
        Debug.Log($"[Time] TargetFrameRate: {Application.targetFrameRate}");
        Debug.Log($"[Fisica] DeltaTime Attuale: {Time.deltaTime}");
        Debug.Log($"[Fisica] FixedDeltaTime: {Time.fixedDeltaTime}");
        Debug.Log($"[Video] Risoluzione: {Screen.width}x{Screen.height}");
        Debug.Log($"[Video] Quality Level: {QualitySettings.GetQualityLevel()}");
        Debug.Log($"=========================================\n");
    }
}