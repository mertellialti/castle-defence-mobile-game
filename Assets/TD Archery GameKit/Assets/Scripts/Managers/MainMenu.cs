using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class MainMenu : MonoBehaviour {
	
	//global game data
	public MainGameData data;
	
	//variables visible in the inspector
	public Transform characterPosition;
	public float rotateSensitivity;
	public float characterWarningDuration;
	public Color nonExistingSceneButtonColor;
	public GameObject[] levelButtons;
	public Image musicButtonImage;
	public Sprite musicEnabledSprite;
	public Sprite musicDisabledSprite;
	
	//only show these variables if we're using unity ads
	#if UNITY_ADS
	[Space(5), Header("Unity Ads")]
	public string gameID;
	public int adCoins;
	GameObject adButton;
	#endif
	
	//not visible in the inspector
	GameObject characterSelection;
	Text characterNameLabel;
	GameObject loadingText;
	Animator transitionController;
	Animator mainMenuController;
	int currentCharacterIndex;
	GameObject leftButton;
	GameObject rightButton;
	GameObject characterLock;
	GameObject characterWarning;
	GameObject currentCharacter;
	GameObject buyCharacterButton;
	GameObject priceLabel;
	Text characterInfoNameLabel;
	Text characterInfo;
	Text infoButtonText;
	Text coinLabel;
	bool characterWarningFade;
	Vector3 characterWarningStartPos;
	bool canRotate;
	
    Vector3 mouseReference;
    Vector3 mouseOffset;
    Vector3 rotation;
	
	GameObject sceneToCheck;
	
	void Start(){
		//if we're using unity ads...
		#if UNITY_ADS
		//initialize the ad and the ad button
		Advertisement.Initialize (gameID);
		adButton = GameObject.Find("ad button");
		StartCoroutine(showAdButtonWhenReady());
		#endif
		
		//set the audio volume based on the saved volume
		AudioListener.volume = PlayerPrefs.GetInt("audio");
		if(AudioListener.volume == 0){
			musicButtonImage.sprite = musicDisabledSprite;
		}
		else{
			musicButtonImage.sprite = musicEnabledSprite;
		}
		
		//find some objects
		characterSelection = GameObject.Find("Character selection");
		characterNameLabel = GameObject.Find("Character name").GetComponent<Text>();
		characterInfoNameLabel = GameObject.Find("Name text").GetComponent<Text>();
		characterInfo = GameObject.Find("Info").GetComponent<Text>();
		infoButtonText = GameObject.Find("Info button text").GetComponent<Text>();
		loadingText = GameObject.Find("Loading text");
		leftButton = GameObject.Find("left");
		rightButton = GameObject.Find("right");
		characterLock = GameObject.Find("Character lock");
		characterWarning = GameObject.Find("Character warning");
		buyCharacterButton = GameObject.Find("Buy button");
		priceLabel = GameObject.Find("Price label");
		coinLabel = GameObject.Find("Coins label").GetComponent<Text>();
		
		transitionController = GameObject.Find("Screen transition").GetComponent<Animator>();
		mainMenuController = GameObject.Find("Canvas").GetComponent<Animator>();
		characterWarningStartPos = characterWarning.transform.position;
		coinLabel.text = "" + PlayerPrefs.GetInt("Coins");
		
		//set these things inactive
		loadingText.SetActive(false);
		characterSelection.SetActive(false);
		characterWarning.SetActive(false);
		characterWarningFade = false;
		
		//initialize level buttons and character
		initializeLevelButtons();
		initializeCharacter();
	}
	
	void Update(){
		//if the character waring is active, move it up
		if(characterWarning.activeSelf){
		characterWarning.transform.Translate(Vector3.up * Time.deltaTime * 20);
		
		//if the warning is fading, decrease its alpha value
		if(characterWarningFade && characterWarning.GetComponent<CanvasGroup>().alpha > 0){
			characterWarning.GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
		}
		//else, stop the warning
		else if(characterWarningFade){
			characterWarningFade = false;
			characterWarning.SetActive(false);
		}
		}
		
		//check character rotation
		checkRotation();
	}
	
	//start the kit and the character selection
	public void start(){
		mainMenuController.SetInteger("Main menu", 1);
		characterSelection.SetActive(true);
	}
	
	//go to the tutorial
	public void tutorial(){
		StartCoroutine(openTutorial());
	}
	
	//open the tutorial after showing a small transition effect
	IEnumerator openTutorial(){
		transitionController.SetBool("Transition", true);
		
		yield return new WaitForSeconds(1);
		
		SceneManager.LoadScene("tutorial");
		
		loadingText.SetActive(true);
	}
	
	//open scene by index
	public void openSceneIndex(int sceneIndex){
		//if it's not just checking the scenes, load the actual scene
		if(!sceneToCheck)
			StartCoroutine(loadScene(sceneIndex));
		
		//if it's checking which scenes exist and which don't, turn of the non-existing scene buttons
		if(sceneToCheck && !(sceneIndex < SceneManager.sceneCountInBuildSettings)){
			sceneToCheck.GetComponent<Button>().enabled = false;
			sceneToCheck.GetComponent<Image>().color = nonExistingSceneButtonColor;
		}
	}
	
	//select character left of this character
	public void characterLeft(){
		//save the new selected character
		PlayerPrefs.SetInt("selectedCharacter", PlayerPrefs.GetInt("selectedCharacter") - 1);
		//current character is one to the left
		currentCharacterIndex--;
		
		//change the character object
		changeCharacter();
		
		//if the character is the default one or the character is unlocked, remove the lock/price label
		if(currentCharacterIndex == 0 || PlayerPrefs.GetInt("Character" + currentCharacterIndex) != 0){
			characterLock.SetActive(false);
			buyCharacterButton.SetActive(false);
			priceLabel.SetActive(false);
		}
		//else, show lock, price and buy button
		else{
			characterLock.SetActive(true);
			buyCharacterButton.SetActive(true);
			priceLabel.GetComponent<Text>().text = "" + data.characters[currentCharacterIndex].price;
			priceLabel.SetActive(true);
		}
		
		//show character name and info
		characterNameLabel.text = data.characters[currentCharacterIndex].characterName;
		characterInfoNameLabel.text = data.characters[currentCharacterIndex].characterName;
		characterInfo.text = data.characters[currentCharacterIndex].description;
	}
	
	public void characterRight(){
		//save the new selected character
		PlayerPrefs.SetInt("selectedCharacter", PlayerPrefs.GetInt("selectedCharacter") + 1);
		currentCharacterIndex++;
		
		//change the character object
		changeCharacter();
		
		//if the character is the default one or the character is unlocked, remove the lock/price label
		if(currentCharacterIndex == 0 || PlayerPrefs.GetInt("Character" + currentCharacterIndex) != 0){
			characterLock.SetActive(false);
			buyCharacterButton.SetActive(false);
			priceLabel.SetActive(false);
		}
		//else, show lock, price and buy button
		else{
			characterLock.SetActive(true);
			buyCharacterButton.SetActive(true);
			priceLabel.GetComponent<Text>().text = "" + data.characters[currentCharacterIndex].price;
			priceLabel.SetActive(true);
		}
		
		//show character name and info
		characterNameLabel.text = data.characters[currentCharacterIndex].characterName;
		characterInfoNameLabel.text = data.characters[currentCharacterIndex].characterName;
		characterInfo.text = data.characters[currentCharacterIndex].description;
	}
	
	public void buyCharacter(){
		//check if the player has enough coins
		if(data.characters[currentCharacterIndex].price <= PlayerPrefs.GetInt("Coins")){
			//decrease coins, save character, update coins text and remove the lock
			PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - data.characters[currentCharacterIndex].price);
			PlayerPrefs.SetInt("Character" + currentCharacterIndex, 1);
			coinLabel.text = "" + PlayerPrefs.GetInt("Coins");
			characterLock.SetActive(false);
			buyCharacterButton.SetActive(false);
			priceLabel.SetActive(false);
		}
	}
	
	void initializeCharacter(){
		//get the selected character index
		currentCharacterIndex = PlayerPrefs.GetInt("selectedCharacter");
		
		//change character object
		changeCharacter();
		
		//if the character is the default one or the character is unlocked, remove the lock/price label
		if(currentCharacterIndex == 0 || PlayerPrefs.GetInt("Character" + currentCharacterIndex) != 0){
			characterLock.SetActive(false);
			buyCharacterButton.SetActive(false);
			priceLabel.SetActive(false);
		}
		//else, show lock, price and buy button
		else{
			characterLock.SetActive(true);
			buyCharacterButton.SetActive(true);
			priceLabel.GetComponent<Text>().text = "" + data.characters[currentCharacterIndex].price;
			priceLabel.SetActive(true);
		}
		
		//show the character name
		characterNameLabel.text = data.characters[currentCharacterIndex].characterName;
	}
	
	//change the character object
	void changeCharacter(){
		//destroy the current character
		if(currentCharacter)
			Destroy(currentCharacter);
		
		//get the selected character
		GameObject selectedCharacter = data.characters[currentCharacterIndex].character;
		
		if(selectedCharacter){
			//instantiate the character if possible, else show a warning. Also disable the shooting script
			currentCharacter = Instantiate(selectedCharacter, characterPosition.position, characterPosition.rotation) as GameObject;
			currentCharacter.GetComponent<PlayerArcher>().enabled = false;
		}
		else{
			Debug.LogWarning("Character " + currentCharacterIndex + " doesn't have a prefab!");
		}
		
		//check if left button should be active
		if(currentCharacterIndex == 0){
			leftButton.SetActive(false);
		}
		else{
			leftButton.SetActive(true);
		}
		
		//check if right button should be active
		if(currentCharacterIndex == data.characters.Count - 1){
			rightButton.SetActive(false);
		}
		else{
			rightButton.SetActive(true);
		}
	}
	
	void initializeLevelButtons(){
		//for all levelbuttons...
		for(int i = 0; i < levelButtons.Length; i++){
			//set the scene to check to the current level button scene
			sceneToCheck = levelButtons[i];
			//try the levelbuttons to see if the scenes exist
			levelButtons[i].GetComponent<Button>().onClick.Invoke();
			
			//lock the buttons for levels that have not been unlocked yet
			if(PlayerPrefs.GetInt("Level" + i) == 0 && i != 0){
				levelButtons[i].GetComponent<Button>().enabled = false;
				levelButtons[i].transform.Find("Lock").gameObject.SetActive(true);
				
			}
			//don't lock the unlocked levels
			else{
				levelButtons[i].GetComponent<Button>().enabled = true;
				levelButtons[i].transform.Find("Lock").gameObject.SetActive(false);
			}
		}
		//don't check any more scenes
		sceneToCheck = null;
	}
	
	IEnumerator loadScene(int sceneIndex){
		//if the current character has been purchased...
		if(PlayerPrefs.GetInt("Character" + currentCharacterIndex) == 0 && currentCharacterIndex != 0){
			StartCoroutine(characterSelectionWarning());
		}
		else{
			//show the scene transition
			transitionController.SetBool("Transition", true);
		
			yield return new WaitForSeconds(1);
			
			//load the scene
			SceneManager.LoadScene(sceneIndex);
		
			loadingText.SetActive(true);
		}
	}
	
	//warning for when the player must unlock a character in order to open a certain scene
	IEnumerator characterSelectionWarning(){
		characterWarning.GetComponent<CanvasGroup>().alpha = 1;
		characterWarning.transform.position = characterWarningStartPos;
		characterWarningFade = false;
		characterWarning.SetActive(true);
		
		//wait a moment
		yield return new WaitForSeconds(characterWarningDuration);
		
		//remove the warning
		characterWarningFade = true;
	}
	
	public void startRotating(){
		//set can rotate to true
		canRotate = true;
	}
	
	public void archerInfo(){
		//check main menu state
		if(mainMenuController.GetInteger("Main menu") != 2){
			//show name and description of the current character
			characterInfoNameLabel.text = data.characters[currentCharacterIndex].characterName;
			characterInfo.text = data.characters[currentCharacterIndex].description;
			
			//set the new state
			mainMenuController.SetInteger("Main menu", 2);
			//change the button for going back to the levels page
			infoButtonText.text = "Levels";
		}
		else{
			//go back to the levels page
			mainMenuController.SetInteger("Main menu", 1);
			infoButtonText.text = "Info";
		}
	}
	
	void checkRotation(){
		if(Input.GetMouseButtonDown(0)){
			//reference to the first touched/clicked position
			mouseReference = Input.mousePosition;
		}
		if(Input.GetMouseButton(0) && canRotate){
			//if finger or mouse button stays down, rotate based on mouse/finger position
			mouseOffset = (Input.mousePosition - mouseReference);
			rotation.y = -(mouseOffset.x + mouseOffset.y) * rotateSensitivity;
			currentCharacter.transform.Rotate(rotation);
			mouseReference = Input.mousePosition;
		}
		//stop rotation when not touching the screen anymore
		if(Input.GetMouseButtonUp(0)){
			canRotate = false;
		}
	}
	
	//check for unity ads
	#if UNITY_ADS
	
	//show an ad if the ad is ready
	IEnumerator showAdButtonWhenReady(){
		adButton.GetComponentInChildren<Text>().text = "+" + adCoins;
		adButton.SetActive(false);
		
		while(!Advertisement.IsReady("rewardedVideo")){
			yield return 0;
		}
		
		//show the ad button
		adButton.SetActive(true);
	}
	
	public void watchAd(){
		//get some new show options and use them while showing the ad
		ShowOptions options = new ShowOptions{resultCallback = videoResult};
		Advertisement.Show("rewardedVideo", options);
	}
	
	//result after watching the ad (or not watching the ad..)
	public void videoResult(ShowResult result){
		//check the result, if it's finished, add coins and update the coins label
		if(result == ShowResult.Finished){
			PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + adCoins);
			coinLabel.text = "" + PlayerPrefs.GetInt("Coins");
		}
		
		// try showing the ad button again
		StartCoroutine(showAdButtonWhenReady());
	}
	#endif
	
	//change the music volume based on the current audiolistener volume
	public void changeMusicVolume(){
		if(AudioListener.volume == 0){
			AudioListener.volume = 1;
			musicButtonImage.sprite = musicEnabledSprite;
		}
		else{
			musicButtonImage.sprite = musicDisabledSprite;
			AudioListener.volume = 0;
		}
		//save the new volume
		PlayerPrefs.SetInt("audio", (int)AudioListener.volume);
	}
}
