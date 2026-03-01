using UnityEngine;

namespace Tanks.Complete
{
    /// <summary>
    /// Controlla la coerenza della vita dopo ogni cambio (danno o heal).
    /// Si aggancia a TankHealthML.OnHealthChanged (event-driven).
    /// </summary>
    public class DeathOracle : GameLogicOracleBase
    {
        protected override string OracleName => "DEATH ORACLE";

        protected override void RegisterEvents()
        {
            for (int i = 0; i < TankCount; i++)
            {
                if (healths[i] != null)
                {
                    int idx = i;
                    healths[i].OnHealthChanged += () => CheckTank(idx);
                }
            }
        }

        private void CheckTank(int index)
        {
            if (!IsTankActive(index)) return;

            var health = healths[index];
            if (health == null) return;

            // Vita NaN o Infinity
            if (float.IsNaN(health.m_CurrentHealth) || float.IsInfinity(health.m_CurrentHealth))
            {
                ReportBug($"health_invalid_t{index}",
                    $"Tank{index}: Vita ha valore non valido " +
                    $"(NaN={float.IsNaN(health.m_CurrentHealth)}, " +
                    $"Inf={float.IsInfinity(health.m_CurrentHealth)})!");
                return;
            }

            // Vita superiore al massimo
            if (health.m_CurrentHealth > health.m_StartingHealth + 0.01f)
            {
                ReportBug($"health_over_max_t{index}",
                    $"Tank{index}: Vita ({health.m_CurrentHealth:F2}) superiore al " +
                    $"massimo ({health.m_StartingHealth})!");
            }

            // Vita negativa
            if (health.m_CurrentHealth < -0.01f)
            {
                ReportBug($"health_negative_t{index}",
                    $"Tank{index}: Vita negativa ({health.m_CurrentHealth:F2}). " +
                    $"Errore nel calcolo del danno.");
            }

            // Tank ancora attivo con vita <= 0
            if (health.m_CurrentHealth <= 0f && health.gameObject.activeInHierarchy)
            {
                ReportBug($"death_still_alive_t{index}",
                    $"Tank{index}: Vita a {health.m_CurrentHealth:F2} ma il tank " +
                    $"è ancora attivo! OnDeath potrebbe non essere stato invocato.");
            }
        }
    }
}