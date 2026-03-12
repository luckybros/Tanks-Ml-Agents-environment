#if DISATTIVATO
namespace Tanks.Complete
{
    public class SpeedPowerUpOracle : PowerUpOracleBase
    {
        protected override string OracleName => "SPEED POWERUP ORACLE";
        protected override PowerUpML.PowerUpType TargetPowerUpType => PowerUpML.PowerUpType.Speed;

        protected override void AssertPowerUpEffect(int tankIndex, TankSnapshot before)
        {
            var movement = movements[tankIndex];
            if (movement == null) return;

            if (movement.m_Speed <= before.speed && movement.m_TurnSpeed <= before.turnSpeed)
            {
                ReportBug($"powerup_speed_t{tankIndex}",
                    $"Tank{tankIndex}: Speed PowerUp raccolto ma nessun aumento! " +
                    $"m_Speed prima: {before.speed}, dopo: {movement.m_Speed}. " +
                    $"m_TurnSpeed prima: {before.turnSpeed}, dopo: {movement.m_TurnSpeed}.");
            }
        }
    }
}
#endif