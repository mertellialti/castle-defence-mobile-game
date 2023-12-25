using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public int damage = 10;
    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var rootGameObj = collision.gameObject.transform.root.gameObject;
        Debug.Log(rootGameObj);
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;
        transform.parent = collision.gameObject.transform;
        GetComponent<Collider>().enabled = false;

        //if (rootGameObj.CompareTag("Enemy"))
        //{
        //    if (collision.transform.CompareTag("Head"))
        //    {
        //        if (rootGameObj.TryGetComponent(out EnemyHealth health))
        //        {
        //            health.SetHealth(-(health.CurrentHealth));
        //        }
        //    }
        //    else if (collision.transform.CompareTag("Body"))
        //    {
        //        if (rootGameObj.TryGetComponent(out EnemyHealth health))
        //        {
        //            health.SetHealth(-damage);
        //        }
        //    }
        //    else if (collision.transform.CompareTag("Leg"))
        //    {
        //        if (rootGameObj.TryGetComponent(out EnemyHealth health))
        //        {
        //            health.SetHealth(-damage * (3 / 4));
        //        }
        //        if (rootGameObj.TryGetComponent(out Movement movement))
        //        {
        //            movement.MovementSpeed = movement.MovementSpeed / 2;
        //        }
        //    }
        //}
    }
}
