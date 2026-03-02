namespace Tanks.Complete
{
    /// <summary>
    /// Detects Bug 7: Healing power-up causes health to exceed maximum.
    /// Tracks health before and after healing pickup.
    /// Subscribes to both OnHealthChanged and OnPowerUpApplied.
    /// </summary>
    public class HealingPowerUpOracle : GameLogicOracleBase
    {
        protected override string OracleName => "HEALING POWERUP ORACLE";

        // Per-tank: previous health value (before the last health change)
        private float[] previousHealth;
        // Per-tank: current health value (after the last health change)
        private float[] currentHealth;

        protected override void RegisterEvents()
        {
            previousHealth = new float[TankCount];
            currentHealth = new float[TankCount];

            for (int i = 0; i < TankCount; i++)
            {
                if (healths[i] != null)
                {
                    previousHealth[i] = healths[i].m_CurrentHealth;
                    currentHealth[i] = healths[i].m_CurrentHealth;

                    int idx = i;
                    // Every time health changes, shift the values
                    healths[i].OnHealthChanged += () => OnHealthChanged(idx);
                }

                if (powerUps[i] != null)
                {
                    int idx = i;
                    // When Healing is picked up, do the assertions
                    powerUps[i].OnPowerUpApplied += (type) => OnPowerUpPickup(idx, type);
                }
            }
        }

        private void OnHealthChanged(int index)
        {
            if (!IsTankActive(index)) return;
            if (healths[index] == null) return;

            // Shift: current becomes previous, new value becomes current
            previousHealth[index] = currentHealth[index];
            currentHealth[index] = healths[index].m_CurrentHealth;
        }

        private void OnPowerUpPickup(int index, PowerUpML.PowerUpType type)
        {
            if (type != PowerUpML.PowerUpType.Healing) return;
            if (!IsTankActive(index)) return;
            if (healths[index] == null) return;

            float before = previousHealth[index];
            float after = healths[index].m_CurrentHealth;
            float max = healths[index].m_StartingHealth;

            // Check 1: health should have increased (unless it was already full)
            if (before < max && after <= before)
            {
                ReportBug($"healing_no_effect_t{index}",
                    $"Tank{index}: Healing picked up but health didn't increase! " +
                    $"Before: {before:F2}, After: {after:F2}, Max: {max}.");
            }

            // Check 2: health should NEVER exceed the maximum
            if (after > max + 0.01f)
            {
                ReportBug($"healing_over_max_t{index}",
                    $"Tank{index}: Healing caused health to exceed maximum! " +
                    $"Before: {before:F2}, After: {after:F2}, Max: {max}.");
            }
        }
    }
}