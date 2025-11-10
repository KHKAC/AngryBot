using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCBullet : MonoBehaviour
{
    public GameObject effect;

    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000.0f);
        Destroy(gameObject, 3.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        var contact = collision.GetContact(0);
        var obj = Instantiate(effect, contact.point, Quaternion.LookRotation(-contact.normal));
        Destroy(obj, 2.0f);
        Destroy(gameObject);
    }
}
