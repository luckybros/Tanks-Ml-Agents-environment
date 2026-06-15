using UnityEngine;

namespace Tanks.Complete
{
    public class StartMatchNoPowerUpOracle : OracleBase
    {
        protected override string OracleName => "START MATCH NO POWERUP ORACLE";

        private void OnEnable()
        {
            EpisodeManager.OnEpisodeBegin += OnEpisodeBeginCheck;
        }

        private void OnDisable()
        {
            EpisodeManager.OnEpisodeBegin -= OnEpisodeBeginCheck;
        }
        
        private void OnEpisodeBeginCheck(TankAgent agent1, TankAgent agent2)
        {
            if (agent1.GetComponent<PowerUpDetectorML>().m_HasActivePowerUp ||
                agent2.GetComponent<PowerUpDetectorML>().m_HasActivePowerUp)
            {
                ReportBug("start_match_no_powerup",
                    $"Match started but a tank has already an active powerup! " +
                    $"Tank0: {agent1.GetComponent<PowerUpDetectorML>().m_HasActivePowerUp}, " +
                    $"Tank1: {agent2.GetComponent<PowerUpDetectorML>().m_HasActivePowerUp}");
            }
        }
    }
}