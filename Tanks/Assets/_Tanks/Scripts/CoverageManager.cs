using UnityEngine;
using System.Collections.Generic;
using Unity.MLAgents;

namespace Tanks.Complete
{
    public class CoverageManager : MonoBehaviour
    {
        public float cellSize = 3.0f;
        
        [Header("Tank References")]
        [Tooltip("Reference to Tank T1")]
        public GameObject tank1;

        [Tooltip("Reference to Tank T2")]
        public GameObject tank2;
        
        [Header("Power-Up References")]
        public PowerUpSpawnerML powerUpSpawner1;
        public PowerUpSpawnerML powerUpSpawner2;
        public PowerUpSpawnerML powerUpSpawner3;
        public PowerUpSpawnerML powerUpSpawner4;
        // add another character and power ups
        
        private TankMovementML tank1Movement;
        private TankShootingML tank1Shooting;
        private TankHealthML tank1Health;
        private Rigidbody tank1Rb;
        private PowerUpDetectorML tank1Detector;
        
        private TankMovementML tank2Movement;
        private TankShootingML tank2Shooting;
        private TankHealthML tank2Health;
        private Rigidbody tank2Rb;
        private PowerUpDetectorML tank2Detector;

        private HashSet<string> visitedStates = new HashSet<string>();

        private bool isFirstFrame = true;
        private GlobalGameState lastState;

        private StatsRecorder statsRecorder;
        
        public GlobalGameState GetCurrentState()
        {
            return lastState;
        }
        
        private void Awake()
        {
            InitializeTankComponents();
            statsRecorder = Academy.Instance.StatsRecorder;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // calcolo posizione
            // calcolo se sta sparando o meno
            // calcolo vita
            // calcolo rotazione
            // ottengo TankState
            GlobalGameState currentState = CaptureGlobalGameState();
            // se è uguale al precedente (non ho cambiamento) oppure è il primo frame vado avanti
            if (!isFirstFrame && currentState.Equals(lastState))
            {
                return;
            }
            // salvo lastState con quello ottenuto
            lastState = currentState;
            if (isFirstFrame) isFirstFrame = false;
        
            // vedo se è presente nell'hashmap, se non è presente lo aggiungo
            if (!visitedStates.Contains(lastState.ToString()))
            {
                visitedStates.Add(lastState.ToString());

                statsRecorder.Add($"Coverage/UniqueStates/{System.Diagnostics.Process.GetCurrentProcess().Id}", visitedStates.Count);
            }
        }

        private GlobalGameState CaptureGlobalGameState()
        {
            TankState t1 = new TankState(
                tank1.transform,
                tank1Rb,
                tank1Health.m_CurrentHealth,
                tank1Shooting.m_CanShoot, 
                tank1Shooting.m_ShotCooldownTimer,
                tank1Detector.m_PowerUpType,
                cellSize
            );

            TankState t2 = new TankState(
                tank2.transform,
                tank2Rb,
                tank2Health.m_CurrentHealth,
                tank2Shooting.m_CanShoot, 
                tank2Shooting.m_ShotCooldownTimer,
                tank2Detector.m_PowerUpType,
                cellSize
            );

            PowerUpState p1 = powerUpSpawner1.m_HasActivePowerUp ? 
                new PowerUpState(powerUpSpawner1.transform,
                                powerUpSpawner1.m_PowerUpType,
                                cellSize) :
                new PowerUpState(powerUpSpawner1.transform, cellSize);
            PowerUpState p2 = powerUpSpawner2.m_HasActivePowerUp ? 
                new PowerUpState(powerUpSpawner2.transform,
                                powerUpSpawner2.m_PowerUpType,
                                cellSize) :
                new PowerUpState(powerUpSpawner2.transform, cellSize);
            PowerUpState p3 = powerUpSpawner3.m_HasActivePowerUp ? 
                new PowerUpState(powerUpSpawner3.transform,
                                powerUpSpawner3.m_PowerUpType,
                                cellSize) :
                new PowerUpState(powerUpSpawner3.transform, cellSize);
            PowerUpState p4 = powerUpSpawner4.m_HasActivePowerUp ? 
                new PowerUpState(powerUpSpawner4.transform,
                                powerUpSpawner4.m_PowerUpType,
                                cellSize) :
                new PowerUpState(powerUpSpawner4.transform, cellSize);

            return new GlobalGameState(t1, t2, p1, p2, p3, p4);
        }

        private void InitializeTankComponents()
        {
            tank1Movement = tank1.GetComponent<TankMovementML>();
            tank1Shooting = tank1.GetComponent<TankShootingML>();
            tank1Health = tank1.GetComponent<TankHealthML>();
            tank1Rb = tank1.GetComponent<Rigidbody>();
            tank1Detector = tank1.GetComponent<PowerUpDetectorML>();
                
            tank2Movement = tank2.GetComponent<TankMovementML>();
            tank2Shooting = tank2.GetComponent<TankShootingML>();
            tank2Health = tank2.GetComponent<TankHealthML>();
            tank2Rb = tank2.GetComponent<Rigidbody>();
            tank2Detector = tank2.GetComponent<PowerUpDetectorML>();
        }
    }
}