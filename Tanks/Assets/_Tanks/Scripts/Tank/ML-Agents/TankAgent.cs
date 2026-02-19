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
        public Transform spawnPoint;
        public TankAgent otherAgent;

        private TankMovementML tankMovement;
        private TankShootingML tankShooting;
        private TankHealthML tankHealth;
        private Rigidbody rb;

        private TankHealthML otherTankHealth;

        private Vector3 startPos;
        private Quaternion startRot;
        
        [SerializeField] private float areaSize = 50f;
        [SerializeField] private float maxSpeed = 12f;
        [SerializeField] private float maxTurnSpeed = 12f;

        public bool useVectorObs;

        private bool m_WasFireButtonPressed = false;

        public override void Initialize()
        {
            GetComponents();
            SaveStartPositions();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (useVectorObs)
            {
                sensor.AddObservation(transform.localPosition.x / areaSize);
                sensor.AddObservation(transform.localPosition.z / areaSize);
                sensor.AddObservation(otherAgent.transform.localPosition.x / areaSize);
                sensor.AddObservation(otherAgent.transform.localPosition.z / areaSize);
                sensor.AddObservation(Mathf.Clamp(transform.InverseTransformDirection(rb.linearVelocity).z / tankMovement.m_Speed, -1, 1));
                //sensor.AddObservation(transform.forward.x);
                //sensor.AddObservation(transform.forward.z);
                sensor.AddObservation((transform.localRotation.eulerAngles.y / 360f) * 2f - 1f);
                sensor.AddObservation(tankShooting.m_CanShoot ? 1.0f : 0.0f); // can Shoot
                sensor.AddObservation(tankHealth.m_CurrentHealth / tankHealth.m_StartingHealth);
                sensor.AddObservation(otherTankHealth.m_CurrentHealth / otherTankHealth.m_StartingHealth);
                sensor.AddObservation(tankShooting.m_ShotCooldownTimer);
            }
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            int moveAction = actionBuffers.DiscreteActions[0];
            int turnAction = actionBuffers.DiscreteActions[1];
            int shootAction = actionBuffers.DiscreteActions[2];

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
        }

        public override void OnEpisodeBegin()
        {
            ResetTank();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActions = actionsOut.DiscreteActions;
            discreteActions[0] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? 2 : 0);
            discreteActions[1] = Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? 2 : 0);
            discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        }

        private void GetComponents()
        {
            tankMovement = GetComponent<TankMovementML>();
            tankShooting = GetComponent<TankShootingML>();
            tankHealth = GetComponent<TankHealthML>();
            rb = GetComponent<Rigidbody>();     

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
                otherAgent.AddReward(0.1f);
            }
            else
            {
                Debug.Log("Colpito da solo");
            }
        }

        private void OnDeath()
        {
            Debug.Log($"On Death:");
            AddReward(-1f);
            otherAgent.AddReward(1f);
            EndEpisode();
            otherAgent.EndEpisode();
        }
    }
}

//activate ml-agents
//mlagents-learn config/tank_trainer_config.yaml --run-id=tankRun --train
//tensorboard --logdir=summaries
//Decision Frequency = 5
