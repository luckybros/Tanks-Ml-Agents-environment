using UnityEngine;

public class SpeedShower : MonoBehaviour
{
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        Debug.Log("Velocità Attuale x : " + rb.linearVelocity.x);
        Debug.Log("Velocità Attuale x inverse : " + localVelocity.x);
        Debug.Log("Velocità Attuale z: " + rb.linearVelocity.z);
        Debug.Log("Velocità Attuale Z inverse : " + localVelocity.z);
        Debug.Log("Magnitude: " + rb.linearVelocity.magnitude);   
    }
}
