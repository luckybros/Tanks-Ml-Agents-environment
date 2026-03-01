namespace Tanks.Complete
{
    public class InvincibilityPowerUpOracle : PowerUpOracleBase
    {
        protected override string OracleName => "INVINCIBILITY POWERUP ORACLE";
        protected override PowerUpML.PowerUpType TargetPowerUpType => PowerUpML.PowerUpType.Invincibility;

        protected override void AssertPowerUpEffect(int tankIndex, TankSnapshot before)
        {
            var powerUp = powerUps[tankIndex];
            if (powerUp == null) return;

            if (!powerUp.m_HasActivePowerUp)
            {
                ReportBug($"powerup_invincibility_t{tankIndex}",
                    $"Tank{tankIndex}: Invincibility raccolto ma m_HasActivePowerUp è false!");
            }
        }
    }
}