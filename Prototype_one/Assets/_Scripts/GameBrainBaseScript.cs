using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System;








public class GameBrainBaseScript : MonoBehaviour {

	[System.Serializable]
	public class ActivityFileList
	{
	    public List <string> files;

	}

	public AudioClip correctWordMatch, incorrectWordMatch;
	public AudioClip allMatchesCompletedSound, gameTimeExpiredSound;


	private List <string>currentWordList = new List<string> ();
	

	private string currentWord = "";
	


	private GameTimer gameTimer;


	private float timeRemaining;
	

	public Texture2D  gameOverTexture, allMatchesCompletedTexture;
	
	private bool showingGameOverDisplay;
	private float timeExpiredAlphaValue = 0.0f;
	
	private bool showingAllMatchesCompletedDisplay;
	private float allMatchesCompletedAlphaValue = 0.0f;
	
	Rect gameOverRect;
    

	ConfigFilePanelScript configFilePanelScript;

	public enum GameState
	{
		WAITING_FOR_GAME_TO_START,
		COUNTING_DOWN_TO_GAME_START,
		GAME_PAUSED,
		GAME_OVER,
		PLAYING,
		WAITING_FOR_INCORRECT_MATCHES_TO_RESET,
		WAITING_FOR_CORRECT_MATCHES_TO_DISPLAY,
	}

	public GameState currentGameState;
	
	void Awake(){

		configFilePanelScript = GameObject.Find("Canvas/ConfigurationDisplay").GetComponent<ConfigFilePanelScript>();
		configFilePanelScript.ConfigOptionChanged += HandleConfigOptionChanged; // add a delegate event handler so we know when something happened

		gameTimer = GameObject.Find ("GameTimer").GetComponent<GameTimer> ();
		currentGameState = GameState.WAITING_FOR_GAME_TO_START;

	}

	// Use this for initialization
	void Start () {
	
		//gameOverRect.width = gameOverTexture.width*0.8f;
		//gameOverRect.height = gameOverTexture.height*0.8f;
		gameOverRect.width = Screen.height * 0.8f;
		gameOverRect.height = Screen.height * 0.8f;
		
		gameOverRect.x = (Screen.width/2) - (gameOverRect.width/2);
		gameOverRect.y = (Screen.height/2) - (gameOverRect.height/2);


        


		
	}

	
	
	// Update is called once per frame
	void Update () {


	}


	public void HandleConfigURLSelected(string fileListStringJSON){

		//ActivityFileList myObject = JsonUtility.FromJson<ActivityFileList>(fileListStringJSON);
		
	}


	

	private List<string> GetImageFilesFromFolder(string filepath){

		List <string> imageListArray = new List <string> ();

		// get rid of the current list so that we can update it
		imageListArray.Clear();

		string [] imagefilesuffix = {"*.jpg", "*.jpeg", "*.png"};

		// before we try to extract the images, make sure that the folder is there		
		if(Directory.Exists (filepath)){
		
			foreach (string suffix in imagefilesuffix) {
				//Debug.Log ("Looking for images that end with: " + suffix);
				//Debug.Log ("checking for images in: " + filepath);
	
							//foreach (string file in System.IO.Directory.GetFiles(pathToConfigFilesRoot)){
				foreach (string file in System.IO.Directory.GetFiles(filepath, suffix)) {

					if(!Path.GetFileNameWithoutExtension(file).StartsWith(".")){ // make sure that we ignore any hidden files that are lurking the directory
						imageListArray.Add ( Path.GetFullPath (file)); // strip it down to just the filename
						//print (file);			
					}
				}
			}
		}


		return imageListArray;
	

	}




	public void HandleConfigOptionChanged(){
		Debug.Log("Config Options Changes - will update the game timers and whether they are enabled");
		UpdateEnableGameTimers();

		UpdateGameDuration();

		//gameTimer.OnReset(); // tell the game timer to read the new config option
		OnReset(); // reset so we're ready to go

		//UpdateComparisonTimerSpeed();

	}


	
	
	


	// this will get called when the user selects the reset button
	void OnReset(){


		timeRemaining = 30.0f;

		currentGameState = GameState.WAITING_FOR_GAME_TO_START;

		
		
		gameTimer.gameCompletedSuccessfully = false;
		gameTimer.OnReset();

	}

	//public void UpdateEnableGameTimers(bool enableGameTimers){
	public void UpdateEnableGameTimers(){
		//PlayerPrefs.SetInt ("EnableGameTimers", enableGameTimers?1:0);
		gameTimer.UpdateEnableGameTimersFromPlayerPrefs();
	}
	
	//public void UpdateGameDuration(int gameDuration){
	public void UpdateGameDuration(){
		//PlayerPrefs.SetInt ("GameDuration", gameDuration);
		gameTimer.UpdateGameDurationFromPlayerPrefs();
	}
	

	

	void OnPlay(){

		if (currentGameState == GameState.WAITING_FOR_GAME_TO_START) {
			StartNewRoundCountdown();
		}

	}


	void StartNewRoundCountdown()
	{
		currentGameState = GameState.COUNTING_DOWN_TO_GAME_START;
		StartGameRound ();

	}

	void HandleCountdownCompleted(){
		currentGameState = GameState.PLAYING;
		//ProcessCompletedWord ();
	}

	void OnGameOver(){
		Debug.Log ("Got Game Over in GameBrainScript");
		ProcessGameOver ();
	}





	void StartGameRound(){

		gameTimer.StartCountdown ();

	}


	
	

	void ProcessGameOver(){
	
		currentGameState = GameState.GAME_OVER;

		
		
		GetComponent<AudioSource>().PlayOneShot (gameTimeExpiredSound, 1.0f);
		
		showingGameOverDisplay = true;
		StartCoroutine("FadeOutGameOverDisplay");
		//DisplayWordMatchSummary();
	}



	
	
	IEnumerator  FadeOutGameOverDisplay(){
		
		// gradually fade out the alpha value for this texture 
		for(float x = 1.0f; x >=0.0f; x -= 0.004f){
			timeExpiredAlphaValue = x;
			yield return new WaitForEndOfFrame();
		}
		
		showingGameOverDisplay = false;
		
		yield return new WaitForEndOfFrame();

		
	}
	
	IEnumerator  FadeOutAllMatchesCompletedDisplay(){
		
		// gradually fade out the alpha value for this texture 
		for(float x = 1.0f; x >=0.0f; x -= 0.004f){
			allMatchesCompletedAlphaValue = x;
			yield return new WaitForEndOfFrame();
		}
		
		showingAllMatchesCompletedDisplay = false;
		
		yield return new WaitForEndOfFrame();
		
		
	}
	

	
	
	
	void OnGUI(){
		GUI.depth = -9;


        if(showingGameOverDisplay){
			GUI.color = new Color(1.0f, 1.0f, 1.0f, timeExpiredAlphaValue);
			GUI.DrawTexture(gameOverRect, gameOverTexture);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		
		if(showingAllMatchesCompletedDisplay){
			GUI.color = new Color(1.0f, 1.0f, 1.0f, allMatchesCompletedAlphaValue);
			GUI.DrawTexture(gameOverRect, allMatchesCompletedTexture);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	
	} // ends OnGUI()
	

	void handlePhysicalDimensions(Vector3 dimensions){

		Debug.Log ("Handling physical dimensions in GameBrainScript");

		// scale this game object by the same as the space dimensions
		transform.localScale = Vector3.Scale(new Vector3(dimensions.x, dimensions.x, dimensions.x), transform.localScale); // multiply each component by the other
		transform.position = Vector3.Scale(dimensions, transform.position);

		
	}
	
	
}





	
