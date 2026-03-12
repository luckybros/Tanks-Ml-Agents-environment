using UnityEngine;

namespace Tanks.Complete
{
    public class EpisodeManager : MonoBehaviour
    {
        public TankAgent[] agents;
        public PowerUpSpawnerML[] powerUpSpawners;

        public void EndEpisode()
        {
            foreach (TankAgent agent in agents)
            {
                agent.EndEpisode();
            }

            foreach (PowerUpSpawnerML spawner in powerUpSpawners)
            {
                spawner.EndEpisode();
            }
        }
    }
}