using UnityEngine;
using System.Collections.Generic;

namespace Tanks.Complete
{
    /// <summary>
    /// Classe base astratta per tutti gli oracoli della logica di gioco.
    /// Tutti gli oracoli sono event-driven: nessun polling.
    /// Supporta un numero arbitrario di tank.
    /// </summary>
    public abstract class GameLogicOracleBase : OracleBase
    {
        [Header("Game Logic Oracle Settings")]
        [Tooltip("Tanks list")]
        public List<GameObject> tanks = new List<GameObject>();

        // Cached component for every tank
        protected List<TankHealthML> healths = new List<TankHealthML>();
        protected List<TankMovementML> movements = new List<TankMovementML>();
        protected List<TankShootingML> shootings = new List<TankShootingML>();
        protected List<PowerUpDetectorML> powerUps = new List<PowerUpDetectorML>();
        protected List<Rigidbody> rigidbodies = new List<Rigidbody>();

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
    }
}