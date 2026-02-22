using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;

namespace Tanks.Complete
{
    public class CrashOracle : MonoBehaviour
    {
        OracleSideChannel sideChannel;
        int counter = 0;
        float timer = 0f;
        float interval = 1.0f;

        public void Awake()
        {
            sideChannel = new OracleSideChannel();
            
            // When a Debug.Log message is created, we send it to the channel
            Application.logMessageReceived += sideChannel.SendDebugStatementToPython;

            // The channel must be registered with the SideChannelManger class
            SideChannelManager.RegisterSideChannel(sideChannel);
        }

        public void Start()
        {
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
        }
    }
}