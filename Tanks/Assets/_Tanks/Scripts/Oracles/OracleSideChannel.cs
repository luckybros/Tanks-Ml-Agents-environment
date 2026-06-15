using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Text;
using System;

namespace Tanks.Complete
{
    public class OracleSideChannel : SideChannel
    {
        public static event Action OnResetReceived;
        public OracleSideChannel()
        {
            ChannelId = new Guid("621f0a70-4f87-11ea-a6bf-784f4387d1f7");
        }

        protected override void OnMessageReceived(IncomingMessage msg)
        {
            var receivedString = msg.ReadString();
            
            if (receivedString == "RESET")
            {
                Debug.Log("Comando di reset ricevuto da Python tramite SideChannel.");
                OnResetReceived?.Invoke();
            }
            Debug.Log("From Python : " + receivedString);
        }

        public void SendStringToPython(string msg)
        {
            // Debug.Log($"Sending to Python {msg}");
            var stringToSend = msg;
            using (var msgOut = new OutgoingMessage())
            {
                msgOut.WriteString(stringToSend);
                QueueMessageToSend(msgOut);
            }
        }
    }
}