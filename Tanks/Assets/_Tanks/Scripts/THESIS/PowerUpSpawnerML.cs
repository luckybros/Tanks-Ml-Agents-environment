using System.Collections;
using UnityEngine;

namespace Tanks.Complete
{
    public class PowerUpSpawnerML : MonoBehaviour
    {
        [Tooltip("Array that holds different power-up prefabs that can be spawned.")]
        public PowerUpML[] m_PowerUps;
        [Tooltip("Time in seconds that will wait this spawner to instantiate a new power up when collected the new one.")]
        public float m_RespawnCooldown = 20f;

        public bool m_HasActivePowerUp = false;
        public PowerUpML.PowerUpType m_PowerUpType;

        private GameObject currentPowerup;


        private void Start()
        {
            m_HasActivePowerUp = true;
            // Spawn a random power up when the game starts.
            SpawnRandomPowerUp();
        }


        private void SpawnRandomPowerUp()
        {
            // Ensure there are power ups available to spawn.
            if (m_PowerUps.Length > 0)
            {
                // Instantiates a random power up from the power ups array
                if (currentPowerup != null)
                    Destroy(currentPowerup);
                int randomNumber = Random.Range(0, m_PowerUps.Length);
                Vector3 positionToSpawn = transform.position;
                positionToSpawn.y = 1.09f;
                PowerUpML m_SpawnedPowerup = Instantiate(m_PowerUps[randomNumber], positionToSpawn, Quaternion.identity);
                m_PowerUpType = m_SpawnedPowerup.GetPowerUpType();  // added for state coverage
                m_SpawnedPowerup.SetSpawner(this);
                currentPowerup = m_SpawnedPowerup.gameObject;
            }
        }

        // Called when a power up is collected, starting a respawn timer.
        public void CollectPowerUp()
        {
            m_HasActivePowerUp = false;
            StartCoroutine(RespawnPowerUp());
        }

        private IEnumerator RespawnPowerUp()
        {
            // Wait for the cooldown time then spawns a power up.
            yield return new WaitForSeconds(m_RespawnCooldown);
            SpawnRandomPowerUp();
            m_HasActivePowerUp = true;
        }

        public void EndEpisode()
        {
            SpawnRandomPowerUp();
        }
    }
}