namespace Tanks.Complete
{
    public class ShootingBonusPowerUpOracle : PowerUpOracleBase
    {
        protected override string OracleName => "SHOOTING BONUS POWERUP ORACLE";
        protected override PowerUpML.PowerUpType TargetPowerUpType => PowerUpML.PowerUpType.ShootingBonus;

        protected override void AssertPowerUpEffect(int tankIndex, TankSnapshot before)
        {
            var shooting = shootings[tankIndex];
            if (shooting == null) return;

            if (before.shotCooldown > 0 && shooting.m_ShotCooldown >= before.shotCooldown)
            {
                ReportBug($"powerup_shooting_t{tankIndex}",
                    $"Tank{tankIndex}: ShootingBonus raccolto ma cooldown non diminuito! " +
                    $"Prima: {before.shotCooldown:F3}, Dopo: {shooting.m_ShotCooldown:F3}.");
            }
        }
    }
}