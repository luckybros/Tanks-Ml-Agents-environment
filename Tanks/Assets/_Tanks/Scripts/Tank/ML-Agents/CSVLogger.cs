using UnityEngine;
using System.IO;
using System;
using System.Globalization;

namespace Tanks.Complete
{
    public class CSVLogger : MonoBehaviour
    {
        private string filepath;
        private StreamWriter file;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            string ID = gameObject.GetInstanceID().ToString();
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            filepath = Path.Combine(homePath, "Desktop", "Tesi", "ML_Agents_tesi", "simple_plugin", "mlagents_plugin", "config", "Tanks", $"TrainingLog{ID}.csv");
            file = new StreamWriter(filepath, true);
            file.AutoFlush = true;
            if (new FileInfo(filepath).Length == 0)
            {
                file.WriteLine("State;Action;NextState;Reward;CumulativeReward");
            }
        }   

        public void AddTransition(float[] state, float[] action, float[] nextState, float reward, float cumulativeReward)
        {
            try 
            {
                string line = string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4}",
                                    FormatVector(state),
                                    FormatVector(action),
                                    FormatVector(nextState),
                                    reward,
                                    cumulativeReward
                                );
                    
                file.WriteLine(line);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("AAAAAAAAAAAAAA", ex);
            }
        }

        private string FormatVector(float[] vector)
        {
            if (vector == null) return "\"\"";
            return "\"" + string.Join(",", Array.ConvertAll(vector, 
                v => v.ToString(CultureInfo.InvariantCulture))) + "\"";
        }

        void OnDestroy()
        {
            if (file != null)
            {
                file.Close();
                file.Dispose();
                file = null;
            }
        }
    }
}
