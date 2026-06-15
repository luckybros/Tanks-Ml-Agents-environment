using UnityEngine;

namespace Tanks.Complete
{
    public class StuckBugger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Tank"))
            {
                other.GetComponent<TankMovementML>().speedBugMultiplier = 0f;
            }
        }
    }
}