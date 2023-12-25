using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnGround : MonoBehaviour {
	
	//not visible in the inspector
	bool reachedGround;

	void Update () {
		//if it didn't reach the ground yet, move this object down
		if(!reachedGround)
			transform.root.Translate(Vector3.up * -Time.deltaTime * 2);
	}
	
	//when this object hits a collider...
	void OnTriggerEnter(Collider col){
		//get the other root object and check if it is the ground
		GameObject rootObject = col.gameObject.transform.root.gameObject;
		if(!rootObject.GetComponent<Enemy>() && !rootObject.GetComponent<Ragdoll>() && !rootObject.GetComponent<FreezeArea>() && 
		!rootObject.GetComponent<Arrow>() && col.gameObject.name != "ice cell"){
			//if it is the ground, stop moving down
			reachedGround = true;
		}
	}
}
