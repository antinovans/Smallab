using UnityEngine;
using System.Collections;

public class ScenarioPausePlayScript : MonoBehaviour {
	
	protected bool isPaused = false;
	protected object [] allGameObjects;
	public Texture2D pausedOverlayTexture;
	
	protected Transform pauseAudio, resetAudio, playAudio;

	private GameBrainBaseScript gameBrainScript;
	
	// Use this for initialization
	void Start () {

		gameBrainScript = GameObject.Find ("GameBrain").GetComponent<GameBrainBaseScript> ();


		pauseAudio = transform.Find("PauseAudio");
		resetAudio = transform.Find("ResetAudio");
		playAudio = transform.Find("PlayAudio");
	}
	
	// Update is called once per frame
	void Update () {
		
		// this is looking for the key commands from the kensington pointer device
		if(Input.GetKeyUp("b") || Input.GetKeyUp (KeyCode.Period)){ // this is the square (bottom) button on the kensington pointer
			handleReset();
		}
		if(Input.GetKeyUp(KeyCode.PageDown)){ // this is the > button on the kensington pointer
			
			if (gameBrainScript.currentGameState == GameBrainBaseScript.GameState.WAITING_FOR_GAME_TO_START) {
				isPaused = true;
			}
			
			handlePlay();
			
		}
		if(Input.GetKeyUp(KeyCode.PageUp)){ // this is the < button on the kensington pointer
			handlePause();
		}
		
	
	}
	
	void OnGUI () {
		
		CheckForKeyboardCommands();
		
		if(isPaused){
			GUI.DrawTexture(new Rect( (Screen.width * 0.5f) - (Screen.height * 0.5f), 0, Screen.height, Screen.height), pausedOverlayTexture);	
		}
	}
	
	protected virtual void CheckForKeyboardCommands(){
		// check for key events to toggle activity states.
		/*Event e = Event.current;
		
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape) // check for the escape button
		{
			handleExit();
		}
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R){
			handleReset();
		}
				
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space){
			if (isPaused){
				handlePlay();
			}else{
				handlePause();
			}
			
		}	*/		
	}
	
	public void handleExit(){
		
		// quit the application
		Application.Quit();
	}
	
	protected virtual void handlePlay(){
		
		
		if(isPaused){
			
			isPaused = false;
			Time.timeScale = 1.0f; // restart time
			
			allGameObjects = GameObject.FindSceneObjectsOfType(typeof (GameObject));
	  		foreach (object o in allGameObjects){
	       		GameObject g = (GameObject) o;
	       		g.SendMessage("OnPlay", SendMessageOptions.DontRequireReceiver);
	   		}
			
			playAudio.GetComponent<AudioSource>().Play();
			
			isPaused = false;
		}
	}
	public void handlePause(){
		

		// if we're sitting idle, waiting to start the game, go ahead and treat this as a 'play' command
		if (gameBrainScript.currentGameState == GameBrainBaseScript.GameState.WAITING_FOR_GAME_TO_START) {

			isPaused = true;
			handlePlay();

		}else if(!isPaused){
			
			isPaused = true;
			Time.timeScale = 0.0f; // stop time
			
			allGameObjects = GameObject.FindSceneObjectsOfType(typeof (GameObject));
	  		foreach (object o in allGameObjects){
	       		GameObject g = (GameObject) o;
	       		g.SendMessage("OnPause", SendMessageOptions.DontRequireReceiver);
	   		}
			
			pauseAudio.GetComponent<AudioSource>().Play();
			
			isPaused = true;
		}
	}
	
	public void handleReset(){
		
		allGameObjects = GameObject.FindSceneObjectsOfType(typeof (GameObject));
  		foreach (object o in allGameObjects){
       		GameObject g = (GameObject) o;
       		g.SendMessage("OnReset", SendMessageOptions.DontRequireReceiver);
   		}
		
		resetAudio.GetComponent<AudioSource>().Play();
		
	}
	
	
	
	
}
