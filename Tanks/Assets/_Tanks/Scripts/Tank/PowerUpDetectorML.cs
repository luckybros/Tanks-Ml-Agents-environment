using System;
using System.Collections;
using UnityEngine;

namespace Tanks.Complete
{
    public class PowerUpDetectorML : MonoBehaviour
    {
        // Variable that indicates if the tank has a PowerUp right now
        public bool m_HasActivePowerUp = false;
        public PowerUpML.PowerUpType m_PowerUpType = PowerUpML.PowerUpType.None;
        // References to the tank's components
        private TankShootingML m_TankShooting;
        private TankMovementML m_TankMovement;
        private TankHealthML m_TankHealth;
        private PowerUpHUD m_PowerUpHUD;

        /// <summary>
        /// Evento invocato DOPO che il power-up è stato applicato.
        /// Parametro: il tipo di power-up raccolto.
        /// Gli oracoli si registrano qui per fare le asserzioni.
        /// </summary>
        public event Action<PowerUpML.PowerUpType> OnPowerUpApplied;

        private void Awake()
        {
            // Get references to the tank's movement, shooting, and health components
            m_TankShooting = GetComponent<TankShootingML>();
            m_TankMovement = GetComponent<TankMovementML>();
            m_TankHealth = GetComponent<TankHealthML>();
            m_PowerUpHUD = GetComponentInChildren<PowerUpHUD>();
        }

        // Applies a temporary speed boost to the tank
        public void PowerUpSpeed(float speedBoost, float turnSpeedBoost, float duration)
        {
            m_PowerUpType = PowerUpML.PowerUpType.Speed;
            StartCoroutine(IncreaseSpeed(speedBoost, turnSpeedBoost, duration));
        }

        // Coroutine to temporarily increase the tank's movement speed and turn speed
        private IEnumerator IncreaseSpeed(float speedBoost, float TurnSpeedBoost, float duration)
        {
            // Apply the speed boost
            m_HasActivePowerUp = true;
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.Speed, duration);
            m_TankMovement.m_Speed += speedBoost;
            m_TankMovement.m_TurnSpeed += TurnSpeedBoost;

            // >>> NOTIFICA GLI ORACOLI <<<
            OnPowerUpApplied?.Invoke(PowerUpML.PowerUpType.Speed);

            // Wait for the duration of the power up
            yield return new WaitForSeconds(duration);
            // Revert the speed boost 
            m_TankMovement.m_Speed -= speedBoost;
            m_TankMovement.m_TurnSpeed -= TurnSpeedBoost;
            m_PowerUpType = PowerUpML.PowerUpType.None;
            m_HasActivePowerUp = false;
        }

        // Applies a temporary shooting rate boost to the tank
        public void PowerUpShoootingRate(float cooldownReduction, float duration)
        {
            m_PowerUpType = PowerUpML.PowerUpType.ShootingBonus;
            StartCoroutine(IncreaseShootingRate(cooldownReduction, duration));
        }

        // Coroutine to temporarily enhance the tank's shooting rate
        private IEnumerator IncreaseShootingRate(float cooldownReduction, float duration)
        {
            // Apply the shooting cooldown reduction if it is greater than zero
            if(cooldownReduction > 0)
            {
                m_HasActivePowerUp = true;
                m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.ShootingBonus, duration);
                m_TankShooting.m_ShotCooldown *= cooldownReduction;

                // >>> NOTIFICA GLI ORACOLI <<<
                OnPowerUpApplied?.Invoke(PowerUpML.PowerUpType.ShootingBonus);

                // Wait for the duration of the power up
                yield return new WaitForSeconds(duration);
                // Revert the shooting boost after the duration ends
                m_TankShooting.m_ShotCooldown /= cooldownReduction;
                m_HasActivePowerUp = false;
                m_PowerUpType = PowerUpML.PowerUpType.None;
            }
        }

        // Grants the tank a temporary shield if it does not already have one
        public void PickUpShield(float shieldAmount, float duration)
        {
            if (!m_TankHealth.m_HasShield)
                m_PowerUpType = PowerUpML.PowerUpType.DamageReduction;
                StartCoroutine(ActivateShield(shieldAmount, duration));
        }

        // Grants the tank a temporary shield if it does not already have one
        private IEnumerator ActivateShield(float shieldAmount, float duration)
        {
            // Activate the shield
            m_HasActivePowerUp = true;
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.DamageReduction, duration);
            m_TankHealth.ToggleShield(shieldAmount);

            // >>> NOTIFICA GLI ORACOLI <<<
            OnPowerUpApplied?.Invoke(PowerUpML.PowerUpType.DamageReduction);

            // Wait for the duration of the power up
            yield return new WaitForSeconds(duration);
            // Deactivate the shield
            m_TankHealth.ToggleShield(shieldAmount);
            m_HasActivePowerUp = false;
            m_PowerUpType = PowerUpML.PowerUpType.None;
        }

        // Increases the health of the tank
        public void PowerUpHealing(float healAmount)
        {
            m_TankHealth.IncreaseHealth(healAmount);
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.Healing, 1.0f);

            // >>> NOTIFICA GLI ORACOLI <<<
            OnPowerUpApplied?.Invoke(PowerUpML.PowerUpType.Healing);
        }

        // Makes the tank invulnerable for an amount of time
        public void PowerUpInvincibility(float duration)
        {
            m_PowerUpType = PowerUpML.PowerUpType.Invincibility;
            StartCoroutine(ActivateInvincibility(duration));
        }

        private IEnumerator ActivateInvincibility(float duration)
        {
            m_HasActivePowerUp = true;
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.Invincibility, duration);
            m_TankHealth.ToggleInvincibility();

            // >>> NOTIFICA GLI ORACOLI <<<
            OnPowerUpApplied?.Invoke(PowerUpML.PowerUpType.Invincibility);

            yield return new WaitForSeconds(duration);
            m_HasActivePowerUp = false;
            m_PowerUpType = PowerUpML.PowerUpType.None;
            m_TankHealth.ToggleInvincibility();
        }

        // Equips the tank with a special shell that increases damage
        public void PowerUpSpecialShell(float damageMultiplier)
        {
            m_HasActivePowerUp = true;
            m_PowerUpType = PowerUpML.PowerUpType.DamageMultiplier;
            m_PowerUpHUD.SetActivePowerUp(PowerUp.PowerUpType.DamageMultiplier, 0f);
            m_TankShooting.EquipSpecialShell(damageMultiplier);

            // >>> NOTIFICA GLI ORACOLI <<<
            OnPowerUpApplied?.Invoke(PowerUpML.PowerUpType.DamageMultiplier);
        }
    }
}