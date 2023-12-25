using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour {
	
	//visible in the inspector
	public GameObject splashEffect;
	
	//not visible in the inspector
	Collider meshCollider;
	
	void Start(){
		//get the mesh collider
		meshCollider = GetComponent<MeshCollider>();
	}

	void OnCollisionEnter(Collision col){
		//start the water check on a collision
		StartCoroutine(checkWater(col.gameObject, col.contacts[0].point));
	}
	
	IEnumerator checkWater(GameObject objectToCheck, Vector3 contactPoint){
		//if the object is a ragdoll, add a splash effect using the contact point
		if(objectToCheck.transform.root.gameObject.GetComponent<Ragdoll>()){
			Instantiate(splashEffect, contactPoint, splashEffect.transform.rotation);
		}
		
		//if this is not a spear, disable the collider for 3 seconds to let the objects through
		if(objectToCheck.name != "Spear"){
			meshCollider.enabled = false;
			yield return new WaitForSeconds(3);
			meshCollider.enabled = true;
		}
	}
}
