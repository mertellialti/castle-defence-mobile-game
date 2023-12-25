using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerArcher : MonoBehaviour {
	
	//visible in the inspector
	public float shootingForce;
	public GameObject bow;
	public GameObject idleArrow;
	public Transform spine;
	public float spinez;
	public Transform chest;
	public AudioSource bowPullAudio;
	public AudioSource bowShootAudio;
	public Animator animator;
	
	//not visible in the inspector
	[HideInInspector]
	public GameObject arrow;
	
	[HideInInspector]
	public LineRenderer lineRenderer;
	int predictionLineSteps;
	
	GameObject arrowPointer;
	
	Animator bowAnimator;
	
	[HideInInspector]
	public Color defaultLineColor;
	
	Manager manager;
	
	float mouseStart;
	float zRotation;
	
	bool canAim;
	bool canFire;
	bool readyToShoot;

	// Use this for initialization
	void Start(){
		//get some components
		manager = GameObject.FindObjectOfType<Manager>();
		lineRenderer = GetComponent<LineRenderer>();
		bowAnimator = bow.GetComponent<Animator>();
		
		//find the arrow pointer 
		arrowPointer = GameObject.Find("Arrow pointer");
		//set the default line color
		defaultLineColor = lineRenderer.materials[0].color;
		//set the current arrow with the global game data
		arrow = manager.data.defaultArrow;
		shootingForce = PlayerManager.Instance.PlayerBowRange;
		//the player can currently aim and fire
		canAim = true;
		canFire = true;
	}
	
	// Update is called once per frame
	void Update(){
		//update the arrow pointer rotation (which is used for shooting the arrow in a straight line according to the tower)
		spinez = spine.transform.eulerAngles.z;
        arrowPointer.transform.localEulerAngles = new Vector3(0, 0, -spine.transform.eulerAngles.z * 2);

        //if player clicks the mouse or taps the mobile screen
        if (Input.GetMouseButtonDown(0)){
			//get drag start position
			mouseStart = Input.mousePosition.y;
			
			//can't aim if the player clicked a UI element
			if(EventSystem.current.IsPointerOverGameObject() || Time.timeScale != 1)
				canAim = false;
			
			//also check if the player can aim on mobile devices with the touch finger id
			if(Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began){
				if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
                    canAim = false;           
                }
			}
		}
		
		//when holding the mouse button down... (or finger)
		if(Input.GetMouseButton(0)){
			//if not currently reloading and if the player can aim
			if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Reloading") && canAim){
				//play the aim animations on both the bow and the character
				bowAnimator.SetBool("Aiming", true);
				animator.SetBool("Aiming", true);
				
				//show arrow prediction line to make things easier for the player
				ShowArrowPrediction(idleArrow.transform.position + idleArrow.transform.right * -0.3f, arrowPointer.transform.right * shootingForce, Physics.gravity);
				
				//if the player is not ready to shoot and it is not reloading, play the pull audio and make sure we're ready to shoot
				if(!readyToShoot){
					bowPullAudio.Play();
					readyToShoot = true;
				}
			}
		}
		//if the character is reloading, show the idle arrow (fake arrow) 
		if((animator.GetCurrentAnimatorStateInfo(0).IsName("Reloading") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.5f) || !animator.GetCurrentAnimatorStateInfo(0).IsName("Reloading")){
			idleArrow.SetActive(true);
		}
		else{
			//else, don't show the fake arrow
			idleArrow.SetActive(false);
		}
	}
	
	void LateUpdate(){
		//if the player can aim
		if(canAim){
			//check for the mouse button/finger
			if(Input.GetMouseButton(0)){
				//get the rotation based on the start drag position compared to the current drag position
				zRotation = (Input.mousePosition.y - mouseStart) * (manager.data.sensitivity/10);
				zRotation = Mathf.Clamp (zRotation, -manager.data.maxAimRotation, manager.data.maxAimRotation);
			}
			//reset the rotation if the player is not aiming
			else if((int)zRotation != 0){
				if(zRotation > 0){
					zRotation --;
				}
				else{
					zRotation ++;
				}
			}

            //rotate the character spine based on input
            spine.Rotate(0, -zRotation / 8, zRotation / 2);
            chest.Rotate(0, -zRotation/8, zRotation/2);
		}
		
		//if the player releases mouse button or finger
		if(Input.GetMouseButtonUp(0)){
			//reset the prediction line
			lineRenderer.positionCount = 0;
			//if it is not reloading
			if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Reloading") && canAim){
				//if it can actually fire
				if(canFire){
					//add a new arrow
					var preName = arrow.name;
					GameObject newArrow = Instantiate(arrow, idleArrow.transform.position, Quaternion.Euler(new Vector3(idleArrow.transform.rotation.x, 180, idleArrow.transform.rotation.z)));
					newArrow.name = preName;
					//add force to the arrow
					newArrow.GetComponent<Rigidbody>().velocity = arrowPointer.transform.right * shootingForce;
					//not ready to shoot anymore
					readyToShoot = false;
					//play some audio
					bowShootAudio.Play();
					//start the arrow fired coroutine
					StartCoroutine(ArrowFired());
					
					//if a special arrow is selected, reset the special arrow
					if(manager.selectedArrow != -1){
						manager.buttonsTime[manager.selectedArrow] = 0;
						GameObject loadingBar = manager.data.specialArrows[manager.selectedArrow].button.transform.Find("loading bar").gameObject;
						loadingBar.GetComponent<Image>().color = manager.data.buttonLoadingColor;
					}
					
					//reset the arrow
					arrow = manager.data.defaultArrow;
					//reset the line color
					lineRenderer.materials[0].color = defaultLineColor;
					//reset the special arrow buttons
					manager.resetButtonOutline();
					//reset the selected arrow
					manager.selectedArrow = -1;
				}
				
				//stop aiming animations
				bowAnimator.SetBool("Aiming", false);
				animator.SetBool("Aiming", false);
			}
			
			//can aim again
			canAim = true;
		}
	}
	
	//show the predication line
	void ShowArrowPrediction(Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity){ 
		
		float timeDelta = 1.0f / initialVelocity.magnitude;
		Vector3 position = initialPosition;
		Vector3 velocity = initialVelocity;
		
		//for each step in the line, add one and check if that part of the line hits anything
		predictionLineSteps = 0;
		for (int i = 0; i < manager.data.maxPredictionLineSteps; ++i){
			
			predictionLineSteps++;
			
			//break the line if it hits something
			if(Physics.OverlapSphere(position, 0.1f).Length > 0)
				break;
			
			//increase position and velocity
			position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
			velocity += gravity * timeDelta;
		}
		
		//set the amount of positions
		lineRenderer.positionCount = predictionLineSteps;
		
		//reset the position and velocity
		position = initialPosition;
		velocity = initialVelocity;
		
		//apply the steps to the linerenderer component
		for (int i = 0; i < predictionLineSteps; ++i){
			lineRenderer.SetPosition(i, position);
 
			position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
			velocity += gravity * timeDelta;
		}
	}
	
	//when an arrow was fired
	IEnumerator ArrowFired(){
		//can't fire again immidiately
		canFire = false;
		yield return new WaitForSeconds(0.5f);
		
		//can fire again after half a second
		canFire = true;
	}
}
