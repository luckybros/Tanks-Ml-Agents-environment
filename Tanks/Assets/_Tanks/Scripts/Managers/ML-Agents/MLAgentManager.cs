using UnityEngine;

namespace Tanks.Complete
{
    public class MLAgentManager : MonoBehaviour
    {
        [Header("Tank References")]
        public TankHealthML m_Tank1;
        public TankHealthML m_Tank2;
        
        [Header("Spawn Points")]
        public Transform m_SpawnPoint1;
        public Transform m_SpawnPoint2;
        
        [Header("Training Settings")]
        public float m_ResetDelay = 0.5f;
        
        [Header("Camera")]
        public CameraControl m_CameraControl;
        
        private Vector3 m_Tank1StartPos;
        private Quaternion m_Tank1StartRot;
        private Vector3 m_Tank2StartPos;
        private Quaternion m_Tank2StartRot;
        
        private bool m_ResettingRound = false;
        
        private void Start()
        {
            SaveStartPositions();
            // SetupCamera();
            ResetBothTanks();
        }
        
        private void SaveStartPositions()
        {
            m_Tank1StartPos = m_SpawnPoint1.position;
            m_Tank1StartRot = m_SpawnPoint1.rotation;

            m_Tank1StartPos = m_Tank1.transform.position;
            m_Tank1StartRot = m_Tank1.transform.rotation;
                
            m_Tank2StartPos = m_SpawnPoint2.position;
            m_Tank2StartRot = m_SpawnPoint2.rotation;
            
            m_Tank2StartPos = m_Tank2.transform.position;
            m_Tank2StartRot = m_Tank2.transform.rotation;
        }
        
        private void SetupCamera()
        {
            Transform[] targets = new Transform[2];
            targets[0] = m_Tank1.transform;
            targets[1] = m_Tank2.transform;
            m_CameraControl.m_Targets = targets;
        }
        
        private void Update()
        {
            CheckForDeath();
        }
        
        private void CheckForDeath()
        {
            if (m_ResettingRound) return;
            
            bool tank1Dead = m_Tank1 != null && !m_Tank1.gameObject.activeSelf;
            bool tank2Dead = m_Tank2 != null && !m_Tank2.gameObject.activeSelf;
            
            if (tank1Dead || tank2Dead)
            {
                m_ResettingRound = true;
                Invoke(nameof(ResetBothTanks), m_ResetDelay);
            }
        }
        
        private void ResetBothTanks()
        {
            ResetTank(m_Tank1, m_Tank1StartPos, m_Tank1StartRot);
            ResetTank(m_Tank2, m_Tank2StartPos, m_Tank2StartRot);
            
            m_ResettingRound = false;
        }
        
        private void ResetTank(TankHealthML tank, Vector3 position, Quaternion rotation)
        {
            if (tank == null) return;
            
            tank.transform.position = position;
            tank.transform.rotation = rotation;
            
            var rb = tank.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            tank.gameObject.SetActive(false);
            tank.gameObject.SetActive(true);
        }
    }
}
