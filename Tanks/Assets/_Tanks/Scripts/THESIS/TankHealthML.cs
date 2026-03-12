using UnityEngine;
using UnityEngine.UI;
using System;

namespace Tanks.Complete
{
    public class TankHealthML : MonoBehaviour
    {
        public float m_StartingHealth = 100f;
        public Slider m_Slider;
        public Image m_FillImage;
        public Color m_FullHealthColor = Color.green;
        public Color m_ZeroHealthColor = Color.red;
        public GameObject m_ExplosionPrefab;
        [HideInInspector] public bool m_HasShield;
        public bool bugVersion;
        
        private AudioSource m_ExplosionAudio;
        private ParticleSystem m_ExplosionParticles;
        public float m_CurrentHealth;
        private bool m_Dead;
        private float m_ShieldValue;
        private bool m_IsInvincible;

        public event Action<TankAgent> DeathNotification;
        public event Action<TankAgent> DamageNotification;

        /// <summary>
        /// Invocato DOPO ogni cambio di vita (danno, heal, reset).
        /// Gli oracoli si registrano qui.
        /// </summary>
        public event Action OnHealthChanged;

        private void Awake ()
        {
            m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource> ();
            m_ExplosionParticles.gameObject.SetActive (false);
            m_Slider.maxValue = m_StartingHealth;
        }

        private void OnDestroy()
        {
            if(m_ExplosionParticles != null)
                Destroy(m_ExplosionParticles.gameObject);
        }

        private void OnEnable()
        {
            ResetHealth();
        }

        public void TakeDamage (float amount, TankAgent attacker)
        {
            if (!m_IsInvincible)
            {
                m_CurrentHealth -= amount * (1 - m_ShieldValue);
                SetHealthUI ();

                // BUG HANG: "ricalcola finché il danno non è validato con lo scudo"
                // m_HasShield è true quando lo scudo è attivo
                // m_CurrentHealth > 0 perché lo scudo riduce il danno (non lo azzera)
                /*
                if (bugVersion)
                {
                    while (m_HasShield && m_CurrentHealth > 0) {}
                }
                */
                // >>> NOTIFICA GLI ORACOLI <<<
                OnHealthChanged?.Invoke();

                if (m_CurrentHealth <= 0f && !m_Dead)
                {
                    OnDeath (attacker);
                }
                else
                {
                    DamageNotification?.Invoke(attacker);
                }
            }
        }

        public void IncreaseHealth(float amount)
        {
            if (bugVersion)
            {
                // LOGIC BUG: >= invece di <=
                if (m_CurrentHealth + amount >= m_StartingHealth)
                {
                    m_CurrentHealth += amount;
                }
                else
                {
                    m_CurrentHealth = m_StartingHealth;
                }
            }
            else
            {
                if (m_CurrentHealth + amount <= m_StartingHealth)
                {
                    m_CurrentHealth += amount;
                }
                else
                {
                    m_CurrentHealth = m_StartingHealth;
                }
            }
            SetHealthUI();

            // >>> NOTIFICA GLI ORACOLI <<<
            OnHealthChanged?.Invoke();
        }

        public void ToggleShield (float shieldAmount)
        {
            m_HasShield = !m_HasShield;
            if (m_HasShield)
                m_ShieldValue = shieldAmount;
            else
                m_ShieldValue = 0;
        }

        public void ToggleInvincibility()
        {
            m_IsInvincible = !m_IsInvincible;
        }

        private void SetHealthUI ()
        {
            m_Slider.value = m_CurrentHealth;
            m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }

        private void OnDeath (TankAgent attacker)
        {
            m_Dead = true;
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive (true);
            m_ExplosionParticles.Play ();
            m_ExplosionAudio.Play();
            DeathNotification?.Invoke(attacker);
        }

        public void ResetHealth()
        {
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;
            m_HasShield = false;
            m_ShieldValue = 0;
            m_IsInvincible = false;
            SetHealthUI();
        }
    }
}