using System.Collections.Generic;
using UnityEngine;

namespace Tanks.Complete
{
    /// <summary>
    /// Classe intermedia per tutti gli oracoli dei power-up (event-driven).
    /// Si registra a OnPowerUpApplied di ogni PowerUpDetectorML.
    /// Ogni figlio controlla UN SOLO tipo di power-up.
    /// </summary>
    public abstract class PowerUpOracleBase : GameLogicOracleBase
    {
        protected abstract PowerUpML.PowerUpType TargetPowerUpType { get; }

        /// <summary>
        /// Asserzione specifica: il figlio controlla se l'effetto è corretto.
        /// Chiamata immediatamente dopo il pickup.
        /// </summary>
        protected abstract void AssertPowerUpEffect(int tankIndex, TankSnapshot before);

        public struct TankSnapshot
        {
            public float health;
            public float speed;
            public float turnSpeed;
            public float shotCooldown;
            public bool hasShield;
        }

        private List<TankSnapshot> snapshots = new List<TankSnapshot>();

        protected override void RegisterEvents()
        {
            for (int i = 0; i < TankCount; i++)
            {
                snapshots.Add(CaptureSnapshot(i));

                if (powerUps[i] != null)
                {
                    int idx = i;
                    powerUps[i].OnPowerUpApplied += (type) => OnPickup(idx, type);
                }

                // Aggiorna snapshot quando cambia la vita (per Healing)
                if (healths[i] != null)
                {
                    int idx = i;
                    healths[i].OnHealthChanged += () => UpdateSnapshot(idx);
                }

                // Aggiorna snapshot quando il tank si muove (per Speed)
                /*
                if (movements[i] != null)
                {
                    int idx = i;
                    movements[i].OnMovementUpdated += () => UpdateSnapshot(idx);
                }
                */
            }
        }

        private void UpdateSnapshot(int index)
        {
            if (index < snapshots.Count)
                snapshots[index] = CaptureSnapshot(index);
        }

        private void OnPickup(int tankIndex, PowerUpML.PowerUpType type)
        {
            if (type != TargetPowerUpType) return;
            if (!IsTankActive(tankIndex)) return;

            TankSnapshot before = snapshots[tankIndex];
            AssertPowerUpEffect(tankIndex, before);
        }

        private TankSnapshot CaptureSnapshot(int index)
        {
            var snap = new TankSnapshot();
            if (index < healths.Count && healths[index] != null)
            {
                snap.health = healths[index].m_CurrentHealth;
                snap.hasShield = healths[index].m_HasShield;
            }
            if (index < movements.Count && movements[index] != null)
            {
                snap.speed = movements[index].m_Speed;
                snap.turnSpeed = movements[index].m_TurnSpeed;
            }
            if (index < shootings.Count && shootings[index] != null)
            {
                snap.shotCooldown = shootings[index].m_ShotCooldown;
            }
            return snap;
        }
    }
}