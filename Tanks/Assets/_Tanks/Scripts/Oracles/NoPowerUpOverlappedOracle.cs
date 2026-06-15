using UnityEngine;

namespace Tanks.Complete
{
    public class NoPowerUpOverlappedOracle : OracleBase
    {
        protected override string OracleName => "NO POWER UP OVERLAPPED ORACLE";

        private void OnEnable()
        {
            PowerUpML.OnPowerUpCollected += OnPowerUpCollected;
        }

        private void OnDisable()
        {
            PowerUpML.OnPowerUpCollected -= OnPowerUpCollected;
        }
        
        private void OnPowerUpCollected(bool hasPowerUpBeforeCollection, PowerUpML.PowerUpType typeBefore, PowerUpML.PowerUpType typeAfter)
        {
            if (hasPowerUpBeforeCollection)
            {
                ReportBug("no_powerup_overlapped",
                    $"PowerUp collected but tank already had an active powerup! " +
                    $"hasPowerUpBeforeCollection: {hasPowerUpBeforeCollection}" +
                    $" typeBefore: {typeBefore}, typeAfter: {typeAfter}");
            }
        }
    }
}