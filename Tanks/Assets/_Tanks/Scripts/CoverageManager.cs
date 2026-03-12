using UnityEngine;
using System.Collections.Generic;
using Unity.MLAgents;

namespace Tanks.Complete
{
    public class CoverageManager : MonoBehaviour
    {
        public float cellSize = 5.0f;
        
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

        private HashSet<TankState> visitedStates= new HashSet<TankState>();
        //private HashSet<TankState> visitedStatesT2 = new HashSet<TankState>();
        private HashSet<GlobalGameState> visitedGlobalStates = new HashSet<GlobalGameState>();

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
            // dovrei calcolare un gamestate per coppia di tanks
            GlobalGameState currentState = CaptureGlobalGameState();
            
            if (!isFirstFrame && currentState.Equals(lastState))
                return;

            lastState = currentState;
            if (isFirstFrame) isFirstFrame = false;
            
            // ora fanno parte di un unico unique states
            bool newT1 = visitedStates.Add(lastState.t1);
            bool newT2 = visitedStates.Add(lastState.t2);
            bool newGlobal = visitedGlobalStates.Add(lastState);

            //Debug.Log($"[STATE] {lastState.ToCompactString()} | newT1={newT1} newT2={newT2} newGlobal={newGlobal}");

            if (newT1) statsRecorder.Add("Coverage/UniqueStates", visitedStates.Count);
            //if (newT2) statsRecorder.Add("Coverage/UniqueStates_T2", visitedStatesT2.Count);
            if (newGlobal) statsRecorder.Add("Coverage/GlobalUniqueStates", visitedGlobalStates.Count);
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
                cellSize,
                tank1Shooting.HasProjectileInAir
            );

            TankState t2 = new TankState(
                tank2.transform,
                tank2Rb,
                tank2Health.m_CurrentHealth,
                tank2Shooting.m_CanShoot, 
                tank2Shooting.m_ShotCooldownTimer,
                tank2Detector.m_PowerUpType,
                cellSize,
                tank2Shooting.HasProjectileInAir
            );

            /*
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

            */
            return new GlobalGameState(t1, t2);
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