using UnityEngine;

namespace Tanks.Complete
{
    /// <summary>
    /// Detects Bug 7: Healing causes health to exceed maximum.
    /// Subscribes to PowerUpDetectorML.OnHealingApplied (static event).
    /// Receives health before, health after, and heal amount.
    /// Just checks if the numbers make sense.
    /// </summary>
    public class HealingPowerUpOracle : OracleBase
    {
        protected override string OracleName => "HEALING POWERUP ORACLE";

        private float maxHealth = 100f;

        private void OnEnable()
        {
            PowerUpDetectorML.OnHealingApplied += OnHealingApplied;
        }

        private void OnDisable()
        {
            PowerUpDetectorML.OnHealingApplied -= OnHealingApplied;
        }

        private void OnHealingApplied(float healthBefore, float healthAfter, float healAmount)
        {
            if (healAmount <= 0f) return;

            // Check 1: health should have increased (unless already full)
            if (healthBefore < maxHealth && healthAfter <= healthBefore)
            {
                ReportBug("healing_no_effect",
                    $"Healing picked up but health didn't increase! " +
                    $"Before: {healthBefore:F2}, After: {healthAfter:F2}, " +
                    $"HealAmount: {healAmount:F2}.");
            }

            // Check 2: health should NEVER exceed the maximum
            if (healthAfter > maxHealth + 0.01f)
            {
                Debug.Log($"[LOGIC BUG DETECTED]: Before: {healthBefore:F2}, After: {healthAfter:F2}");
                ReportBug("healing_over_max",
                    $"Healing caused health to exceed maximum! " +
                    $"Before: {healthBefore:F2}, After: {healthAfter:F2}, " +
                    $"HealAmount: {healAmount:F2}, Max: {maxHealth}.");
            }
        }
    }
}