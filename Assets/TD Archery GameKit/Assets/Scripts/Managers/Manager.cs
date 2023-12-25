using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {
	
	//variables visible in the inspector
	public MainGameData data;
	public GameObject tower;
	public GameObject towerFractured;
	public Sprite pauseImage;
	public Sprite resumeImage;
	public GameObject defeatPanel;
	public GameObject victoryPanel;
	public GameObject button;
	public bool tutorial;
	
	//next level button index
	[Space(5), Tooltip("Index of the next level in the level selection buttons list ('main menu' scene -> Main Menu Manager)")]
	public int nextLevelButtonIndex;
	
	//not visible in the inspector
	CanvasGroup defeatPanelGroup;
	CanvasGroup victoryPanelGroup;
	CanvasGroup panelToFade;
	
	GameObject loadingText;
	Animator transitionController;
	
	GameObject pausePanel;
	GameObject pauseButton;
	GameObject skipText;
	GameObject coinsText;
	Transform characterPosition;
	GameObject playerArcher;
	
	Vector3 camStartPos;
	Vector3 camStartRot;
	
	[HideInInspector]
	public int selectedArrow;
	
	[HideInInspector]
	public int spawnedEnemies;
	
	[HideInInspector]
	public bool gameEnded;
	
	[HideInInspector]
	public int coins;
	
	public static int enemyCount;
	Text spawnedLabel;
	bool fade;
	bool skipKillCam;
	Vector3 scale;
	
	[HideInInspector]
	public List<float> buttonsTime = new List<float>();
	
	void Start(){
		//check if this is not a tutorial
		if(!tutorial){
			//find some objects & components
			spawnedLabel = GameObject.Find("spawned enemies label").GetComponent<Text>();
		
			enemyCount = GetComponent<Spawner>().enemies.Count;
		
			defeatPanelGroup = defeatPanel.GetComponent<CanvasGroup>();
			victoryPanelGroup = victoryPanel.GetComponent<CanvasGroup>();
		
			defeatPanelGroup.alpha = 0;
			victoryPanelGroup.alpha = 0;
		
			pauseButton = GameObject.Find("Pause button");
			pausePanel = GameObject.Find("Pause screen");
			skipText = GameObject.Find("Skip text");
			coinsText = GameObject.Find("Coins text");
			
			//set camera start position and rotation
			camStartPos = Camera.main.gameObject.transform.localPosition;
			camStartRot = Camera.main.gameObject.transform.localEulerAngles;
			
			//camera should not follow an arrow immediately
			Camera.main.gameObject.GetComponent<FollowArrow>().enabled = false;
			//set current scale
			scale = button.transform.localScale;
			
			//disable some panels
			pausePanel.SetActive(false);
			defeatPanel.SetActive(false);
			victoryPanel.SetActive(false);
			skipText.SetActive(false);
			coinsText.SetActive(false);
		}
		
		//find some more objects
		characterPosition = GameObject.Find("Character position").transform;
		
		// loadingText = GameObject.Find("Loading text");
		// loadingText.SetActive(false);
		
		transitionController = GameObject.Find("Screen transition").GetComponent<Animator>();
		
		//set the default arrow to selected
		selectedArrow = -1;
		
		//initialize character and arrows
		InitializePlayerCharacter();
		InitializeSpecialArrows();
	}
	
	void Update(){
		//check if this is not the tutorial scene
		if(!tutorial){
			//update spawn label text
			spawnedLabel.text = (enemyCount - spawnedEnemies) + " / " + enemyCount;
			
			//check if the panel should fade and increase alpha if so
			if(fade && panelToFade.alpha < 1){
				panelToFade.alpha += Time.deltaTime * data.fadeSpeed;
			}
			//stop fading if the alpha is not smaller than 1 anymore
			else if(fade){
				fade = false;
				Time.timeScale = 0;
				coinsText.SetActive(true);
				coinsText.GetComponent<Text>().text = "" + coins;
				PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coins);
			}
			
			//skip kill camera effect if player clicks/taps
			if(Input.GetMouseButtonDown(0))
				skipKillCam = true;
		}
		
		//update the special arrows
		if(data.specialArrows.Count > 0)
			updateSpecialArrows();
	}
	
	public void checkEnemies(){
		//end game if all enemies are killed
		if(allEnemiesKilled())
			StartCoroutine(endGame(true));
	}
	
	//end game (for both victory and defeat)
	public IEnumerator endGame(bool victory){
		
		//the game has ended
		gameEnded = true;
		
		//if defeat
		if(!victory){
			//fracture tower
			Instantiate(towerFractured, tower.transform.position, Quaternion.identity);
			//destroy real tower
			Destroy(tower);
			
			//destroy new enemy popup buttons
			foreach(NewEnemyPopup newEnemyButton in GameObject.FindObjectsOfType<NewEnemyPopup>()){
				Destroy(newEnemyButton.gameObject);
			}
			
			//replace the archer for its ragdoll object
			GameObject archerRagdoll = data.characters[PlayerPrefs.GetInt("selectedCharacter")].characterRagdoll;
			Instantiate(archerRagdoll, playerArcher.transform.position, playerArcher.transform.rotation);
			Destroy(playerArcher);
			
			//wait for the fade
			yield return new WaitForSeconds(data.fadeDelay);
			
			//show the defeat panel
			defeatPanel.SetActive(true);
			panelToFade = defeatPanelGroup;
			fade = true;
		}
		else{
			//if victory, show victory panel and open the next level
			victoryPanel.SetActive(true);
			panelToFade = victoryPanelGroup;
			fade = true;
			
			PlayerPrefs.SetInt("Level" + nextLevelButtonIndex, 1);
		}
	}
	
	//check if all enemies are killed by checking the spawner values
	bool allEnemiesKilled(){
		bool enemiesKilled = true;
		
		for(int i = 0; i < GetComponent<Spawner>().enemies.Count; i++){
			if(!GetComponent<Spawner>().enemies[i].dead)
				enemiesKilled = false;
		}
		
		return enemiesKilled;
	}
	
	//pause the game
	public void pauseGame(){
		//if the game didn't already end
		if(!gameEnded){
			//set the timescale, close/open the pause panel and change the sprite
			if(Time.timeScale != 0){
				Time.timeScale = 0;
				pausePanel.SetActive(true);
				pauseButton.GetComponent<Image>().sprite = resumeImage;
			}
			else{
				Time.timeScale = 1;
				pausePanel.SetActive(false);
				pauseButton.GetComponent<Image>().sprite = pauseImage;
			}
		}
	}
	
	public void home(){
		//load the main menu
		StartCoroutine(loadScene(0));
	}
	
	public void retry(){
		//load the current scene
		StartCoroutine(loadScene(SceneManager.GetActiveScene().buildIndex));
	}
	
	public void next(){
		//load the next scene
		StartCoroutine(loadScene(SceneManager.GetActiveScene().buildIndex + 1));
	}
	
	public void zoomCamera(GameObject arrow){
		//zoom camera towards the enemy kill
		StartCoroutine(zoomCameraKill(arrow));
	}
	
	void InitializePlayerCharacter(){
		//if there is a character, check the selected character, instantiate it and parent it to the character position object
		if(data.characters.Count > 0 && data.characters[PlayerPrefs.GetInt("selectedCharacter")].character){
			playerArcher = Instantiate(data.characters[PlayerPrefs.GetInt("selectedCharacter")].character, characterPosition.position, characterPosition.rotation) as GameObject;
			playerArcher.transform.parent = characterPosition;
		}
		else{
			//if there is no character, show a warning
			Debug.LogWarning("Character " + PlayerPrefs.GetInt("selectedCharacter") + " doesn't exist!");
		}
	}
	
	//initialize the special arrows
	void InitializeSpecialArrows(){
		//for all arrows...
		for(int i = 0; i < data.specialArrows.Count; i++){
			//check if the arrow should be shown
			if(PlayerPrefs.GetInt("selectedCharacter") >= data.specialArrows[i].requiredCharacter || data.specialArrows[i].requiredCharacter == 0){
				//add a button to the list of buttons
				GameObject newButton = Instantiate(button);
				RectTransform rectTransform = newButton.GetComponent<RectTransform>();
				rectTransform.SetParent(GameObject.Find("special arrow buttons").transform, false);
			
				//set the correct button sprite
				newButton.transform.Find("button").gameObject.GetComponent<Image>().sprite = data.specialArrows[i].image;
				newButton.transform.Find("deselect icon").gameObject.SetActive(false);
				
				//set the outline color
				newButton.GetComponent<Image>().color = data.buttonOutlineColor;
				//set the loading bar color
				newButton.transform.Find("loading bar").gameObject.GetComponent<Image>().color = data.buttonLoadingColor;
				
				//change the button name
				newButton.transform.name = "" + i;
			
				//add a onclick function to the button with the name to select the proper arrow
				newButton.transform.Find("button").gameObject.GetComponent<Button>().onClick.AddListener(
				() => { 
				changeArrow(int.Parse(newButton.transform.name)); 
				}
				);
			
				//this is the new button ui
				data.specialArrows[i].button = newButton;
				//add the button to the list
				buttonsTime.Add(new float());
			}
		}
	}
	
	void updateSpecialArrows(){
		//for all special arrows
		for(int i = 0; i < data.specialArrows.Count; i++){
			//if the arrow has a button...
			if(data.specialArrows[i].button){
				//get the arrow button
				Button arrowButton = data.specialArrows[i].button.transform.Find("button").gameObject.GetComponent<Button>();
				
				//increase the loading amount if the arrow is not yet done loading
				if(buttonsTime[i] < data.specialArrows[i].delay){
					buttonsTime[i] += Time.deltaTime;
					arrowButton.enabled = false;	
				}
				else if(!arrowButton.enabled){
					//else, set the button to its ready colors
					buttonsTime[i] = data.specialArrows[i].delay;
					arrowButton.enabled = true;
					data.specialArrows[i].button.transform.Find("loading bar").gameObject.GetComponent<Image>().color = data.buttonReadyColor;
				}
				//update the button fill amount
				data.specialArrows[i].button.transform.Find("loading bar").gameObject.GetComponent<Image>().fillAmount = buttonsTime[i]/data.specialArrows[i].delay;
			}
		}
	}
	
	//change the currently selected arrow
	public void changeArrow(int arrowIndex){
		//get the archer
		PlayerArcher archer = GameObject.FindObjectOfType<PlayerArcher>();
		
		//check if the archer exists
		if(!archer)
			return;
		
		//reset buttons
		resetButtonOutline();
		
		//if this button is not already selected...
		if(selectedArrow != arrowIndex){
			//set the archer arrow to this arrow
			archer.arrow = data.specialArrows[arrowIndex].arrow;
			//set the line/button colors
			archer.lineRenderer.materials[0].color = data.specialArrows[arrowIndex].lineColor;
			data.specialArrows[arrowIndex].button.GetComponent<Image>().color = data.selectedButtonOutlineColor;
			//change the button scale
			data.specialArrows[arrowIndex].button.transform.localScale = new Vector3(scale.x * 1.1f, scale.y * 1.1f, scale.z * 1.1f);
			//show deselect icon
			data.specialArrows[arrowIndex].button.transform.Find("deselect icon").gameObject.SetActive(true);
			//select this arrow
			selectedArrow = arrowIndex;
		}
		else{
			//reset the arrow, color and selected arrow value
			archer.arrow = data.defaultArrow;
			archer.lineRenderer.materials[0].color = archer.defaultLineColor;
			selectedArrow = -1;
		}
	}
	
	public void resetButtonOutline(){
		//for all arrows...
		for(int i = 0; i < data.specialArrows.Count; i++){
			//if there is a button
			if(data.specialArrows[i].button){
				//reset the color, scale and deselect icon
				data.specialArrows[i].button.GetComponent<Image>().color = data.buttonOutlineColor;
				data.specialArrows[i].button.transform.localScale = scale;
				data.specialArrows[i].button.transform.Find("deselect icon").gameObject.SetActive(false);
			}
		}
	}
	
	IEnumerator zoomCameraKill(GameObject arrow){
		//stop if the timescale is not normal
		if(Time.timeScale != 1)
			yield break;
		
		//get the game UI
		GameObject gameUI = GameObject.Find("Game interface");
		Vector3 current = Camera.main.gameObject.transform.localPosition;
		Camera.main.gameObject.transform.localPosition = new Vector3(current.x, current.y - 1, current.z);
		Camera.main.gameObject.GetComponent<FollowArrow>().camTarget = arrow.transform.parent;
		Camera.main.gameObject.GetComponent<FollowArrow>().enabled = true;
		
		//turn off the game UI
		gameUI.SetActive(false);
		//show the skip UI
		skipText.SetActive(true);
		skipKillCam = false;
		
		//set timescale 
		Time.timeScale = data.killCamTimescale;
		
		//get endtime
		float endTime = Time.realtimeSinceStartup + data.killCamDuration;
		
		//wait until endtime
		while(Time.realtimeSinceStartup < endTime && !skipKillCam){
			yield return 0;
		}
		
		//reset everything to the settings before the zoom effect
		Time.timeScale = 1f;
		skipKillCam = false;
		skipText.SetActive(false);
		gameUI.SetActive(true);
		Camera.main.gameObject.GetComponent<FollowArrow>().enabled = false;
		Camera.main.gameObject.transform.localPosition = camStartPos;
		Camera.main.gameObject.transform.localEulerAngles = camStartRot;
	}
	
	IEnumerator loadScene(int sceneIndex){
		//set timescale, show the transition and load the scene
		Time.timeScale = 1;
		transitionController.SetBool("Transition", true);
		
		yield return new WaitForSeconds(1);
		
		SceneManager.LoadScene(sceneIndex);
		
		loadingText.SetActive(true);
	}
}
