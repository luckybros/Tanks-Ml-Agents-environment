using UnityEngine;
using System.Collections.Generic;

namespace Tanks.Complete
{
    /// <summary>
    /// Classe base astratta per tutti gli oracoli della logica di gioco.
    /// Tutti gli oracoli sono event-driven: nessun polling.
    /// Supporta un numero arbitrario di tank.
    /// </summary>
    public abstract class GameLogicOracleBase : MonoBehaviour
    {
        [Header("Base Oracle Settings")]
        [Tooltip("Riferimento al side channel per comunicare con Python")]
        public OracleSideChannel sideChannel;

        [Tooltip("Lista dei tank da monitorare")]
        public List<GameObject> tanks = new List<GameObject>();

        // Componenti cachate per ogni tank
        protected List<TankHealthML> healths = new List<TankHealthML>();
        protected List<TankMovementML> movements = new List<TankMovementML>();
        protected List<TankShootingML> shootings = new List<TankShootingML>();
        protected List<PowerUpDetectorML> powerUps = new List<PowerUpDetectorML>();
        protected List<Rigidbody> rigidbodies = new List<Rigidbody>();

        // Cooldown per non spammare lo stesso errore
        private Dictionary<string, float> errorTimestamps = new Dictionary<string, float>();

        [Tooltip("Secondi di cooldown prima di poter risegnalare lo stesso tipo di errore")]
        public float reportCooldown = 5f;

        /// <summary>Nome dell'oracolo, usato nei log e nel CSV.</summary>
        protected abstract string OracleName { get; }

        /// <summary>
        /// Ogni figlio si registra qui ai propri eventi.
        /// Chiamato dopo CacheComponents().
        /// </summary>
        protected abstract void RegisterEvents();

        protected virtual void Awake()
        {
            CacheComponents();
            RegisterEvents();
        }

        protected void CacheComponents()
        {
            healths.Clear();
            movements.Clear();
            shootings.Clear();
            powerUps.Clear();
            rigidbodies.Clear();

            foreach (var tank in tanks)
            {
                if (tank != null)
                {
                    healths.Add(tank.GetComponent<TankHealthML>());
                    movements.Add(tank.GetComponent<TankMovementML>());
                    shootings.Add(tank.GetComponent<TankShootingML>());
                    powerUps.Add(tank.GetComponent<PowerUpDetectorML>());
                    rigidbodies.Add(tank.GetComponent<Rigidbody>());
                }
                else
                {
                    healths.Add(null);
                    movements.Add(null);
                    shootings.Add(null);
                    powerUps.Add(null);
                    rigidbodies.Add(null);
                }
            }
        }

        protected int TankCount => tanks.Count;

        protected bool IsTankActive(int index)
        {
            return index >= 0 && index < tanks.Count &&
                   tanks[index] != null && tanks[index].activeInHierarchy;
        }

        /// <summary>
        /// Segnala un bug al SideChannel Python. Non chiude il gioco.
        /// </summary>
        protected void ReportBug(string bugType, string message)
        {
            if (errorTimestamps.ContainsKey(bugType))
            {
                if (Time.time - errorTimestamps[bugType] < reportCooldown)
                    return;
            }
            errorTimestamps[bugType] = Time.time;

            Debug.LogWarning($"[{OracleName}] {message}");

            if (sideChannel != null)
            {
                string fullMessage = $"GAME_LOGIC_BUG|{OracleName}|{bugType}|{message}";
                sideChannel.SendStringToPython(fullMessage);
            }
        }
    }
}