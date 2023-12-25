using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XBotController : MonoBehaviour
{
    private SkinnedMeshRenderer myMeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        myMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        // var col = GetComponent<MeshCollider>();
        // col.sharedMesh = smr.sharedMesh;
        // col.material = GetComponent<PhysicMaterial>();
    }


    // private void Update()
    // {
    //     var colliders = Physics.CheckBox(myMeshRenderer.bounds.center, myMeshRenderer.bounds.extents, transform.rotation, 7);
    //
    //     if (colliders)
    //     {
    //         Debug.Log("Collision detected with friendly");
    //     }
    // }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
        {
            Debug.Log("Collision detected with SkinnedMeshRenderer object!");
        }
    }
    private void OnCollisionStay(Collision collisionInfo)
    {
        Debug.Log("Colliding with " + collisionInfo.gameObject.name);
    }
}
