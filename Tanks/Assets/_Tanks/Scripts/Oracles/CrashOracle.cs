using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Collections;

namespace Tanks.Complete
{
    public class CrashOracle : MonoBehaviour
    {
        OracleSideChannel sideChannel;
        int counter = 0;
        float timer = 0f;
        float interval = 1.0f;

        public void Start()
        {
            StartCoroutine(InitializeWhenReady());
        }

        private IEnumerator InitializeWhenReady()
        {
            while (!Academy.IsInitialized)
            {
                Debug.Log("[CrashOracle] Waiting for Academy to initialize...");
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;

            sideChannel = new OracleSideChannel();
            
            SideChannelManager.RegisterSideChannel(sideChannel);
            // When a Debug.Log message is created, we send it to the channel
            Application.logMessageReceived += sideChannel.SendDebugStatementToPython;

            sideChannel.SendStringToPython("START");
        }

        public void OnDestroy()
        {
            Application.logMessageReceived -= sideChannel.SendDebugStatementToPython;
            if (Academy.IsInitialized)
            {
                SideChannelManager.UnregisterSideChannel(sideChannel);
            }
        }
        // Update is called once per frame
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
    }
}