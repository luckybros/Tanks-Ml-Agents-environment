using UnityEngine;
using System.Collections.Generic;
using Unity.MLAgents;
using System.IO;

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

        private static HashSet<TankState> visitedStates= new HashSet<TankState>();
        //private HashSet<TankState> visitedStatesT2 = new HashSet<TankState>();
        private static HashSet<GlobalGameState> visitedGlobalStates = new HashSet<GlobalGameState>();

        private bool isFirstFrame = true;
        private GlobalGameState lastState;

        private StatsRecorder statsRecorder;

        private static readonly object fileLock = new object(); // Lucchetto per evitare collisioni tra le arene
        private static string tankStatesPath;
        private static string globalStatesPath;
        private static bool isLoaded = false;
        
        private void Awake()
        {

            //#if !UNITY_EDITOR
            //Debug.unityLogger.logEnabled = false;
            //#endif

            InitializeTankComponents();
            statsRecorder = Academy.Instance.StatsRecorder;

            // Salviamo i file un livello sopra la cartella Assets, vicini alla cartella 'results' di mlagents
            tankStatesPath = Path.Combine(Application.persistentDataPath, "./TankStatesLog.txt");
            globalStatesPath = Path.Combine(Application.persistentDataPath, "./GlobalStatesLog.txt");

            // Debug.Log($"Path: {tankStatesPath}");
            // Il lock impedisce alle 4 arene di provare a caricare i file in simultanea
            lock (fileLock) 
            {
                if (!isLoaded)
                {
                    LoadStatesFromDisk();
                    isLoaded = true;
                }
            }
        }

        private void LoadStatesFromDisk()
        {
            if (File.Exists(tankStatesPath))
            {
                foreach (string line in File.ReadLines(tankStatesPath))
                {
                    if (!string.IsNullOrWhiteSpace(line)) visitedStates.Add(TankState.Parse(line));
                }
            }

            if (File.Exists(globalStatesPath))
            {
                foreach (string line in File.ReadLines(globalStatesPath))
                {
                    if (!string.IsNullOrWhiteSpace(line)) visitedGlobalStates.Add(GlobalGameState.Parse(line));
                }
            }
            
            // Debug.Log($"[COVERAGE] Ripristinati {visitedStates.Count} stati singoli e {visitedGlobalStates.Count} stati globali dal disco.");
        }

        public GlobalGameState GetCurrentState()
        {
            return lastState;
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

            if (newT1 || newT2 || newGlobal)
            {
                lock (fileLock)
                {
                    // AppendAllText apre il file, va all'ultima riga, scrive e lo chiude istantaneamente
                    if (newT1) File.AppendAllText(tankStatesPath, lastState.t1.ToString() + "\n");
                    if (newT2) File.AppendAllText(tankStatesPath, lastState.t2.ToString() + "\n");
                    if (newGlobal) File.AppendAllText(globalStatesPath, lastState.ToCompactString() + "\n");
                }
            }

            if (newT1 || newT2) statsRecorder.Add("Coverage/UniqueStates", visitedStates.Count);
            // if (newT2) statsRecorder.Add("Coverage/UniqueStates", visitedStates.Count);
            if (newGlobal) statsRecorder.Add("Coverage/GlobalUniqueStates", visitedGlobalStates.Count);

            if (Time.frameCount % 100 == 0)
            {
                int envSteps = Academy.Instance.StepCount;
                int agentSteps = tank1.GetComponent<TankAgent>().StepCount + tank2.GetComponent<TankAgent>().StepCount;

                //Debug.Log($"[Coverage]: {envSteps}");
                //Debug.Log($"[Coverage Agent]: {agentSteps}");
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