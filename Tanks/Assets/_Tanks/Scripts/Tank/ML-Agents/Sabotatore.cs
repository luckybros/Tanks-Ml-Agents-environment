using UnityEngine;
using System;

namespace Tanks.Complete
{
    public class Sabotatore : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("--- ATTIVAZIONE HANG: Unity entrerà in un loop ininito");
                while (true)
                {

                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.LogError("--- ATTIVAZIONE CRASH: Forzatura eccezione");
                throw new Exception("Simulated exception for Python detection");
            }
        }
    }
}
