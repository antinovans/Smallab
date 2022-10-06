/*

Copyright © 2011.  Arizona Board of Regents.  All Rights Reserved
 
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
POSSIBILITY OF SUCH DAMAGE. 

*/

//---------------------------------------------------------------------------------------------------------------------
// InfoDisplay.cs
//---------------------------------------------------------------------------------------------------------------------
// InfoDisplay creates a sliding fullpage Information Display GUI as a 'help' system for an application
// by digesting a series of .png images comprising the background and page(s) to be displayed.  Pages are
// cycled forward and back via the L\R arrow buttons on the keyboard and the GUI is activated byt 'CTRL-I'
// There must be an application level folder named to what you have infoFolder string set to that contains
// images named as described below.
// Page images must be named as follows... page_0.png, page_1.png, etc.
// Background Images must be named as follows... background.png
// You set the speed of the in\out page animation by changing the animationDuration (in seconds) variable.

using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

// Enum A Few States
public enum State{IDLE_IN,IDLE_OUT,ANIM_IN, ANIM_OUT}

public class InfoDisplay : MonoBehaviour {

	// Folder name for directory housing the InfoDisplay images.
	private string infoFolder = "Resources/InfoPages";
	
	public string mainScenarioReferenceURL = "";
	
	// Styles
	public GUIStyle pageNumberDisplayStyle;
	public GUIStyle pageChangeButtonDisplayStyleLeft, pageChangeButtonDisplayStyleRight, versionNumberDisplayStyle;
	
	// Images and file paths
	private Texture2D background = null;
	private Texture2D[] pageImages;
	private List<string> pageImageURLList = new List<string>();
	
	// Page Info
	private int numberOfPages = 0;
	private int currentPage = 0;
	
	private int centerImageWidth = 800;
	private int centerImageHeight = 600;
	
	private int arrowButtonWidth = 100;
	private int arrowButtonHeight = 200;
	
	public float versionNumber = 1.0f;
	
	// GUI utility stuff
	private Rect parentRect;
	private Rect bgRect;
	private Rect infoRect;
	private Rect labelRect;
	private Rect versionLabelRect;
	private Rect leftButtonRect;
	private Rect rightButtonRect;
	private int labelSize = 50;
	private Vector2 versionLabelSize = new Vector2(100, 50);
	private State currentState = State.IDLE_IN;
	
	// movment
	public float animationDuration = 0.5f;
	private float animationSpeed = 0.0f;
	private float animatedX = 0.0f;
	private Vector2 outTarget;
	private Vector2 inTarget;

	void Awake(){
		LoadVersionNumberFromInfoFile ();
	}

	// Use this for initialization
	void Start () 
	{
		//Initialize();
		
		InitializeImageResources();
		
		// Setup default animation \ position info for the display
		animatedX = -Screen.width;
		animationSpeed = Screen.width/animationDuration;
		outTarget = new Vector2(0f,0f);
		inTarget = new Vector2(-Screen.width,0f);
		
		// Setup our Rects for GUI Rendering
		parentRect = new Rect(animatedX, 0, Screen.width, Screen.height);
		bgRect = new Rect(0,0, Screen.width, Screen.height);
		infoRect = new Rect(Screen.width / 2 - 400, Screen.height / 2 - 300, 800, 600);
		labelRect = new Rect(Screen.width / 2 - labelSize, Screen.height / 2 + (300 + labelSize), labelSize, labelSize);
		versionLabelRect = new Rect(Screen.width / 2 - versionLabelSize.x, Screen.height / 2 + (300 + versionLabelSize.y), versionLabelSize.x, versionLabelSize.y);
		//leftButtonRect = new Rect(infoRect.xMin, infoRect.yMax + labelSize, infoRect.width/3, labelSize);
		//rightButtonRect = new Rect(infoRect.xMax - infoRect.width/3, infoRect.yMax + labelSize, infoRect.width/3, labelSize);
		leftButtonRect = new Rect((((Screen.width / 2) - 50) - 450), Screen.height / 2 - 100, 100, 200);
		rightButtonRect = new Rect((((Screen.width / 2) - 50) + 450), Screen.height / 2 - 100, 100, 200);


	}
	
	// Update is called once per frame
	void Update () 
	{
		// 2D Vector to hold our position
		Vector2 newPos;
		
		// Check our GUI's State
		switch (currentState)
		{
			case State.IDLE_IN:
				break;
			case State.IDLE_OUT:
				break;
			case State.ANIM_OUT:
				newPos = new Vector2(animatedX, 0f);
				newPos = Vector2.MoveTowards(newPos,outTarget,Time.deltaTime * animationSpeed);
				animatedX = newPos.x;
			    break;
			case State.ANIM_IN:
				newPos = new Vector2(animatedX, 0f);
				newPos = Vector2.MoveTowards(newPos,inTarget,Time.deltaTime * animationSpeed);
				animatedX = newPos.x;
				break;
			default:
				Debug.LogError("Unknown Animation State.");
				break;
		}
		
		// See if we have reached our intended end point(s)
		// and modify state as necessary.
		if(animatedX >= 0.0f)
		{
			animatedX = 0.0f;
			SetState(State.IDLE_OUT);
		}
			
		if(animatedX <= -Screen.width)
		{
			animatedX = -Screen.width;
			currentPage = 0;
			SetState(State.IDLE_IN);
		}		
		
	}

	private void LoadVersionNumberFromInfoFile(){
		TextAsset infoFileAsset = Resources.Load ("APP_INFO") as TextAsset;

		string[] linesFromFile = infoFileAsset.text.Split ("\n" [0]);

		Dictionary <string, string> infoDictionary = new Dictionary<string, string> ();
		foreach (string row in linesFromFile) {
			string[] keyValuePair = row.Split ('=');
			infoDictionary.Add (keyValuePair[0], keyValuePair[1]);
		}
		versionNumber = float.Parse(infoDictionary["VERSION"]);
	}
	
	// called every frame
	void OnGUI ()
	{
		
		GUI.depth = -10;
		// Don't draw if the GUI is off the screen.
		if(currentState != State.IDLE_IN)
		{
			// Make sure we have valid already loaded images to display
			if(background != null && pageImages[currentPage] != null)
			{
				// Update parentRect
				parentRect = new Rect(animatedX, 0, Screen.width, Screen.height);
				
				
				// Setup our Rects for GUI Rendering
				bgRect = new Rect(0,0, Screen.width, Screen.height);
				
				//Debug.Log("Screen dimensions = " + Screen.width + " x " + Screen.height);
				
				centerImageWidth = (int)(Screen.width * 0.625f);
				centerImageHeight = (int)(Screen.height * 0.625f);
				
				arrowButtonWidth = (int)(100.0f * ((float)(Screen.width)/1280.0f));
				arrowButtonHeight = (int)(200.0f * ((float)(Screen.height)/1024.0f));
				
				infoRect = new Rect(Screen.width / 2 - (centerImageWidth/2), Screen.height / 2 -  (centerImageHeight/2), centerImageWidth, centerImageHeight);
				labelRect = new Rect(Screen.width / 2 - labelSize, Screen.height / 2 + ((centerImageHeight/2) + labelSize), labelSize, labelSize);
				versionLabelRect = new Rect(Screen.width - versionLabelSize.x - 20, Screen.height - versionLabelSize.y - 20, versionLabelSize.x, versionLabelSize.y);
				
				//leftButtonRect = new Rect((((Screen.width / 2) - 50) - (centerImageWidth + 50)), Screen.height / 2 - 100, 100, (centerImageHeight -100));
				//rightButtonRect = new Rect((((Screen.width / 2) - 50) + (centerImageWidth + 50)), Screen.height / 2 - 100, 100, (centerImageHeight - 100));
				leftButtonRect = new Rect((((Screen.width / 2) - (arrowButtonWidth/2)) - ((centerImageWidth/2) + (arrowButtonWidth/2))), Screen.height / 2 - (arrowButtonHeight/2), arrowButtonWidth, arrowButtonHeight);
				rightButtonRect = new Rect((((Screen.width / 2) - (arrowButtonWidth/2)) + ((centerImageWidth/2) + (arrowButtonWidth/2))), Screen.height / 2 - (arrowButtonHeight/2), arrowButtonWidth, arrowButtonHeight);
				 
				
				string labelText = currentPage + 1 + "/" + numberOfPages;
			
				
				// Draw
				GUI.BeginGroup(parentRect);
				
					GUI.DrawTexture(bgRect, background, ScaleMode.StretchToFill);
					GUI.DrawTexture(infoRect, pageImages[currentPage], ScaleMode.ScaleToFit);
					GUI.Label(labelRect, labelText, pageNumberDisplayStyle);
					GUI.Label(versionLabelRect, "Version " +versionNumber, versionNumberDisplayStyle);
					// Handle user page requests
					//if (GUI.Button(leftButtonRect,"<", pageChangeButtonDisplayStyle))
					if (GUI.Button(leftButtonRect, "", pageChangeButtonDisplayStyleLeft))
					{
						GetPreviousPage();
					}
					//if (GUI.Button(rightButtonRect,">", pageChangeButtonDisplayStyle))
					if (GUI.Button(rightButtonRect, "", pageChangeButtonDisplayStyleRight))
					{
						GetNextPage();
					}
					
				GUI.EndGroup();
			}
		}
			
		
		// check for key events to toggle InfoDisplay activity state.  While in Editor mode.
		Event e = Event.current;
		if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.I)
		{
			// Check our GUI's State
			switch (currentState)
			{
				case State.IDLE_IN:
					SetState(State.ANIM_OUT);
					break;
				case State.IDLE_OUT:
					SetState(State.ANIM_IN);
					break;
				case State.ANIM_IN:
					SetState(State.ANIM_OUT);
					break;
				case State.ANIM_OUT:
					SetState(State.ANIM_IN);
					break;
				default:
					Debug.LogError(gameObject.name + "OnGUI::InfoPageStateSwitch::Unknown Animation State.");
				break;
			}
		}
		
		// Check for info page change requests.
		if(e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftArrow)
		{
			GetPreviousPage();
		}
		if(e.type == EventType.KeyDown && e.keyCode == KeyCode.RightArrow)
		{
			GetNextPage();
		}
		
	}
	
	void SetState(State newState)
	{
		currentState = newState;
	}
	
	void InitializeImageResources(){

		background = Resources.Load("InfoPages/background", typeof(Texture2D)) as Texture2D;
		
		numberOfPages = 0;
		
		for(int x = 1; x < 50; x++){ // go through in order and determine how many pages we have up to a max of 50
			if((Resources.Load("InfoPages/page_" + x, typeof(Texture2D)) as Texture2D) != null){ // see if we get something returned
				numberOfPages++;
			}
		}
		
		pageImages = new Texture2D[numberOfPages];
		for(int x = 0; x < numberOfPages; x++){ // load the pages
			pageImages[x] = Resources.Load("InfoPages/page_" + (x + 1), typeof(Texture2D)) as Texture2D;
		}
		
		
		currentPage = 0;
	}
	
	
	void Initialize ()
	{
		string infoFolderPath = "";
		
		if(Application.isEditor)
		{
			// Get the path to the Info folder in our application's root directory. 
			infoFolderPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + infoFolder; 
		}
		else
		{
			DirectoryInfo info = new DirectoryInfo(Application.dataPath).Parent;
			infoFolderPath = info.FullName + "/" + infoFolder;
		}
		
		// See if we have an Info Directory
		if (Directory.Exists(infoFolderPath) == false)
		{
			// If we don't log an error and drop out early destroying this gameObject.
			Debug.LogError(gameObject.name +"::InfoDisplay::Start::" + infoFolderPath + " Does Not Exist. Destroying GameObject");
			Destroy(gameObject);
		}
		else
		{
			// We have a valid Info folder so now read in the available images.
			string[] imagePaths = Directory.GetFiles(@infoFolderPath, "page_*");
			List<int> pageIntList = new List<int>();
			
			// Strip off all but the integer values
			foreach (string s in imagePaths)
			{
				string sub = s.Substring(infoFolderPath.Length+1); 
				sub = sub.Substring(0,sub.Length - 4);
				string[] parts = sub.Split('_');
				int num = int.Parse(parts[1]);
				pageIntList.Add(num);
			}
			
			// Sort the images numerically smallest to largest
			pageIntList.Sort();
			
			// Reconstruct the image path strings
			foreach(int i in pageIntList)
			{
				string path = infoFolderPath + "/page_" + i + ".png";
				path = path.Replace("\\", "/");
				
				// Make sure reconstructed paths are valid and
				// that we can still find the files in question.
				if (File.Exists(path))
				{
					pageImageURLList.Add(path);
				}
			}
			
			// Cleanup
			pageIntList.Clear();
			
			// Finish Setup and Loading of Images
			numberOfPages = pageImageURLList.Count;
			currentPage = 0;
			pageImages = new Texture2D[numberOfPages];
			LoadPageImages();
			
			// Get our background image.
			string bgPath = infoFolderPath + "/background.png";
			bgPath = bgPath.Replace("\\", "/");
			if(File.Exists(bgPath))
			{
				WWW bgDownload = new WWW("file://" + bgPath);
				StartCoroutine(GetBackgroundImage(bgDownload));
			}
		}
	}
	
	// Load the info page images.
	private void LoadPageImages ()
	{
		for(int i=0; i<pageImages.Length; i++)
		{
			WWW req = new WWW("file://" + pageImageURLList[i]);
			StartCoroutine(GetImage(req, i));
		}
	}
	
	// Coroutine for non-blocking image loads
	IEnumerator GetBackgroundImage(WWW reqWWW)
	{        
		yield return reqWWW;        
		
		// We are done, check for errors        
		if (reqWWW.error == null)
		{
			background = null;
			background = reqWWW.texture;
		} 
		else 
		{   
			// Log any error(s)
			Debug.LogError(gameObject.name +"::InfoDisplay::GetBackgroundImage:: " + reqWWW.error);  
		}        
	}
	
	// Coroutine for non-blocking image loads
	IEnumerator GetImage(WWW reqWWW, int index)
	{        
		yield return reqWWW;        
		
		// We are done, check for errors        
		if (reqWWW.error == null)
		{
			pageImages[index] = null;
			pageImages[index] = reqWWW.texture;
		} 
		else 
		{   
			// Log any error(s)
			Debug.LogError(gameObject.name +"::InfoDisplay::GetBackgroundImage:: " + reqWWW.error);        
		}        
	}

	// Cleanup on quit
	void OnApplicationQuit ()
	{
		
		/*
		// Free the textures we created
		Destroy(background);
		
		// Destroy page textures
		foreach(Texture2D tex in pageImages)
		{
			Destroy(tex);
		}
		
		// Cleanup our string List
		pageImageURLList.Clear();
		*/
	}

	// Get previous page index
	private void GetPreviousPage ()
	{
		// Don't allow page flips while the page is animating in or out.
		if(currentState == State.ANIM_IN || currentState == State.ANIM_OUT)
			return;
		
		int testIndex = currentPage-1;
		if(testIndex < 0)
		{
			currentPage = 0;
		}
		else
		{
			currentPage = testIndex;
		}
	}
	
	// Get next page index
	private void GetNextPage ()
	{
		// Don't allow page flips while the page is animating in or out.
		if(currentState == State.ANIM_IN || currentState == State.ANIM_OUT)
			return;
		
		int testIndex = currentPage+1;
		if(testIndex > pageImages.Length -1)
		{
			currentPage = pageImages.Length -1;
		}
		else
		{
			currentPage = testIndex;
		}
	}
	
}
