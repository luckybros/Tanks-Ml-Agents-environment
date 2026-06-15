using UnityEngine;
using System;
using Unity.MLAgents;

namespace Tanks.Complete
{
    public class EpisodeManager : MonoBehaviour
    {
        // l'episode manager è per env
        public TankAgent[] agents;
        public Transform[] spawnPoints;
        public PowerUpSpawnerML[] powerUpSpawners;

        public static event Action<TankAgent, TankAgent> OnEpisodeBegin;

        void OnEnable()
        {
            Academy.Instance.OnEnvironmentReset += EndEpisode;
            OracleSideChannel.OnResetReceived += EndEpisode;
        }

        void OnDisable()
        {
            if (Academy.IsInitialized)
            {
                Academy.Instance.OnEnvironmentReset -= EndEpisode;
            }
            OracleSideChannel.OnResetReceived -= EndEpisode;
        }

        void Start()
        {
            SpawnAgents();
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                EndEpisode();
            }
        }
        
        public void EndEpisode()
        {
            SpawnAgents();

            foreach (PowerUpSpawnerML spawner in powerUpSpawners)
            {
                spawner.EndEpisode();
            }

            OnEpisodeBegin?.Invoke(agents[0], agents[1]);
        }

        private void SpawnAgents()
        {
            int index1 = UnityEngine.Random.Range(0, spawnPoints.Length);
            int index2 = UnityEngine.Random.Range(0, spawnPoints.Length);

            if (index2 == index1)
            {
                index2 = spawnPoints.Length - 1 - index1;
            }

            agents[0].EndEpisode();
            agents[1].EndEpisode();

            agents[0].ResetTank(spawnPoints[index1].position, spawnPoints[index1].rotation);
            agents[1].ResetTank(spawnPoints[index2].position, spawnPoints[index2].rotation);
        }
    }
}