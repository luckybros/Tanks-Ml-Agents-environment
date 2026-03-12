#if DISATTIVATO
namespace Tanks.Complete
{
    public class ShieldPowerUpOracle : PowerUpOracleBase
    {
        protected override string OracleName => "SHIELD POWERUP ORACLE";
        protected override PowerUpML.PowerUpType TargetPowerUpType => PowerUpML.PowerUpType.DamageReduction;

        protected override void AssertPowerUpEffect(int tankIndex, TankSnapshot before)
        {
            var health = healths[tankIndex];
            if (health == null) return;

            if (!health.m_HasShield)
            {
                ReportBug($"powerup_shield_t{tankIndex}",
                    $"Tank{tankIndex}: Shield raccolto ma m_HasShield è false!");
            }
        }
    }
}
#endif