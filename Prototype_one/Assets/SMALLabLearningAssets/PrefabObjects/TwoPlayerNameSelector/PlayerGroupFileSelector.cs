using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System;

public class PlayerGroupFileSelector : MonoBehaviour {
	
	StreamReader streamReader;
	
	public AudioClip itemSelectedAudioClip;
	
	public GUIStyle boxStyle;
	public GUIStyle boxTextStyle;
	public GUIStyle selectStudentBoxStyle;
	public GUIStyle selectStudentButtonStyle;
	GUIStyle listStyle;
	GUIContent []list;
	bool showList = false;
	int listEntry = 0;
	
	
	//public DataLogger dataLoggerScript;
	
	string currentConfigFile = "";
	string userSelectedConfigFile = "";
	
	public Vector2 scrollPosition = Vector2.zero;
	
	// TODO!! need to make this nonserializable so that it won't show up in the unity editor
	public XmlDocument currentXMLDoc;
	
	ArrayList fileListArray = new ArrayList();
	bool configFilesRootFolderExists = false;
	string pathToConfigFilesRoot = "";
	
	GUIContent []studentList;
	ArrayList studentListArray = new ArrayList();
	
	public Rect listLocationRect = new Rect(50, 150, 300, 20);
	public string selectButtonLabel = "Select Config File";
	
	float retractedDisplayPosY = Screen.height - 100;
	float expandedDisplayPosY = 200;
	float studentOneDisplayPosY = 0;
	float studentTwoDisplayPosY = 0;
	
	/** config files must be parallel to the root of the application folder
	 * If the application is named 'DogsAndCats'
	 * the convention is for config files to be contained in a folder 'DogsAndCats_Media'
	 * that sits parallel to the application folder
	 */
	
	public string ConfigFilesRootFolder = "";
	
	string currentPlayerOneStudent = "";
	string currentPlayerTwoStudent = "";
	bool showPlayerOneStudentList = false;
	bool showPlayerTwoStudentList = false;
	bool showConfigFileSelectionPrompt = false;
	bool configFilesFound = false;
	
	// these are events that other scripts can register as listeners
	// to register a listener from another script, insert the following in your code:
	/**
	 * In your header: XMLConfigFileIOScript configFileIOScript;
	 * 
	 * In your Start() method:
	 * configFileIOScript = GameObject.Find("XMLConfigFileIO").GetComponent<XMLConfigFileIOScript>();
	 * configFileIOScript.ConfigFileSelected += HandleConfigFileSelected; // add this as a listener (HandleConfigFileSelected(string configFile) should be a method in your custom script that receives the notification
	 * 
	 * Make sure that you have a method in your script: void HandleConfigFileSelected(string configFile){}
	 * 
	 * to load the stored xml file, call this next: configFileIOScript.ReadCurrentConfigFilePref()
	 * 
	*/
	
	public delegate void ConfigFileSelectedEventHandler(string filename);
	public event ConfigFileSelectedEventHandler ConfigFileSelected;
	
	string dataPath = "";
	// Use this for initialization
	void Awake () {
		
		retractedDisplayPosY = Screen.height - 25;
		expandedDisplayPosY = Screen.height - 500;
		studentOneDisplayPosY = retractedDisplayPosY;
		studentTwoDisplayPosY = retractedDisplayPosY;
		
		
		
		dataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/SMALLabLearning/" + ConfigFilesRootFolder;
		//dataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Documents/SMALLabLearning/" + ConfigFilesRootFolder;

	//	if(Directory.Exists(Application.dataPath + "/../../" + ConfigFilesRootFolder)){
		if(Directory.Exists(dataPath)){
			Debug.Log("directory " + dataPath + " exists!");
			configFilesRootFolderExists = true;
		//	pathToConfigFilesRoot = Application.dataPath + "/../../" + ConfigFilesRootFolder;
			pathToConfigFilesRoot = dataPath;
			ReadCurrentConfigFilePref();
			
		}else{ // if the root directory doesn't exist, create it
			string newDir = dataPath;
			Directory.CreateDirectory(newDir);
			configFilesRootFolderExists = true;
			
		}
	
		// if it's there, go ahead and generate a list of files that we can use
		if(configFilesRootFolderExists){
			GenerateListOfConfigFiles();
			//GenerateListOfFolders();
			
			if(configFilesFound){
				InitConfigSelectionGUI();
				//showConfigFileSelectionPrompt = true;
			}
		}

		
		InitWithStoredStudentGroupFile();
		
	}
	
	
	void Start(){
		//dataLoggerScript = GameObject.Find("DataLogger").GetComponent<DataLogger>();
	}
	
	public void ReadCurrentConfigFilePref(){
		
		currentConfigFile = PlayerPrefs.GetString("CurrentStudentGroupFile");
		
		// if it's valid go ahead and process, alerting any listeners
		if(currentConfigFile != null){
			handleConfigurationFileSelected();
		}
		
	}
	
	public void InitWithStoredStudentGroupFile(){
		
		currentConfigFile = PlayerPrefs.GetString("CurrentStudentGroupFile");
		
		if (ConfigFileSelected != null){ // make sure that we have a listener
			ConfigFileSelected( GetFullPathFromConfigFile(currentConfigFile) );	
		}
		
		if(currentConfigFile != null)
			ParseStudentGroupXMLFile(GetFullPathFromConfigFile(currentConfigFile));
	}
	
	void StoreCurrentConfigFilePref(){
		
		PlayerPrefs.SetString("CurrentStudentGroupFile", currentConfigFile);
		
	}

	// Update is called once per frame
	void Update () {
	
	}
	
	
	void RefreshMenuOfFolders(){
		// if it's there, go ahead and generate a list of files that we can use
		if(configFilesRootFolderExists){
			GenerateListOfConfigFiles();
			//GenerateListOfFolders();
			
			if(configFilesFound){
				InitConfigSelectionGUI();
				//showConfigFileSelectionPrompt = true;
			}
		}
	}
	
	void GenerateListOfConfigFiles(){
		
		// get rid of the current list so that we can update it
		fileListArray.Clear();
		
		//foreach (string file in System.IO.Directory.GetFiles(pathToConfigFilesRoot)){
		foreach (string file in System.IO.Directory.GetFiles(pathToConfigFilesRoot)){
			
			Debug.Log("checking " + file + " in " + pathToConfigFilesRoot);
			//fileListArray.Add(Path.GetDirectoryName(file)); // strip it down to just the filename
			fileListArray.Add(Path.GetFileName(file)); // strip it down to just the filename
			print(file);	
			
		}
		
		// make sure that we actually found some config files
		if(fileListArray.Count > 0){
			//fileListArray.Insert(0, ""); // put a blank one at the top so the user isn't required to select an option
			configFilesFound = true;
		}		
	}
	
	void InitConfigSelectionGUI(){
		// Make some content for the popup list
    	list = new GUIContent[fileListArray.Count];
		int index = 0;
		foreach(string file in fileListArray){
			list[index] = new GUIContent(file);	
			index++;
		}
		    
    	// Make a GUIStyle that has a solid white hover/onHover background to indicate highlighted items
    	listStyle = new GUIStyle();
    	listStyle.normal.textColor = Color.white;
		Texture2D tex = new Texture2D(2, 2);
    	Color [] colorsArray = new Color[4];
    	for(int x = 0; x < colorsArray.Length; x++){
			//(Color color in colorsArray){
			colorsArray[x] = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
    	
		tex.SetPixels(colorsArray);
    	tex.Apply();
		Texture2D blackTexture = new Texture2D(2, 2);
    	Color [] blackColorsArray = new Color[4];
    	for(int x = 0; x < blackColorsArray.Length; x++){
			//(Color color in colorsArray){
			blackColorsArray[x] = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		}
    	
		blackTexture.SetPixels(blackColorsArray);
    	blackTexture.Apply();
		listStyle.normal.background = blackTexture;
    	listStyle.hover.background = tex;
    	listStyle.onHover.background = tex;
    	listStyle.padding.left = listStyle.padding.right = listStyle.padding.top = listStyle.padding.bottom = 4;
		

	}
	
	
	
	
	void handleConfigurationFileSelected(){
			
		// make sure that the user has made a valid selection
		if(userSelectedConfigFile != "" && currentConfigFile != userSelectedConfigFile){
			
			// play audio so the user knows something has been selected
			GetComponent<AudioSource>().PlayOneShot(itemSelectedAudioClip);
			
			currentConfigFile = userSelectedConfigFile; // update the currentConfigFile to the one selected by the user
			
			// store this in player prefs so we'll remember it next time around
			StoreCurrentConfigFilePref();
			
			// notify any listeners of this new event - it sends the full path to make it easier to retrieve
			//ConfigFileSelected( Application.dataPath + "/../" + ConfigFilesRootFolder + "/" + currentConfigFile );
			if (ConfigFileSelected != null) // make sure that we have a listener
				ConfigFileSelected( GetFullPathFromConfigFile(currentConfigFile) );
			
			
			ParseStudentGroupXMLFile(GetFullPathFromConfigFile(currentConfigFile));
		}
		
	}
	
	
	void ParseStudentGroupXMLFile(string filepath){
		try{
			XmlDocument doc = new XmlDocument();
			doc.Load(filepath);
		
			
			XmlNodeList xnList = doc.SelectNodes("studentgroup/student");
			
			studentListArray = new ArrayList();
			Student aStudent;
			foreach (XmlNode xn in xnList){
				string name = xn["name"].InnerText;
				string id = xn["id"].InnerText;
 			
  				Debug.Log("Got Student Named: " + name);
				
				aStudent = new Student();
				aStudent.name = name;
				aStudent.id = id;
			
				studentListArray.Add(aStudent);
			} 
		
		
			// Make some content for the popup list
    		// make sure that we have updated the list of students available for selection in each list
			studentList = new GUIContent[studentListArray.Count];
			int index = 0;
			
			foreach(Student student in studentListArray){
				studentList[index] = new GUIContent(student.name);	
				index++;
			}
			
		}catch(System.Exception e){
		
			Debug.Log(e.ToString());
			
		}
		
	}
	
	
	
	string GetFullPathFromConfigFile(string filename){
	
		//return Application.dataPath + "/../../" + ConfigFilesRootFolder + "/" + filename;
		return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/SMALLabLearning/" + ConfigFilesRootFolder + "/" + filename;
		//return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Documents/SMALLabLearning/" + ConfigFilesRootFolder + "/" + filename;
		
	}
	
	
	void OnGUI(){
		
		GUI.depth = -9;
		
		// check for key events to toggle InfoDisplay activity state.  While in Editor mode.
		Event e = Event.current;
		if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.W)
		{
			//Debug.Log("Pressed the A key");
			showConfigFileSelectionPrompt = !showConfigFileSelectionPrompt;
		}
		
		
		
		if(showConfigFileSelectionPrompt){
			GUI.BeginGroup(new Rect(Screen.width/2 - 300, Screen.height/2 - 200, 600, 320));
			GUI.Box(new Rect(0, 0, 600, 320), "Load a Student Group File", boxStyle);
				if(configFilesFound){
					GUI.Label(new Rect(50, 40, 500, 30), "Select a file to load a different set of students.  Press control-c to hide this chooser.", boxTextStyle);
					
					GUI.Label(new Rect(50, 70, 500, 20), "Current Student Group: " + currentConfigFile, boxTextStyle);
				
					 scrollPosition = GUI.BeginScrollView(new Rect(150, 100, 300, 150), scrollPosition, new Rect(0, 0, 280, (list.Length * 25)), false, false);
				    	for(int x = 0; x < list.Length; x++){
							if(GUI.Button(new Rect(0, x * 25, 280, 25), list[x].text, listStyle)){
								userSelectedConfigFile = list[x].text;
								handleConfigurationFileSelected();
							}	
						}
					/*
						GUI.Button(new Rect(0, 0, 280, 25), "Top-left", listStyle);
        				GUI.Button(new Rect(0, 25, 280, 25), "Top-right", listStyle);
        				GUI.Button(new Rect(0, 50, 280, 25), "Bottom-left", listStyle);
        				GUI.Button(new Rect(0, 75, 280, 25), "Bottom-right", listStyle);
        				*/
			        GUI.EndScrollView();
				
				/*
					if (Popup.List (new Rect(150, 70, 300, 20), ref showList, ref listEntry, new GUIContent(selectButtonLabel), list, listStyle)) {
	        			print(list[listEntry].text);
						userSelectedConfigFile = list[listEntry].text;
					
						handleConfigurationFileSelected();
				
    				}
    				*/
					/*
					// provide a button so the user can close the window
					if(GUI.Button(new Rect(250, 150, 100, 30), "Done")){
						showConfigFileSelectionPrompt = false;
					}
					*/
					
					
					
				
				}
				else{
					GUI.Label(new Rect(50, 40, 500, 100), "No student group files were found.  Please add at least one student group file in the folder: " + dataPath, boxTextStyle);
					
				}
				// provide a button to open the base folder
				if(GUI.Button(new Rect(300 - 150 - 10, 320 - 30 - 10, 150, 30), "Open Profiles Folder")){
					Debug.Log("Opening folder: " + dataPath);
					Application.OpenURL (dataPath);

				}
				// provide a button to open the base folder
				if(GUI.Button(new Rect(300 + 10, 320 - 30 - 10, 150, 30), "Refresh Profiles List")){
					RefreshMenuOfFolders();
				}
			
			GUI.EndGroup();
					
					
		}
		
		
		ShowPlayerStudentList(0, currentPlayerOneStudent, showPlayerOneStudentList);
		ShowPlayerStudentList(1, currentPlayerTwoStudent, showPlayerTwoStudentList);
		
		/*
		//showPlayerOneStudentList = true;
		if(showPlayerOneStudentList){
			
		}else{
			ShowPlayerStudentList(0, currentPlayerOneStudent, false);
		}
		
		//showPlayerTwoStudentList = true;
		if(showPlayerTwoStudentList){
			ShowPlayerStudentList(1, currentPlayerTwoStudent, true);
		}else{
			ShowPlayerStudentList(1, currentPlayerTwoStudent, false);
		}
		*/
		
	}
	
	public void ShowPlayerStudentList(int playerID, string currentStudentName, bool showExpanded){
		
		float targetDisplayPosY = 0.0f;
		if(playerID == 0)
			targetDisplayPosY = studentOneDisplayPosY;
		else
			targetDisplayPosY = studentTwoDisplayPosY;
		
		if(showExpanded){
			targetDisplayPosY = Mathf.MoveTowards(targetDisplayPosY, expandedDisplayPosY, Time.deltaTime * 800.0f);
		}else{
			targetDisplayPosY = Mathf.MoveTowards(targetDisplayPosY, retractedDisplayPosY, Time.deltaTime * 800.0f);
		}
		
		
		if(playerID == 0){
			//GUI.BeginGroup(new Rect(Screen.width/2 - 310, targetDisplayPosY, 300, 500));
			GUI.BeginGroup(new Rect(100, targetDisplayPosY, 300, 500));
			studentOneDisplayPosY = targetDisplayPosY;
		}else{
			//GUI.BeginGroup(new Rect(Screen.width/2 + 10, studentTwoDisplayPosY, 300, 500));
			GUI.BeginGroup(new Rect(Screen.width - 400, studentTwoDisplayPosY, 300, 500));
			studentTwoDisplayPosY = targetDisplayPosY;
		}		
		GUI.Box(new Rect(0, 0, 300, 500), "", selectStudentBoxStyle);
		
		if(GUI.Button(new Rect(5, 5, 290, 20), "", selectStudentButtonStyle)){
			if(playerID == 0){
				showPlayerOneStudentList = !showPlayerOneStudentList;
				Debug.Log("Show one student list = " + showPlayerOneStudentList);
			}else{
				showPlayerTwoStudentList = !showPlayerTwoStudentList;
				Debug.Log("Show two student list = " + showPlayerTwoStudentList);
			}
		}
		
		GUI.Label(new Rect(10, 5, 280, 20), "Player " + (playerID + 1) +": " + currentStudentName, boxTextStyle);
		
		if(studentList != null){
			scrollPosition = GUI.BeginScrollView(new Rect(20, 30, 260, 400), scrollPosition, new Rect(0, 0, 250, (studentList.Length * 25)), false, false);
				for(int x = 0; x < studentList.Length; x++){
					if(GUI.Button(new Rect(0, x * 25, 280, 25), studentList[x].text, listStyle)){
						//userSelectedConfigFile = studentList[x].text;
						handleStudentNameSelected(playerID, (Student)studentListArray[x]);
					}	
				}
		
			GUI.EndScrollView();
		}
		
		GUI.EndGroup();
	}
	
	
	public void handleStudentNameSelected(int player, Student selectedStudent){
	
		Debug.Log("You selected student named: " + selectedStudent.name);
		
		if(player == 0){
			currentPlayerOneStudent = selectedStudent.name;
			showPlayerOneStudentList = false;
			string []values = {"Player 1", selectedStudent.id}; // create an array based on the current position of this object
			// do something with player one here!
			//dataLoggerScript.WriteLogValues<string>(values); // actually write the data to the file
		}
		else{
			currentPlayerTwoStudent = selectedStudent.name;
			showPlayerTwoStudentList = false;
			string []values = {"Player 2", selectedStudent.id}; // create an array based on the current position of this object
			// do something with player two here!
			//dataLoggerScript.WriteLogValues<string>(values); // actually write the data to the file
		}
	}
		
	
	
//========================================================//	
//========================================================//	
	public void LoadXMLConfigFileFromDisc(string configFile, string baseNodeTagPath){
		
		try{
			currentXMLDoc = new XmlDocument();
			currentXMLDoc.Load(pathToConfigFilesRoot + "/" + configFile);
		}catch(System.Exception e){
		
			Debug.Log(e.ToString());
			
		}
            
		//return inputDimensions;
	}
	
	
	private void SaveCurrentXMLDocToConfigFile(string file){
		try{			
			currentXMLDoc.Save(file); // write the new data out	
		}catch(System.Exception e){
		
			Debug.Log(e.ToString());
			
		}
	}
	
	public void SaveCurrentXMLDocToNewConfigFile(){
		
	}
	
	public void SaveCurrentXMLDocToCurrentConfigFile(string configFile, string baseNodeTagPath){
			
		SaveCurrentXMLDocToConfigFile(GetFullPathFromConfigFile(currentConfigFile));
			
	}
		
}
