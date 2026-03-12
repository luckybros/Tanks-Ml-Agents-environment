using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Collections;

namespace Tanks.Complete
{
    /*
    public class Sabotatore : MonoBehaviour
    {
        OracleSideChannel sideChannel;

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
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= sideChannel.SendDebugStatementToPython;
            if (Academy.IsInitialized)
            {
                SideChannelManager.UnregisterSideChannel(sideChannel);
            }
        }

        void Update()
        {
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
    */
}
