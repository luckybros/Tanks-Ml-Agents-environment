using UnityEngine;
using System.Collections.Generic;

namespace Tanks.Complete
{
    /// <summary>
    /// Checks if the state of the game is stuck
    /// checking the tanks positions.
    /// </summary>
    public class StuckOracle : OracleBase
    {
        protected override string OracleName => "STUCK ORACLE";

        [Header("Tanks references")]
        [Tooltip("Tanks list to keep trak of")]
        public List<Transform> tanks = new List<Transform>();
        public CoverageManager coverageManager;

        private GlobalGameState averageState;

        [Header("Settings")]
        [Tooltip("Time interval between a sampling and another one")]
        private float interval = 1.0f;
        [Tooltip("Number of sample in history")]
        private int historySize = 5;
        [Tooltip("Threshold between states")]
        public float stuckThreshold = 0.5f;

        // private Queue<GlobalGameState> stateHistory = new Queue<GlobalGameState>();
        //private int steps = 0;
        // TODO: si potrebbe fare con lo stato normale
        private List<Queue<Vector2>> positionHistories = new List<Queue<Vector2>>();
        private float timer = 0f;
        
        private void Update()
        {
            timer += Time.deltaTime;

            if (timer < interval) return;
            
            timer = 0f;

            // Check if every tank is stuck
            for (int i = 0; i < tanks.Count; i++)
            {
                if (tanks[i] == null) continue;

                Vector2 currentPos = Vector2(tanks[i].position.x, tanks[i].position.z);
                var history = positionHistories[i];

                history.Enqueue(currentPos);
                if (history.Count > historySize)
                {
                    history.Dequeue();
                }

                if (history.Count >= historySize && IsStuck(currentPos, history))
                {
                    ReportBug(
                        $"stuck_tank_{i}",
                        $"Tank {i} stuck: the position doesn't change."
                    );
                }
            }
        }

        private bool IsStuck(Vector2 currentPos, Queue<Vector2> history)
        {
            Vector2 sum = Vector2.zero;

            foreach (Vector2 pos in history)
            {
                sum += pos;
            }

            Vector2 average = sum / history.Count;

            return Vector2.Distance(currentPos, average) < stuckThreshold;
        }
    }
}
