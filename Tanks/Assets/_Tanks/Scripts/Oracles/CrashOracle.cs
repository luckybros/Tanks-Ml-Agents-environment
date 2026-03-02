using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Collections;

namespace Tanks.Complete
{
    /// <summary>
    /// Captures errors and Unity exception (Debug.LogError, Exception)
    /// and send them to Python as "ERROR:...".
    /// </summary>
    public class CrashOracle : OracleBase
    {
        protected override string OracleName => "CRASH ORACLE";

        private void OnEnable()
        {
            // Captures Debug.LogError and exception
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                if (type == LogType.Error)
                {
                    ReportBug("error", logString + " : " + stackTrace);
                }
                else if (type == LogType.Exception)
                {
                    ReportBug("exception", logString + " : " + stackTrace);
                }
            }
        }
        
        /*
        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= interval)
            {
                counter++;
                sideChannel.SendStringToPython($"ALIVE {counter}");
                timer = 0f;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("--- Crash without logging");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("Hang infinite loop");
                while (true) {}
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                // Debug.LogError("--- ATTIVAZIONE CRASH: Forzatura eccezione");
                Debug.LogError("Simulated crash for testing");
                Invoke("KillProcess", 2f);
            }
        }

        void KillProcess()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        */
    }
}