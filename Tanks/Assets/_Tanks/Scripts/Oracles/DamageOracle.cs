using UnityEngine;

namespace Tanks.Complete
{
    /// <summary>
    /// Detects Bug 8: damage not applied when tank health is <= 10%
    /// Subscribe to ShellExplosionML.OnEnemyHit
    /// When an explosion hits a tank saves health before
    /// </summary>
    public class DamageOracle : OracleBase
    {
        protected override string OracleName => "DAMAGE ORACLE";

        private void OnEnable()
        {
            ShellExplosionML.OnEnemyHit += OnEnemyHit;
        }

        private void OnDisable()
        {
            ShellExplosionML.OnEnemyHit -= OnEnemyHit;
        }

        private void OnEnemyHit(float healthBefore, float healthAfter, float damage)
        {
            if (damage <= 0f) return;

            if (healthAfter >= healthBefore)
            {
                Debug.Log($"[LOGIC BUG: Before: {healthBefore:F2}, After: {healthAfter:F2}]");
                ReportBug(
                    "damage_not_applied",
                    $"Explosion hit tank but health didn't decrease! " +
                    $"Before: {healthBefore:F2}, After: {healthAfter:F2}, " +
                    $"Damage: {damage:F2}."
                );        
            }
        }
    }
}