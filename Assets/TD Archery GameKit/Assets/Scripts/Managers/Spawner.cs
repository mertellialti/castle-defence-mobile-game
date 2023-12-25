using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
//enemy variables
public class enemy{
	public int spawnPointIndex;
	public GameObject enemyPrefab;
	public float delay;
	
	public bool newEnemy;
	public Sprite bigImage;
	public Sprite smallImage;
	public string name;
	public string description;
	
	public bool dead;
}

[System.Serializable]
//spawnpoint variables
public class spawnpoint{
	public Transform spawnPoint;
	public GameObject warning;
}

public class Spawner : MonoBehaviour {
	
	//variables not visible in the inspector
	[HideInInspector]
	public List<spawnpoint> spawnpoints;
	[HideInInspector]
	public List<enemy> enemies;
	
	//variables visible in the inspector
	public Vector3 towerPosition;
	public float startWait;
	public GameObject newEnemyButton;
	public Animator newEnemyPopupAnimator;
	
	//not visible in the inspector
	[HideInInspector]
	public bool showRangeWarning;
	
	[HideInInspector]
	public GameObject defaultEnemy;
	
	GameObject targetEnemy;
	Quaternion targetRotation;
	
	IEnumerator Start(){
		//update the range warnings immediately
		if(showRangeWarning)
			updateRangeWarning(0);
		
		//wait before spawning the first enemy
		yield return new WaitForSeconds(startWait);
		
		//for all enemies in the list, spawn it and wait for the delay
		for(int i = 0; i < enemies.Count; i++){
			Spawn(i);
			yield return new WaitForSeconds(enemies[i].delay);
		}
	}
	
	void Update(){
		//get target enemy and rotate towards it
		GetTargetEnemy();
		AssignTargetRotation();
		
		//assign the rotation to rotate towards the enemy
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * GetComponent<Manager>().data.camRotationSpeed);
	}
	
	//spawn the enemy at index i
	void Spawn(int i){
		//get spawnpoint index for this enemy
		int spawnpointIndex = enemies[i].spawnPointIndex;
		
		//instantiate the enemy, assign the spawnpointindex & spawnerindex and add 1 to the spawned enemies value
		GameObject newEnemy = Instantiate(enemies[i].enemyPrefab, spawnpoints[spawnpointIndex].spawnPoint.position, spawnpoints[spawnpointIndex].spawnPoint.rotation);
		newEnemy.GetComponent<Enemy>().spawnPoint = spawnpointIndex;
		newEnemy.GetComponent<Enemy>().spawnerIndex = i;
		GetComponent<Manager>().spawnedEnemies++;
		
		//if this is the last enemy, set last enemy to true in the enemy script
		if(i == enemies.Count - 1)
			newEnemy.GetComponent<Enemy>().lastEnemy = true;
		
		//if this is a new enemy, show the new enemy button
		if(enemies[i].newEnemy)
			NewEnemy(i);
	}
	
	void GetTargetEnemy(){
		//create closest distance and target
		float closestDistance = Mathf.Infinity;
		GameObject closestTarget = null;
		
		float closestDistanceFrozen = Mathf.Infinity;
		GameObject closestFrozenTarget = null;
		
		//for all enemies...
		foreach(Enemy enemy in FindObjectsOfType<Enemy>()){
			//check if the distance is smaller then the closest distance
			//if it is a smaller distance, update the closest target value
			if(Vector3.Distance(enemy.gameObject.transform.position, towerPosition) < closestDistance 
			&& Vector3.Distance(enemy.gameObject.transform.position, towerPosition) > 5 && !enemy.freeze){
				closestDistance = Vector3.Distance(enemy.gameObject.transform.position, towerPosition);
				closestTarget = enemy.gameObject;
			}
			else if(Vector3.Distance(enemy.gameObject.transform.position, towerPosition) < closestDistanceFrozen 
			&& Vector3.Distance(enemy.gameObject.transform.position, towerPosition) > 5 && enemy.freeze){
				closestDistanceFrozen = Vector3.Distance(enemy.gameObject.transform.position, towerPosition);
				closestFrozenTarget = enemy.gameObject;
			}
		}
		
		//if there is a target, assign it
		if(closestTarget){
			targetEnemy = closestTarget;
		}
		//it there is a frozen target and no non-frozen target, assign the frozen one
		else if(closestFrozenTarget){
			targetEnemy = closestFrozenTarget;
		}
	}
	
	void AssignTargetRotation(){
		//if there is a target...
		if(targetEnemy){
			//get the spawnpoint where the closest target was originally spawned and calculate the direction between the castle and the spawnpoint
			int closestEnemySpawnPoint = targetEnemy.GetComponent<Enemy>().spawnPoint;
			Vector3 targetPosition = spawnpoints[closestEnemySpawnPoint].spawnPoint.position;
			Vector3 direction = (targetPosition - transform.position).normalized;
			
			//calculate the target rotation
			Vector3 temporaryRotation = Quaternion.LookRotation(direction).eulerAngles;
			temporaryRotation = new Vector3(transform.rotation.x, temporaryRotation.y - 90, transform.rotation.z);
			targetRotation = Quaternion.Euler(temporaryRotation);
			
			//update the range warning after rotating
			if(!spawnpoints[closestEnemySpawnPoint].warning.activeSelf && showRangeWarning)
				updateRangeWarning(closestEnemySpawnPoint);
		}
	}
	
	void updateRangeWarning(int targetWarning){
		//check which spawnpoint is used to get the range warning
		//turn off all range warnings exept for the target warning
		for(int i = 0; i < spawnpoints.Count; i++){
			if(spawnpoints[i].warning){
				if(i == targetWarning){
					spawnpoints[i].warning.SetActive(true);
				}
				else{
					spawnpoints[i].warning.SetActive(false);
				}
			}
			else{
				//if there is no warning, show a warning in the console
				Debug.LogWarning("Range warning missing");
			}
		}
	}
	
	void NewEnemy(int i){
		//instantiate new button
		GameObject newButton = Instantiate(newEnemyButton);
		RectTransform rectTransform = newButton.GetComponent<RectTransform>();
		rectTransform.SetParent(GameObject.Find("new enemy buttons").transform, false);
			
		//set the correct button sprite
		newButton.transform.Find("Character image").gameObject.GetComponent<Image>().sprite = enemies[i].smallImage;
		
		//get the popup script and assign all values that have to do with the new enemy
		NewEnemyPopup popupScript = newButton.GetComponent<NewEnemyPopup>();
		popupScript.bigImage = enemies[i].bigImage;
		popupScript.enemyName = enemies[i].name;
		popupScript.description = enemies[i].description;
		popupScript.popupAnimator = newEnemyPopupAnimator;
		popupScript.enemy = enemies[i].enemyPrefab.GetComponent<Enemy>();
	}
	
	public void closeNewEnemyPopup(){
		//close the popup and reset the timescale
		Time.timeScale = 1;
		newEnemyPopupAnimator.SetBool("visible", false);
	}
}
