using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//color renderer
[System.Serializable] 
public class colorRenderer{
	//the renderer component
	public Renderer renderer;
	//original colors of this renderer
	public List<Color> originalColors = new List<Color>();
}

public class Enemy : MonoBehaviour {
	
	//variables visible in the inspector
	public float runSpeed;
	public float walkSpeed;
	public float attackingDistance;
	public float legHitWaitTime;
	public GameObject ragdoll;
	public GameObject healthBar;
	public RectTransform healthBarFill;
	public Vector3 towerPosition;
	public int startLives;
	public int headDamage;
	public int chestDamage;
	public int legDamage;
	public float freezeDamage;
	public GameObject bloodEffect;
	public GameObject iceFractured;
	public int coins;
	public Transform leg;
	public GameObject coinEffect;
	
	//not visible in the inspector
	[HideInInspector]
	public int spawnPoint;
	
	[HideInInspector]
	public int spawnerIndex;
	
	Animator animator;
	GameObject newRagdoll;
	
	[HideInInspector]
	public bool lastEnemy;
	
	[HideInInspector]
	public bool freeze;
	
	[HideInInspector]
	public bool unfreezing;
	
	[HideInInspector]
	public List<colorRenderer> renderers = new List<colorRenderer>();
	
	Manager manager;
	Spawner spawner;
	
	float unfreezePercentage;
	
	float currentSpeed;
	
	[HideInInspector]
	public float lives;
	
	bool showingKillCam;

	void Start () {
		//start moving with the runspeed
		currentSpeed = runSpeed;
		//get animator, set lives, don't show healthbar and get the manager & spawner
		animator = GetComponent<Animator>();
		lives = startLives;
		healthBar.SetActive(false);
		manager = GameObject.FindObjectOfType<Manager>();
		spawner = GameObject.FindObjectOfType<Spawner>(); 
	}
	
	void Update () {
		//if this enemy is not frozen
		if(!freeze){
			//continue animator
			animator.speed = 1;
			//move the enemy with current speed
			if(Vector3.Distance(transform.position, towerPosition) > attackingDistance){
				transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
			}
			//if it has reached the castle, play the idle animation and end game
			else if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")){
				animator.SetBool("Idle", true);
			
				if(!manager.gameEnded)
				StartCoroutine(manager.endGame(false));
			}
		}
		//if it is frozen
		else{
			//slow down the animator
			if(animator.speed > 0.1f){
				animator.speed -= Time.deltaTime * 3;
				if(animator.speed <= 0.1f)
					animator.speed = 0;
			}
			
			//decrease lives
			if(lives > 0){
				lives -= Time.deltaTime * freezeDamage;
				//update the healthbar with the new amount of lives
				updateHealthBar();
			}
			else{
				//if lives is smaller than 0, die
				die();
			}
		}
		
		//if it is unfreezing....
		if(unfreezing){
			//get all renderers
			foreach(colorRenderer renderer in renderers){
				//get all materials of this renderer
				for(int i = 0; i < renderer.renderer.materials.Length; i++){
					//change all materials of the renderer to their original color
					Color iceColor = renderer.renderer.materials[i].color;
					renderer.renderer.materials[i].color = Color.Lerp(iceColor, renderer.originalColors[i], unfreezePercentage);
					
					//if it is still frozen, keep unfreezing
					if(unfreezePercentage < 1)
						unfreezePercentage += Time.deltaTime/500;
				}
			}
			
			//check if the last material of the last renderer has been resetted
			//if so, stop unfreezing
			Renderer renderComponent = renderers[renderers.Count - 1].renderer;
			if(renderComponent.materials[renderComponent.materials.Length - 1].color == 
			renderers[renderers.Count - 1].originalColors[renderers[renderers.Count - 1].originalColors.Count - 1]){
				unfreezing = false;
			}
		}
	}
	
	//leg hit
	public IEnumerator legHit(){
		//if there are some lives left...
		if(lives >= legDamage + 1){
			//play the leg hit animation
			animator.SetBool("Leg Hit", true);
			//stop the enemy movement
			currentSpeed = 0;
			//decrease lives
			lives -= legDamage;
			//update healthbar
			updateHealthBar();
			
			//wait while the leg hit animation plays
			yield return new WaitForSeconds(legHitWaitTime);
			
			//check for an animator
			if(animator){
				//if it is currently walking, set speed to walk speed
				if(animator.GetBool("Walking")){
					currentSpeed = walkSpeed;
				}
				else{
					//else, set speed to run speed
					currentSpeed = runSpeed;
				}
				//stop leg hit
				animator.SetBool("Leg Hit", false);
			}
		}
		else{
			//if there are no lives left, die
			die();
		}
	}
	
	public void hit(){
		//check if there are lives left
		if(lives >= chestDamage + 1){
			//set walk animation
			animator.SetBool("Walking", true);
			
			//if it was not hit in the leg, set speed to walk speed
			if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Leg hit"))
			currentSpeed = walkSpeed;
			
			//decrease lives
			lives -= chestDamage;
			//update the healthbar
			updateHealthBar();
		}
		else{
			//if there are no lives left, die
			die();
		}
	}
	
	public IEnumerator headShot(GameObject arrow){
		//if there are lives left
		if(lives >= headDamage + 1){
			
			//play head shot animation
			animator.SetBool("Headshot", true);
			//decrease lives
			lives -= headDamage;
			//update health bar
			updateHealthBar();
			
			//wait a moment
			yield return new WaitForSeconds(0.1f);
			
			//stop head shot animation
			animator.SetBool("Headshot", false);
		}
		else{
			//if there are no lives left
			if(manager.data.killCamChance != 0 && !freeze){
				//check killcam and choose wheter or not to show it
				int killCam = Random.Range(1, manager.data.killCamChance);
				
				//if the random value = 1, show kill cam
				if(!lastEnemy && killCam == 1)
					showingKillCam = true;
			}
			
			//if there are no lives left, die
			die();
			
			//show the kill camera effect
			if(showingKillCam)
				manager.zoomCamera(arrow);
		}
	}
	
	public void die(){
		//if there already is a ragdoll (it died already), don't die again so return
		if(newRagdoll != null)
			return;
		
		//if there is a ragdoll and the enemy is not frozen, instantiate a ragdoll
		if(ragdoll && (!freeze || unfreezing)){
			newRagdoll = Instantiate(ragdoll, transform.position, transform.rotation);
		
		//get all arrows in the enemy
		foreach(Arrow arrowScript in GameObject.FindObjectsOfType<Arrow>()){
			//get arrow transform component
			Transform arrowTransform = arrowScript.gameObject.transform;
			//check arrow root to parent it to the ragdoll
			if(arrowTransform.root == gameObject.transform){
				if(arrowTransform.parent.gameObject.name == "legs collider"){
					arrowScript.transform.parent = newRagdoll.GetComponent<Ragdoll>().legs;
				}
				else if(arrowTransform.parent.gameObject.name == "chest"){
					arrowScript.transform.parent = newRagdoll.GetComponent<Ragdoll>().chest;
				}
				else if(arrowTransform.parent.gameObject.name == "head_end"){
					arrowScript.transform.parent = newRagdoll.GetComponent<Ragdoll>().head;
				}
			}
		}
		
		//position and rotate all the ragdoll bones exactly like the enemy bones
		foreach(Transform child in GetComponentsInChildren<Transform>()){
			foreach(Transform ragdollChild in newRagdoll.GetComponentsInChildren<Transform>()){
				if(child.gameObject.name == ragdollChild.gameObject.name){
					ragdollChild.position = child.position;
					ragdollChild.rotation = child.rotation;
				}
			}
		}
		}
		//if the enemy is frozen...
		else if(freeze){
			//instantiate the ice effect
			GameObject ice = Instantiate(iceFractured, transform.position, transform.rotation) as GameObject;
			
			//assign the ice color
			foreach(Transform child in ice.transform){
				child.gameObject.GetComponent<Renderer>().material.color = GameObject.FindObjectOfType<Manager>().data.iceColor;
			}
			
			//get all arrows
			foreach(Arrow arrowScript in GameObject.FindObjectsOfType<Arrow>()){
			
			//unparent the arrows and turn on gravity so they fall to the ground
			Transform arrowTransform = arrowScript.gameObject.transform;
				if(arrowTransform.root == gameObject.transform){
					arrowScript.transform.parent = null;
					arrowScript.gameObject.GetComponent<Rigidbody>().isKinematic = false;
					arrowScript.gameObject.GetComponent<Collider>().enabled = true;
				}
			}
		}
		
		//add coins for killing the enemy
		manager.coins += coins;
		
		//if not showing the kill camera effect, instantiate a coin effect (it looks bad in slowmotion..)
		if(!showingKillCam)
			Instantiate(coinEffect, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
		
		//tell the spawner that this enemy is dead
		spawner.enemies[spawnerIndex].dead = true;
		
		//check if all enemies are dead now
		manager.checkEnemies();
		
		//destroy this enemy
		Destroy(gameObject);
	}
	
	//update the healthbar by showing it and updating the fill size
	void updateHealthBar(){
		healthBar.SetActive(true);
		healthBarFill.sizeDelta = new Vector2((float)lives/startLives, healthBarFill.sizeDelta.y);
	}
	
	//set unfreezing to true and stop freezing
	public void unfreeze(){
		unfreezing = true;
		StartCoroutine(stopFreeze());
	}
	
	//wait a moment and set freeze to false
	IEnumerator stopFreeze(){
		yield return new WaitForSeconds(1f);
		freeze = false;
	}
}
