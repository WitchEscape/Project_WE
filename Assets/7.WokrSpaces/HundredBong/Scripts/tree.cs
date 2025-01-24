using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree : MonoBehaviour
{
    public float force;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            print("충ㄷ골");
            rb.AddForce(Vector3.up * force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Scoop")) { return; }
        if (other.transform.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            print("충ㄷ골");
            rb.AddForce(Vector3.up * force);
        }
    }
}
