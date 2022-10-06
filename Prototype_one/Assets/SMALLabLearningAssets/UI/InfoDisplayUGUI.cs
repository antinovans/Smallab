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
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class InfoDisplayUGUI : MonoBehaviour {

	public float versionNumber = 1.0f;

	public string scenarioPdfUrl, scenarioUrl;
	
		GameObject infoPanel;

		public bool isShowingInfoPanel = false;

		Text versionLabel;

		void Awake(){
			infoPanel = GameObject.Find("Canvas/InfoDisplayPanel").gameObject;
			versionLabel = infoPanel.transform.Find("InfoPanel/VersionLabel").GetComponent<Text>();

		}

	// Use this for initialization
	void Start () 
	{
		LoadVersionNumberFromInfoFile();
		versionLabel.text = "Version " + versionNumber;

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
	
	// Update is called once per frame
	void Update () {
			

		
	}


	void OnGUI(){
				// check for key events to toggle InfoDisplay activity state.  While in Editor mode.
		Event e = Event.current;
		if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.I){
			isShowingInfoPanel = !isShowingInfoPanel;
			ShowInfoPanel(isShowingInfoPanel);
		}

	}
	
	public void ShowInfoPanel(bool show){
		isShowingInfoPanel = show;
		infoPanel.GetComponent<CanvasGroup>().interactable = show;
		infoPanel.GetComponent<CanvasGroup>().blocksRaycasts = show;
		infoPanel.GetComponent<CanvasGroup>().alpha = (show) ? 1.0f : 0.0f;
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


}
