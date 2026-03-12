using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

namespace Tanks.Complete
{
    public class TankAgent : Agent
    {
        public int tankId;
        public enum PowerUpType { None, Speed, DamageReduction, ShootingBonus, Healing, Invincibility, DamageMultiplier };
        const int NUM_ITEM_TYPES = (int)PowerUpType.DamageMultiplier + 1;
        public Transform spawnPoint;
        public TankAgent otherAgent;

        private TankMovementML tankMovement;
        private TankShootingML tankShooting;
        private TankHealthML tankHealth;
        private Rigidbody rb;
        private PowerUpDetectorML powerUpDetector;

        private TankHealthML otherTankHealth;

        private Vector3 startPos;
        private Quaternion startRot;
        //private CSVLogger logger;
        private float[] lastState = null;
        
        [SerializeField] private float areaSize = 50f;
        [SerializeField] private float maxSpeed = 12f;
        [SerializeField] private float maxTurnSpeed = 12f;

        [SerializeField] private EpisodeManager episodeManager;
        public bool useVectorObs;

        private bool m_WasFireButtonPressed = false;

        private float maxDistance = 9f;

        public override void Initialize()
        {
            GetComponents();
            SaveStartPositions();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (useVectorObs)
            {
                foreach (float obs in GetCurrentStateArray())
                {
                    sensor.AddObservation(obs);
                }
                sensor.AddOneHotObservation((int)powerUpDetector.m_PowerUpType, NUM_ITEM_TYPES);
            }
        }

        private float[] GetCurrentStateArray()
        {
            return new float[]
            {   
                //transform.localPosition.x / areaSize, 
                //transform.localPosition.z / areaSize,
                //otherAgent.transform.localPosition.x / areaSize,
                //otherAgent.transform.localPosition.z / areaSize,
                //Mathf.Clamp(transform.InverseTransformDirection(rb.linearVelocity).z / tankMovement.m_Speed, -1, 1),
                //(transform.localRotation.eulerAngles.y / 360f) * 2f - 1f,
                tankShooting.m_CanShoot ? 1.0f : 0.0f,
                tankHealth.m_CurrentHealth / tankHealth.m_StartingHealth,
                //otherTankHealth.m_CurrentHealth / otherTankHealth.m_StartingHealth,
                //tankShooting.m_ShotCooldownTimer
            };
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            int moveAction = actionBuffers.DiscreteActions[0];
            int turnAction = actionBuffers.DiscreteActions[1];
            int shootAction = actionBuffers.DiscreteActions[2];

            //float currentReward = GetCumulativeReward();
            //if (lastState == null)
            //{
            //    lastState = GetCurrentStateArray();
            //}
            //float[] action = {moveAction, turnAction, shootAction};

            switch (moveAction)
            {
                case 0:
                    tankMovement.m_MovementInputValue = 0;
                    break;
                case 1:
                    tankMovement.m_MovementInputValue = 1;
                    break;
                case 2:
                    tankMovement.m_MovementInputValue = -1;
                    break;
            }

            switch (turnAction)
            {
                case 0:
                    tankMovement.m_TurnInputValue = 0;
                    break;
                case 1:
                    tankMovement.m_TurnInputValue = 1;
                    break;
                case 2:
                    tankMovement.m_TurnInputValue = -1;
                    break;
            }

            switch (shootAction)
            {
                case 0:
                    // button up after shooting
                    if (m_WasFireButtonPressed)
                    {
                        tankShooting.m_MLFireButtonHeld = false;
                        tankShooting.m_MLFireButtonUp = true;
                        m_WasFireButtonPressed = false;
                    }
                    break;
                case 1:
                    // was not pressing before
                    if (!m_WasFireButtonPressed)
                    {
                        m_WasFireButtonPressed = true;
                        tankShooting.m_MLFireButtonDown = true;
                        tankShooting.m_MLFireButtonHeld = true;
                    }
                    else
                    {
                        tankShooting.m_MLFireButtonHeld = true;
                    }
                break;
            }

            AddReward(-0.0001f);

            //float[] newState = GetCurrentStateArray();
            //float stepReward = GetCumulativeReward() - currentReward;

            //logger.AddTransition(lastState, action, newState, stepReward, GetCumulativeReward());

            //lastState = newState;
        }

        public override void OnEpisodeBegin()
        {
            ResetTank();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActions = actionsOut.DiscreteActions;
            if (tankId == 0)
            {
                discreteActions[0] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? 2 : 0);
                discreteActions[1] = Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? 2 : 0);
                discreteActions[2] = Input.GetKey(KeyCode.Z) ? 1 : 0;
            }
            else
            {
                discreteActions[0] = Input.GetKey(KeyCode.UpArrow) ? 1 : (Input.GetKey(KeyCode.DownArrow) ? 2 : 0);
                discreteActions[1] = Input.GetKey(KeyCode.LeftArrow) ? 1 : (Input.GetKey(KeyCode.RightArrow) ? 2 : 0);
                discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
            }
        }

        private void GetComponents()
        {
            tankMovement = GetComponent<TankMovementML>();
            tankShooting = GetComponent<TankShootingML>();
            tankHealth = GetComponent<TankHealthML>();
            rb = GetComponent<Rigidbody>();    
            powerUpDetector = GetComponent<PowerUpDetectorML>();
            //logger = GetComponent<CSVLogger>(); 

            tankMovement.m_IsMLAgentControlled = true; 
            tankShooting.m_IsMLAgentControlled = true;

            otherTankHealth = otherAgent.GetComponent<TankHealthML>();

            var inputUser = GetComponent<TankInputUser>();
            if (inputUser != null && inputUser.ActionAsset != null)
            {
                inputUser.ActionAsset.Disable();
            }

            tankHealth.DeathNotification += OnDeath;    // notification when dies
            tankHealth.DamageNotification += OnDamageTaken;
            tankShooting.ExplosionPositionNotification += OnExplosion;
            powerUpDetector.OnPowerUpApplied += OnPowerUpApplied;
        }

        private void SaveStartPositions()
        {
            startPos = spawnPoint.position;
            startRot = spawnPoint.rotation;
        }

        private void ResetTank()
        {
            tankMovement.m_MovementInputValue = 0;
            tankMovement.m_TurnInputValue = 0;

            rb.isKinematic = true;

            transform.position = startPos;
            transform.rotation = startRot;
            
            rb.isKinematic = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.Sleep();
            rb.WakeUp();

            tankHealth.ResetHealth();
        }

        private void OnDamageTaken(TankAgent attacker)
        {
            // if he doesnt hurt himself
            if (attacker != this)
            {
                Debug.Log("Altro agente colpito!");
                attacker.AddReward(0.1f);
            }
            else
            {
                Debug.Log("Colpito da solo");
            }
        }

        private void OnDeath(TankAgent attacker)
        {
            Debug.Log($"On Death:");
            AddReward(-1f);
            if (attacker != this)
            {
                Debug.Log("Killed other agent!");
                attacker.AddReward(1f);
            }
            else 
            {
                Debug.Log("Ucciso da solo");
            }
            StartCoroutine(EndEpisodeDelayed());
            // se attacker 
        }

        private IEnumerator EndEpisodeDelayed()
        {
            yield return new WaitForEndOfFrame();
            episodeManager.EndEpisode();
        }

        private void OnExplosion(Vector2 position)
        {
            float distance = Vector2.Distance(position, new Vector2(otherAgent.transform.position.x, otherAgent.transform.position.z));

            if (distance <= maxDistance)
            {
                float reward = (1 - distance / maxDistance) * 0.1f;
                AddReward(reward); 
            }
        }

        private void OnPowerUpApplied()
        {
            AddReward(0.05f);
        }
    }
}

//activate ml-agents
//mlagents-learn config/tank_trainer_config.yaml --run-id=tankRun --train
//tensorboard --logdir=summaries
//Decision Frequency = 5
