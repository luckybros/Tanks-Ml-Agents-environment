using UnityEngine;

namespace Tanks.Complete
{
    public class BuckStuckTrap : MonoBehaviour
    {
        private Collider trapCollider;
        
        private void Start()
        {
            trapCollider = GetComponent<Collider>();
            trapCollider.isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Tank"))
            {
                Collider playerCollider = other.GetComponent<Collider>();

                if (trapCollider.bounds.Contains(playerCollider.bounds.min) && 
                trapCollider.bounds.Contains(playerCollider.bounds.max))
                {
                    Debug.Log("Bug activated, player trapped");
                    trapCollider.isTrigger = false;
                }

            }
        }
    }

}
