namespace Tanks.Complete
{
    public class HealingPowerUpOracle : PowerUpOracleBase
    {
        protected override string OracleName => "HEALING POWERUP ORACLE";
        protected override PowerUpML.PowerUpType TargetPowerUpType => PowerUpML.PowerUpType.Healing;

        protected override void AssertPowerUpEffect(int tankIndex, TankSnapshot before)
        {
            var health = healths[tankIndex];
            if (health == null) return;

            if (before.health >= health.m_StartingHealth) return;

            if (health.m_CurrentHealth <= before.health)
            {
                ReportBug($"powerup_healing_t{tankIndex}",
                    $"Tank{tankIndex}: Healing raccolto ma vita non aumentata! " +
                    $"Prima: {before.health:F2}, Dopo: {health.m_CurrentHealth:F2}, " +
                    $"Max: {health.m_StartingHealth}.");
            }
        }
    }
}