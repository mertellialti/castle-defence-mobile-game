using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
	
	//visible in the inspector
	public float killRange;
	
	//not visible
	SphereCollider sphereCollider;

	void Start(){
		//get the collider and immidiately explode
		sphereCollider = GetComponent<SphereCollider>();
		explode();
	}
	
	void explode(){
		//get all enemies
		foreach(Enemy enemy in GameObject.FindObjectsOfType<Enemy>()){
			//for each enemy, check if it's in the explosion range
			if(Vector3.Distance(transform.position, enemy.gameObject.transform.position) <= killRange && enemy.lives > 0)
				//if the enemy is too close to the explosion, it dies
				enemy.die();
		}
		
		//when exploding, there's a growing collider that creates the explosion effect 
		//this collider should be removed after exploding so it doesn't hinder other enemies
		StartCoroutine(removeCollider());
	}
	
	void Update(){
		//if the collider is enabled, it should grow very fast 
		if(sphereCollider.enabled)
			sphereCollider.radius += Time.deltaTime * killRange * 3;
	}
	
	IEnumerator removeCollider(){
		//wait a moment and disable collider
		yield return new WaitForSeconds(0.15f);
		sphereCollider.enabled = false;
	}
}
