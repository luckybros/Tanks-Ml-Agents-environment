using UnityEngine;
using System.Collections.Generic;

namespace Tanks.Complete
{
    public class StuckOracle : MonoBehaviour
    {
        public CoverageManager coverageManager;

        private Queue<GlobalGameState> stateHistory = new Queue<GlobalGameState>();
        private GlobalGameState averageState;

        private float timer = 0f;
        private float interval = 1.0f;
        private int historySize = 5;
        private int steps = 0;
        public float stuckThreshold = 0.5f;

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= interval)
            {
                // get new state
                GlobalGameState currentState = coverageManager.GetCurrentState();
                
                stateHistory.Enqueue(currentState);

                if (stateHistory.Count > historySize)
                {
                    stateHistory.Dequeue();
                }
                if (stateHistory.Count >= historySize && CheckIfStuck(currentState))
                {
                    if (OracleManager.Instance != null)
                    {
                        OracleManager.Instance.ReportGameLogicBug(
                            "STUCK ORACLE",
                            "stuck_detected",
                            "Possible Stuck! Tank position doesn't change."
                        );
                    }
                }
                timer = 0f;
            }
        }

        private bool CheckIfStuck(GlobalGameState currentState)
        {
            float sumXTank1 = 0f;
            float sumZTank1 = 0f;
            float sumXTank2 = 0f;
            float sumZTank2 = 0f;

            foreach (GlobalGameState s in stateHistory)
            {
                sumXTank1 += s.t1.x;
                sumZTank1 += s.t1.z;
                sumXTank2 += s.t2.x;
                sumZTank2 += s.t2.z;
            }

            float averageXTank1 = sumXTank1 / stateHistory.Count;
            float averageZTank1 = sumZTank1 / stateHistory.Count;
            float averageXTank2 = sumXTank2 / stateHistory.Count;
            float averageZTank2 = sumZTank2 / stateHistory.Count;

            Vector2 currentPositionTank1 = new Vector2(currentState.t1.x, currentState.t1.z);
            Vector2 currentPositionTank2 = new Vector2(currentState.t2.x, currentState.t2.z);
            Vector2 averagePositionTank1 = new Vector2(averageXTank1, averageZTank1);
            Vector2 averagePositionTank2 = new Vector2(averageXTank2, averageZTank2);

            return Vector2.Distance(currentPositionTank1, averagePositionTank1) < stuckThreshold || 
                    Vector2.Distance(currentPositionTank2, averagePositionTank2) < stuckThreshold;
        }
    }
}
