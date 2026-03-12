#if DISATTIVATO
namespace Tanks.Complete
{
    public class DamageMultiplierPowerUpOracle : PowerUpOracleBase
    {
        protected override string OracleName => "DAMAGE MULTIPLIER POWERUP ORACLE";
        protected override PowerUpML.PowerUpType TargetPowerUpType => PowerUpML.PowerUpType.DamageMultiplier;

        protected override void AssertPowerUpEffect(int tankIndex, TankSnapshot before)
        {
            var powerUp = powerUps[tankIndex];
            if (powerUp == null) return;

            if (!powerUp.m_HasActivePowerUp)
            {
                ReportBug($"powerup_damage_multiplier_t{tankIndex}",
                    $"Tank{tankIndex}: DamageMultiplier raccolto ma m_HasActivePowerUp è false!");
            }
        }
    }
}
#endif