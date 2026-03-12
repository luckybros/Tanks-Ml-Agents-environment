/* using UnityEngine;
using UnityEditor;
using CodeCoverage; // Indispensabile

public static class HeadlessTrainingLauncher
{
    public static void StartPlayMode()
    {
        // 1. Configurazione Manuale (Sostituisce i flag del terminale)
        // Questo evita che i parametri da riga di comando sovrascrivano tutto
        CoveragePreferences.instance.clearAll(); 
        
        // Impostiamo i filtri (solo la tua cartella THESIS)
        CodeCoverage.VerbosityLevel = CoverageVerbosityLevel.Verbose;
        
        // 2. Apriamo la scena
        string scenePath = "Assets/_Tanks/Tutorial_Demo/Demo_Scenes/ML-Agents-Scene.unity";
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);

        // 3. Facciamo partire il Recorder PRIMA del Playmode
        Debug.Log("[Tesi] Avvio manuale del Recorder...");
        CodeCoverage.StartRecording();

        // 4. Entriamo in Play
        EditorApplication.isPlaying = true;

        // Ci sottoscriviamo allo stop per salvare
        EditorApplication.playModeStateChanged += OnPlayStateChanged;
    }

    private static void OnPlayStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("[Tesi] Playmode terminato. Salvataggio forzato dei dati...");
            CodeCoverage.StopRecording();
            CodeCoverage.SaveRecording();
            
            // Genera anche il report HTML se vuoi vederlo subito
            CodeCoverage.GenerateReport(); 
        }
    }
}*/