using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeArea : MonoBehaviour {
	
	//visible in the inspector
	public float lifetime;
	
	//not visible in the inspector
	List<Enemy> frozenEnemies = new List<Enemy>();
	float waitTimeBeforeFreezing = 0.3f;
	bool freezingEnemy;

	IEnumerator Start(){
		//wait a moment before stopping the particle system
		yield return new WaitForSeconds(lifetime * 0.8f);
		
		//stop particlesystems
		foreach(Transform child in transform){
			if(child.GetComponent<ParticleSystem>())
				child.GetComponent<ParticleSystem>().Stop();
		}
		
		//wait another moment
		yield return new WaitForSeconds(lifetime * 0.2f);
		
		//wait while the enemies is getting frozen
		while(freezingEnemy)
			yield return 0;
		
		//unfreeze all frozen enemies
		foreach(Enemy enemy in frozenEnemies){
			if(enemy)
			enemy.unfreeze();
		}
		
		//destroy the freeze area
		Destroy(gameObject);
	}
	
	//when an enemy hits the area...
	void OnTriggerEnter(Collider col){
		
		//return if this is not an enemy
		if(!col.gameObject.transform.root.gameObject.GetComponent<Enemy>())
			return;
		
		//if this is an enemy, freeze it
		StartCoroutine(freezeEnemy(col.gameObject));
	}
	
	IEnumerator freezeEnemy(GameObject enemy){
		//it is currently freezing an enemy
		freezingEnemy = true;
		//get the enemy script
		Enemy enemyScript = enemy.transform.root.gameObject.GetComponent<Enemy>();
		
		//add the new enemy to the enemy list
		if(!frozenEnemies.Contains(enemyScript))
			frozenEnemies.Add(enemyScript);
		
		//wait a moment before freezing the enemy (so enemies don't collide)
		yield return new WaitForSeconds(waitTimeBeforeFreezing);
		waitTimeBeforeFreezing -= 0.1f;
		enemyScript.freeze = true;
		
		//if there is an enemy with a root object...
		if(enemy && enemy.transform.root.gameObject){
		//get all renderers of the enemy
		foreach(Renderer renderer in enemy.transform.root.gameObject.GetComponentsInChildren<Renderer>()){
			
			//if this is a renderer, it's not an arrow and it doesn't have an arrow script...
			if(renderer && !renderer.gameObject.GetComponent<Arrow>() && !renderer.transform.parent.gameObject.GetComponent<Arrow>()){
				
			//make a list of colors and add all colors of this renderer
			List<Color> colors = new List<Color>();
			for(int i = 0; i < renderer.materials.Length; i++){
				colors.Add(renderer.materials[i].color);
			}
			
			//check if the renderer already exists in the list
			bool containsRenderer = false;
			for(int i = 0; i < enemyScript.renderers.Count; i++){
				if(enemyScript.renderers[i].renderer == renderer)
					containsRenderer = true;
			}
			
			//if this renderer has not been added already, add it to the enemy renderers
			if(!containsRenderer)
				enemyScript.renderers.Add(new colorRenderer{renderer = renderer, originalColors = colors});
			
			//get all materials of this renderer and make them all look like ice
			foreach(Material material in renderer.materials){
				yield return new WaitForSeconds(0.05f);
				material.color = GameObject.FindObjectOfType<Manager>().data.iceColor;
			}
			}
		}
		}
		
		//not freezing an enemy anymore
		freezingEnemy = false;
	}
}
