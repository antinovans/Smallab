using UnityEngine;
using System.Collections;

public class GameTimer : MonoBehaviour {

	public AudioClip last10Seconds, tick, countIn, gameStart, gameOverSound;

	//public MatchingManager gameController;
	public int roundTime = 20;
	int currentTime;
	float timeRate = 1;
	bool timeStarted = false;
	bool doingStartCountdown = true;
	int countdownTime = 4;
//	public RacingTimer[] timers;
	public GUIStyle mySkin, timeRemainingSyle;
	private float nextTime = 0;
	Rect countdownRect;
	//Rect gameOverRect;
	public Texture2D backgroundTexture;
	
	public Texture2D[] countdownTexture;
	AudioSource[] myAudioSources;
	bool gameOver = false;
	bool enableGameTimers = true;

	private float timeExpiredAlphaValue = 1.0f;
	
	public bool gameCompletedSuccessfully = false;

	private bool showingTimeExpiredDisplay = false;

	// this will display the time that is remaining
	private TextMesh timeRemainingTextMesh;

	void Awake(){
		timeRemainingTextMesh = GameObject.Find ("TimeRemainingText").GetComponent<TextMesh>();
		
		UpdateEnableGameTimersFromPlayerPrefs();
		UpdateGameDurationFromPlayerPrefs();
	}

	public void UpdateEnableGameTimersFromPlayerPrefs(){
		if(PlayerPrefs.HasKey ("EnableGameTimers")){
			enableGameTimers = PlayerPrefs.GetInt ("EnableGameTimers") == 1?true:false;
		}else{
			enableGameTimers = true;
			PlayerPrefs.SetInt ("EnableGameTimers", enableGameTimers?1:0);
		}
		
		timeRemainingTextMesh.GetComponent<Renderer>().enabled = enableGameTimers;
	}

	// Use this for initialization
	void Start () {
	
//		gameController = GetComponent<LeverConfigPanel>();
		myAudioSources = GetComponentsInChildren<AudioSource>();
		GetComponent<AudioSource>().clip = countIn;
		StartCoroutine("SetupGameTime");
		
		timerRect = new Rect(0,0,0,0);
		
		timerRect.width = backgroundTexture.width * 0.7f;
		timerRect.height = backgroundTexture.height * 0.7f;
		
		timerRect.x = (Screen.width/2)-timerRect.width*0.5f;
		timerRect.y = Screen.height - timerRect.height;
		
		StartCoroutine("SetupGameTime");
		countdownRect = new Rect(0,0,0,0);
		countdownRect.width = countdownTexture[0].width*0.6f;
		countdownRect.height = countdownTexture[0].height*0.5f;
		
		countdownRect.x = (Screen.width/2)-countdownRect.width/2;
		countdownRect.y = (Screen.height/2)-countdownRect.height/2;
		
		/*
		gameOverRect.width = gameOverTexture.width*0.8f;
		gameOverRect.height = gameOverTexture.height*0.8f;
		
		gameOverRect.x = (Screen.width/2) - gameOverRect.width/2;
		gameOverRect.y = (Screen.height*0.1f);
		*/
		
		countdownTime = 2;
		currentTime = roundTime;
		timeRemainingTextMesh.text = "Time: " + currentTime;

		showingTimeExpiredDisplay = false;


		UpdateGameDurationFromPlayerPrefs();
	}
	bool didTimeStart = false;
	// Update is called once per frame
	void Update () {
	
		if (currentTime <= 0 && ! gameOver){  // Check to see if the timer has run out
//			Debug.Log("game over!");
			timeStarted = false;
			gameOver = true;
			DoGameOver();
		}else{
			if (timeStarted){  // have we turned on the timer?	
				if (doingStartCountdown){ // handle the countdown time (3-2-1-go)
					if (!didTimeStart){
						GetComponent<AudioSource>().Play();
						didTimeStart = true;
						nextTime = Time.time + timeRate;
					}

					if (countdownTime < 0){  // we've hit zero on our countdown.  Time to start the game
						StartGame();
					}else{  // we're counting down (3-2-1-go)
						GetComponent<AudioSource>().clip = countIn;
						countdownTime = DoCountdown(countdownTime);
	//					Debug.Log("starting countdown");
					}
				}else{  // now we're counting down the regular game clock
					//timeStarted = CheckForFinish();
					//if (!timeStarted)
					//	DoGameOver();
					if(!gameCompletedSuccessfully){
						//Debug.Log("currentTime = " + currentTime);
							if (currentTime < 11)
								GetComponent<AudioSource>().clip = last10Seconds;
							else
								GetComponent<AudioSource>().clip = tick;
							
							currentTime = DoElapsedTimeCountdown(currentTime);
						}
					} // ends the else where we're processing the countdown
				} // ends the check if timeStarted
		} // ends the else

	} // ends method Update
	
	public void UpdateGameDurationFromPlayerPrefs(){
		if(PlayerPrefs.HasKey("GameDuration")){
			roundTime = PlayerPrefs.GetInt("GameDuration");
		}else{
			PlayerPrefs.SetInt("GameDuration", 60);
		}


	}
	
	void StartGame(){

		showingTimeExpiredDisplay = false;
		UpdateGameDurationFromPlayerPrefs();
		currentTime = roundTime;
		timeRemainingTextMesh.text = "Time: " + currentTime;

		myAudioSources[1].clip = gameStart;
		myAudioSources[1].Play();
		doingStartCountdown = false;
		//gameController.SetGameInProgress(true);
		object[] allGameObjects = GameObject.FindSceneObjectsOfType(typeof (GameObject));
		foreach (object o in allGameObjects){
       		GameObject g = (GameObject) o;
			g.SendMessage("HandleCountdownCompleted", SendMessageOptions.DontRequireReceiver);
	   	}
	   	
		gameCompletedSuccessfully = false;
	}
	
	int DoCountdown (int timer){
		
		// count down the time
		if (Time.time > nextTime){
			timer --;
			
			nextTime = Time.time + timeRate;

			GetComponent<AudioSource>().Play();


		}
		
		return timer;
	}

	int DoElapsedTimeCountdown(int timer){
		
		if(enableGameTimers){
	
			// count down the time
			if (Time.time > nextTime){
				timer --;	
				nextTime = Time.time + timeRate;
	
				GetComponent<AudioSource>().Play();
				timeRemainingTextMesh.text = "Time: " + (currentTime - 1);
			}
		
		}
		return timer;
	}
	
	IEnumerator SetupGameTime(){
		
		yield return new WaitForEndOfFrame();
		
		currentTime = roundTime;
		timeRemainingTextMesh.text = "Time: " + currentTime;
		
	}

	/*
	bool CheckForFinish(){

		if(currentTime > 0){
			return true;
		}else{
			return false;
		}
	}
	*/
	
	
	
	void DoGameOver(){

		//showingTimeExpiredDisplay = true;
		//StartCoroutine("fadeOutTimeExpiredDisplay");

		//myAudioSources[1].clip = gameOverSound;
		//myAudioSources[1].Play();
		Debug.Log("sending game over message");
		object[] allGameObjects = GameObject.FindSceneObjectsOfType(typeof (GameObject));
		foreach (object o in allGameObjects){
	       		GameObject g = (GameObject) o;
	       		g.SendMessage("OnGameOver", SendMessageOptions.DontRequireReceiver);
	   	}
	}
	
	protected Rect timerRect ;
	
	protected virtual void OnGUI(){
		
		GUI.depth = 6;
		
//		if ( doingStartCountdown && timeStarted){
//			GUI.Box(timerRect,"");
//		if (countdownTime == 0){
//			GUI.Label(timerRect,"GO!", mySkin);
//				//doingStartCountdown = false;
//		}else{
//			GUI.Label(timerRect,countdownTime.ToString(), mySkin);
//			}
//		}

		//GUI.Label(timerRect,currentTime.ToString(), timeRemainingSyle);

		Rect newRect = new Rect(0,0, countdownRect.width, countdownRect.height);
		if (doingStartCountdown&& timeStarted){
			GUI.BeginGroup(countdownRect);
			if (countdownTime > -1){
//				Debug.Log("countdown time = " + countdownTime);
				if (countdownTexture[countdownTime])
					GUI.DrawTexture(newRect, countdownTexture[countdownTime]);
			}
//			if ( doingStartCountdown && timeStarted){
//				//GUI.Box(timerRect,"");
//				if (countdownTime == 0){
//					GUI.Label(new Rect((newRect.width/2)-newRect.width/2, (newRect.height/2)-newRect.height*0.3f, newRect.width, newRect.height),"GO!", mySkin);
//				//doingStartCountdown = false;
//				}else{
//
//					GUI.Label(new Rect((newRect.width/2)-newRect.width/2, (newRect.height/2)-newRect.height*0.3f, newRect.width, newRect.height),countdownTime.ToString(), mySkin);
//				}
//			}
				
			GUI.EndGroup();
			
		}
/*
		if(showingTimeExpiredDisplay){
			GUI.color = new Color(1.0f, 1.0f, 1.0f, timeExpiredAlphaValue);
			GUI.DrawTexture(gameOverRect, gameOverTexture);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
*/	
		
	}
/*
	IEnumerator  fadeOutTimeExpiredDisplay(){

		// gradually fade out the alpha value for this texture 
		for(float x = 1.0f; x >=0.0f; x -= 0.004f){
			timeExpiredAlphaValue = x;
			yield return new WaitForEndOfFrame();
		}

		showingTimeExpiredDisplay = false;

		yield return new WaitForEndOfFrame();
		
	}
*/
	

	public void OnReset(){
		GetComponent<AudioSource>().clip = countIn;
		countdownTime = 2;
		UpdateGameDurationFromPlayerPrefs();
		currentTime = roundTime;
		timeRemainingTextMesh.text = "Time: " + currentTime;
		timeStarted = false;
		doingStartCountdown = true;
		nextTime = 0;
		gameOver = false;
		didTimeStart = false;

		showingTimeExpiredDisplay = false;
	}
	
	public void StartCountdown(){
		
		Debug.Log("starting timer");
		OnReset ();
		timeStarted = true;

		doingStartCountdown = true;	
		
	}
	
	void OnPause(){
		//doingStartCountdown = false;
		//timeStarted = false;	
	}
}
