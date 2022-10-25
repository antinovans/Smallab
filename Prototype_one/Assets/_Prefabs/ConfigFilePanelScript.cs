using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Text;


using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ConfigFilePanelScript : MonoBehaviour {

	public AudioClip itemSelectedAudioClip;

	string currentConfigFile = "";
	string userSelectedConfigFile = "";

	public Vector2 scrollPosition = Vector2.zero;

	List <ConfigFileData> fileListArray = new List<ConfigFileData>();
	bool configFilesRootFolderExists = false;
	string pathToConfigFilesRoot = "";

	/** config files must be parallel to the root of the application folder
	 * If the application is named 'DogsAndCats'
	 * the convention is for config files to be contained in a folder 'DogsAndCats_Media'
	 * that sits parallel to the application folder
	 */

	public string ConfigFilesRootFolder = "";
	bool showConfigFileSelectionPrompt = false;
	bool configFilesFound = false;

	// these are events that other scripts can register as listeners
	// to register a listener from another script, insert the following in your code:
	/**
	 * In your header: ConfigFilePanelScript configFileIOScript;
	 * 
	 * In your Start() method:
	 * configFileIOScript = GameObject.Find("Canvas/ConfigurationDisplay").GetComponent<ConfigFilePanelScript>();
	 * configFileIOScript.ConfigFileSelected += HandleConfigFileSelected; // add this as a listener (HandleConfigFileSelected(string configFile) should be a method in your custom script that receives the notification
	 * 
	 * Make sure that you have a method in your script: void HandleConfigFileSelected(string configFile){}
	 * 
	 * to load the stored xml file, call this next: configFileIOScript.ReadCurrentConfigFilePref()
	 * 
	*/

	
	public delegate void ConfigFileSelectedEventHandler(string filename);
	public event ConfigFileSelectedEventHandler ConfigFileSelected;

	public delegate void ConfigOptionChangedEventHandler();
	public event ConfigOptionChangedEventHandler ConfigOptionChanged;

	int maxNumOrderChecks = 3;
	bool enableGameTimers = true;

	string dataPath = "";

	GameObject configBlackoutBackground;

	int gameDuration = 60;

	string submittedFilename;
	[SerializeField]
	Button exitButton;
	[SerializeField]
	InputField gameDurationInput;
	GameObject gameDurationInputPanel;
	[SerializeField]
	Text gameDurationLabel;
	[SerializeField]
	Text currentActivityText;

	Toggle gameTimersToggle;

    #region ActivityLaunchHelpers
    LaunchFromActivityFileIdLinkScript linkLaunchScript;
    ServerActivityFileAccessScript serverActivityFileAccessScript;
    ScenarioAnalyticsScript scenarioAnalyticsScript;
    ActivityLoadingErrorDisplayScript activityLoadingErrorDisplayScript;
    ActivityDownloadingDisplayScript activityDownloadingDisplayScript;

    // COMMENTED OUT BECAUSE PLUGIN REQUIRES PAID LICENSE
    //ZipFileExtractionScript zipFileExtractionScript;
    #endregion

    // Use this for initialization
    void Awake () {
		if (exitButton)
			exitButton.onClick.AddListener (() => {
				UpdateShowConfigPrompt ();
			});
		configBlackoutBackground = GameObject.Find("Canvas/ConfigurationDisplay");

		gameDurationInput = GameObject.Find("Canvas/ConfigurationDisplay/ConfigurationPanel/GameDurationInput").GetComponent<InputField>();

		gameDurationInputPanel = GameObject.Find("Canvas/ConfigurationDisplay/ConfigurationPanel/GameDurationInput");

		gameDurationLabel = GameObject.Find("Canvas/ConfigurationDisplay/ConfigurationPanel/GameDurationLabel").GetComponent<Text>();

		gameTimersToggle = GameObject.Find("Canvas/ConfigurationDisplay/ConfigurationPanel/UseGameTimersToggle").GetComponent<Toggle>();

        // ActivityLaunchHelpers 
        linkLaunchScript = GetComponent<LaunchFromActivityFileIdLinkScript>();
        serverActivityFileAccessScript = GetComponent<ServerActivityFileAccessScript>();
        scenarioAnalyticsScript = GameObject.Find("!StandardComponents/ScenarioAnalytics").GetComponent<ScenarioAnalyticsScript>();
        activityLoadingErrorDisplayScript = GameObject.Find("Canvas/ActivityLoadingErrorDisplay").GetComponent<ActivityLoadingErrorDisplayScript>();
        activityDownloadingDisplayScript = GameObject.Find("Canvas/ActivityDownloadingDisplay").GetComponent<ActivityDownloadingDisplayScript>();

        // COMMENTED OUT BECAUSE PLUGIN REQUIRES PAID LICENSE
        //zipFileExtractionScript = GetComponent<ZipFileExtractionScript>();


    }


    void Start(){

		if(!PlayerPrefs.HasKey ("GameDuration")){
			PlayerPrefs.SetInt("GameDuration", gameDuration);
		}

		gameDuration = PlayerPrefs.GetInt ("GameDuration");
/*		gameDurationInput.text = gameDuration.ToString();*/


		if(PlayerPrefs.HasKey ("EnableGameTimers")){
			enableGameTimers = PlayerPrefs.GetInt ("EnableGameTimers")==1?true:false;
		}
		gameTimersToggle.isOn = enableGameTimers;
		EnableGameDurationInput(enableGameTimers);

		//gameDurationLabel.text = "Game Duration: " + gameDuration.ToString("F1");

		

		dataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/SMALLabLearning/" + ConfigFilesRootFolder;

		if(Directory.Exists(dataPath)){
			Debug.Log("directory " + dataPath + " exists!");
			configFilesRootFolderExists = true;
			//	pathToConfigFilesRoot = Application.dataPath + "/../../" + ConfigFilesRootFolder;
			pathToConfigFilesRoot = dataPath;
			ReadCurrentConfigFilePref();

		}else{ // if the root directory doesn't exist, create it
			string newDir = dataPath;
			Debug.Log ("Creating the root media directory here: " + dataPath);
			Directory.CreateDirectory(newDir);
			pathToConfigFilesRoot = dataPath;
			configFilesRootFolderExists = true;

		}

        /***
         * LEAVE COMMENTED OUT UNLESS SUPPORTING LAUNCH FROM RESOURCES SITE WITH FILE
        // first, try to launch it using incoming parameters from a link
        int launchLinkActivityFileId = linkLaunchScript.GetActivityFileIdFromLaunchParameters();
        if (launchLinkActivityFileId != -1)
        {
            Debug.Log("Got launch parameters for activity file id: " + launchLinkActivityFileId);
            // if we have a valid id from the launch update our internal user id
            int userId = linkLaunchScript.GetUserIdFromLaunchParameters();
            if (userId != -1)
            {
                scenarioAnalyticsScript.currentUserId = userId;
            }

            // since we have an id to mess with, go information from the server
            serverActivityFileAccessScript.GetActivityFileDataFromServer(launchLinkActivityFileId);
        }
        else{
            // if there aren't any new launch link parameters, load the previously stored file 
            Debug.Log("Will initialize the config with a stored file");
            InitWithStoredFile();

            // if it's there, go ahead and generate a list of files that we can use
            if (configFilesRootFolderExists)
            {
                Debug.Log("Config files root folder DOES exist");
                GenerateListOfConfigFiles();
                //GenerateListOfFolders();

                
            }
            else
            {
                Debug.Log("WARNING: Config files root folder does not exist");
            }
            


        }
        *
        * 
        *****/
	}

	public void ReadCurrentConfigFilePref(){

		currentConfigFile = PlayerPrefs.GetString("CurrentConfigFile");

		// if it's valid go ahead and process, alerting any listeners
		if (currentConfigFile != null) {
			Debug.Log ("Loading configuration file from preferences here: " + currentConfigFile);
		}
	}

	public void InitWithStoredFile(){

		currentConfigFile = PlayerPrefs.GetString("CurrentConfigFile");

		
        UpdateCurrentActivityTextFromCurrentConfigFile();
        

        if (currentConfigFile != null)
			ProcessNewFileSelected();
	}

    void UpdateCurrentActivityTextFromCurrentConfigFile()
    {
        if (currentConfigFile.IndexOf("_cache") == -1)
        {
            currentActivityText.text = currentConfigFile;
        }
        else
        {
            currentActivityText.text = GetActivityFileNameFromInfo(GetFullPathFromConfigFile(currentConfigFile));
        }
    }

	void StoreCurrentConfigFilePref(){

		PlayerPrefs.SetString("CurrentConfigFile", currentConfigFile);

	}

	// Update is called once per frame
	void Update () {

	}

    public void HandleVideoStreamDataFromServer(bool success, string errorMessage, bool connectedToLicenseServer, JSONObject data)
    {
        
    }

    public void HandleActivityFileDataFromServer(bool success, string errorMessage, bool connectedToLicenseServer, JSONObject data)
    {
        Debug.Log("Got data about the activity file");
        if (success)
        {
            JSONObject activityFileData = data["activityfile"];
            string downloadBaseUrl = data["download_base_url"].str;

            if (CheckIfNeedToDownloadActivityFile(activityFileData))
            {
                Debug.Log("We will download your activity!");
                // download the file
                StartCoroutine(DownloadActivityFileZip(activityFileData, downloadBaseUrl));
            }
            else // if the current cached file is up to date, just use it without a download
            {
                Debug.Log("DO NOT Need to download your activity...");

                int activityFileId = (int)activityFileData.GetField("id").n;

                currentConfigFile = "_cache/" + activityFileId.ToString();

                StoreCurrentConfigFilePref();
                RefreshMenu();


                UpdateCurrentActivityTextFromCurrentConfigFile();

                ProcessNewFileSelected();
            }
        }
        else
        { // if we get an error trying to grab data from the server, just load from the previous version

            errorMessage = errorMessage + "\n\nPlease try your request again and contact SMALLab Learning Support if this problem persists";

            activityLoadingErrorDisplayScript.UpdatePanelText("Error Accessing the Activity File", errorMessage, "OK");
            activityLoadingErrorDisplayScript.ShowMessagePanel(true);

            //shouldAutoPlayVideoWhenReady = false;

            InitWithStoredFile();
            RefreshMenu();
            ProcessNewFileSelected();
        }
    }

    private bool CheckIfNeedToDownloadActivityFile(JSONObject activityFileData)
    {

        int activityFileId = (int)activityFileData.GetField("id").n;
        string activityFilePath = GetFullPathToCacheDirectoryConfigFile(activityFileId.ToString());

        // see if the path exists
        if (Directory.Exists(activityFilePath)) // see if we're dealing with a directory
        {
            if (File.Exists(activityFilePath + "/.info.json")) // check if it has a .info file that we can read
            {
                JSONObject infoJSON = new JSONObject(ReadJSONDataFromTextFile(activityFilePath + "/.info.json"));

                DateTime localLastUpdatedDate = DateTime.Parse(infoJSON.GetField("updated_at").str);
                DateTime remoteLastUpdatedDate = DateTime.Parse(activityFileData.GetField("updated_at").str);

                if(localLastUpdatedDate < remoteLastUpdatedDate) // if the remote timestamp is newer than the local cached one, we need to download
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
        
        
        return true;
    }


    IEnumerator DownloadActivityFileZip(JSONObject activityFileData, string downloadBaseUrl)
    {
        string activityFileZipUrl = downloadBaseUrl + activityFileData.GetField("filepath").str;

        int activityFileId = (int) activityFileData.GetField("id").n;

        Debug.Log("Will download this zip file: " + activityFileZipUrl);
        //string savedFilePath = SaveVideoStreamDataFromServerToConfigFile(videostreamData);
        activityDownloadingDisplayScript.UpdatePanelText("Please Wait - Loading Activity", "Downloading activity file...", "Cancel");
        activityDownloadingDisplayScript.ShowMessagePanel(true);

        var www = new WWW(activityFileZipUrl);
        yield return www;

        string errorMessage = "";

        if (www.error != null) // check that we don't have an error during the download process
        {
            Debug.Log(www.error);
            errorMessage = "We encountered an error downloading your activity. \n\nPlease try again.\n\n" + www.error;
            activityLoadingErrorDisplayScript.UpdatePanelText("Error Loading the Activity File", errorMessage, "OK");
            activityLoadingErrorDisplayScript.ShowMessagePanel(true);
        }

        activityDownloadingDisplayScript.UpdatePanelText("Please Wait - Loading Activity", "Storing downloaded file...", "Cancel");
        
        // write the zip file to a temp location
        string pathToTempZipFile = GetFullPathToTempStorageDirectory(activityFileId.ToString() + ".zip");
        Debug.Log("Will write the zip file here: " + pathToTempZipFile);
        File.WriteAllBytes(pathToTempZipFile, www.bytes);

        activityDownloadingDisplayScript.UpdatePanelText("Please Wait - Loading Activity", "Decompressing files...", "Cancel");


        // unzip the file
        string pathToTempExtractedFiles = GetFullPathToTempStorageDirectory(activityFileId.ToString());
        Debug.Log("Will extract the zip file to here: " + pathToTempExtractedFiles);

        // COMMENTED OUT BECAUSE THE ZIP DECOMPRESSION REQUIRES PAID LICENSE
        //zipFileExtractionScript.DecompressFile(pathToTempZipFile, pathToTempExtractedFiles);

        
        
        // However, if we're extracting something like memory that contains a subfolder, we need to move the internal files into the root dir
        Debug.Log("Done extracting the files");

        activityDownloadingDisplayScript.UpdatePanelText("Please Wait - Loading Activity", "Installing the activity...", "Cancel");

        string pathToCachedFiles = GetFullPathToCacheDirectoryConfigFile(activityFileId.ToString());

        // if we're extracting for something like spinner that just contains a file, we would just get the extracted file and move it
        // for activities like memory, we need to get the subfolder and move it to the cache location
        foreach (string file in Directory.GetDirectories(pathToTempExtractedFiles))
        {
            // move the subfolder to the new location in the cache
            Debug.Log("Found this directory after the extraction: " + file);

            // if we already have a folder, clear it so we can replace
            if (Directory.Exists(pathToCachedFiles))
            {
                Directory.Delete(pathToCachedFiles, true);
            }

            Directory.Move(file, pathToCachedFiles); // move it to the new location

        }

        WriteInfoFileToDirectory(activityFileData, pathToCachedFiles);

        // CLEANUP
        // delete the original download from the temp location
        File.Delete(pathToTempZipFile); // delete the original zip file

        // delete the temp extracted folder
        Directory.Delete(pathToTempExtractedFiles, true);


        currentConfigFile = "_cache/" + activityFileId.ToString();
        

        StoreCurrentConfigFilePref();
        RefreshMenu();
        UpdateCurrentActivityTextFromCurrentConfigFile();
        //currentActivityText.text = currentConfigFile.GetField("display_name").str;
        ProcessNewFileSelected();

        //yield return new WaitForSeconds(4.0f);

        // hide the download display
        activityDownloadingDisplayScript.ShowMessagePanel(false);

        /*
        // show an error message if the download fails
        errorMessage = "We encountered an error downloading your activity. \n\nPlease try again.";
        activityLoadingErrorDisplayScript.UpdatePanelText("Error Loading the Activity File", errorMessage, "OK");
        activityLoadingErrorDisplayScript.ShowMessagePanel(true);
        */
    }

    private string GetActivityFileNameFromInfo(string folderpath)
    {
        if (File.Exists(folderpath + "/.info.json")) // make sure that there is an info file to read
        {

            JSONObject infoJSON = new JSONObject(ReadJSONDataFromTextFile(folderpath + "/.info.json"));
            return infoJSON.GetField("name").str;
        }
        else
        {
            return Path.GetFileName(folderpath);
        }

    }

    private string ReadJSONDataFromTextFile(string filepath)
    {

        string orderPrompt = "";

        // before we try to extract the images, make sure that the folder is there		
        if (File.Exists(filepath))
        {

            // Handle any problems that might arise when reading the text
            try
            {
                string line;
                // Create a new StreamReader, tell it which file to read and what encoding the file
                // was saved as
                StreamReader theReader = new StreamReader(filepath, Encoding.UTF8);

                // Immediately clean up the reader after this block of code is done.
                // You generally use the "using" statement for potentially memory-intensive objects
                // instead of relying on garbage collection.
                // (Do not confuse this with the using directive for namespace at the 
                // beginning of a class!)
                using (theReader)
                {
                    // While there's lines left in the text file, do this:
                    do
                    {
                        line = theReader.ReadLine();

                        if (line != null)
                        {
                            orderPrompt += line.Trim();
                            //Debug.Log ("Read this line from the text file: " + line);
                        }
                    }
                    while (line != null);

                    // Done reading, close the reader and return true to broadcast success    
                    theReader.Close();

                }
            }

            // If anything broke in the try block, we throw an exception with information
            // on what didn't work
            catch (Exception e)
            {
                Console.WriteLine("{0}\n", e.Message);
                return orderPrompt;
            }
        }


        return orderPrompt;
    }

    public void WriteInfoFileToDirectory(JSONObject activityFileData, string pathToCachedFiles)
    {
        JSONObject jsonInfo = new JSONObject();
        jsonInfo.AddField("id", (int)activityFileData.GetField("id").n);
        jsonInfo.AddField("user_id", (int)activityFileData.GetField("created_by").n);
        jsonInfo.AddField("name", Path.GetFileNameWithoutExtension(activityFileData.GetField("filepath").str));
        jsonInfo.AddField("created_at", activityFileData.GetField("created_at").str);
        jsonInfo.AddField("updated_at", activityFileData.GetField("updated_at").str);

        File.WriteAllText(pathToCachedFiles + "/.info.json", jsonInfo.ToString());
    }

    public void HandleCancelActivityFileDownload()
    {
        Debug.Log("Will cancel your download");
    }

    

	public void RefreshMenuButtonPressed(){
		RefreshMenu();
	}

	void RefreshMenu(){
		// if it's there, go ahead and generate a list of files that we can use
		if(configFilesRootFolderExists){
			GenerateListOfConfigFiles();


			


		}
	}

	void HighlightCurrentSelectedConfigFileInList(){
		int selectedIndex = DetermineIndexOfSelectedConfigFile();

		for (var i = 0; i < fileListArray.Count; i++)
		{	
			if(i == selectedIndex){
				// set it to selected if desired
				fileListArray[i].Selected = true;
			}else{
				fileListArray[i].Selected = false;
			}
		}
	}

	int DetermineIndexOfSelectedConfigFile(){
		for (var i = 0; i < fileListArray.Count; i++)
		{	
			if(fileListArray[i].filepath == currentConfigFile){
				return i;
			}
		}

		return 0;
	}

	

	

	void GenerateListOfConfigFiles(){

		// get rid of the current list so that we can update it
		fileListArray.Clear();
		
		foreach (string file in Directory.GetDirectories(pathToConfigFilesRoot)){
			if (Path.GetFileName(file) != "_temp") // don't read files from the _temp directory
            {
                if (Path.GetFileName(file) == "_cache") // look inside the cache directory too
                {
                    foreach (string cache_file in Directory.GetDirectories(pathToConfigFilesRoot + "/_cache"))
                    {

                        if (File.Exists(cache_file + "/.info.json")) // make sure that there is an info file to read
                        {

                            JSONObject infoJSON = new JSONObject(ReadJSONDataFromTextFile(cache_file + "/.info.json"));
                            fileListArray.Add(new ConfigFileData("_cache/" + Path.GetFileName(cache_file), infoJSON.GetField("name").str)); // strip it down to just the filename
                        }
                        else
                        {
                            // if we don't find the info.json file for some reason, just default to using the path name
                            fileListArray.Add(new ConfigFileData("_cache/" + Path.GetFileName(cache_file), Path.GetFileName(cache_file)));
                        }
                    }
                }
                else
                {
                    fileListArray.Add(new ConfigFileData(Path.GetFileName(file), Path.GetFileName(file))); // strip it down to just the filename
                }                                                        //print(file);
            }
			
		}
		
		// make sure that we actually found some config files
		if(fileListArray.Count > 0){
			//fileListArray.Insert(0, ""); // put a blank one at the top so the user isn't required to select an option
			configFilesFound = true;
		}
	}

    

	public void HandleGameDurationChanged(){
		int tempValue = 0;
		if(int.TryParse(gameDurationInput.text, out tempValue)){
			gameDuration = tempValue;
			SaveGameDuration(gameDuration);
			ConfigOptionChanged();
		}
				//gameDuration = float gameDurationInput.text;
		//gameDurationLabel.text = "Game Speed: " + gameDuration.ToString("F1");

	}

	private void EnableGameDurationInput(bool enable){
		gameDurationInputPanel.SetActive(enable);
		gameDurationLabel.enabled = enable;
	}

	public void HandleEnableGameTimersValueChanged(){

		if(enableGameTimers != gameTimersToggle.isOn){

			enableGameTimers = gameTimersToggle.isOn;
			EnableGameDurationInput(enableGameTimers);
			PlayerPrefs.SetInt ("EnableGameTimers", enableGameTimers?1:0);

			ConfigOptionChanged();
		}

	}

	void HandleConfigurationFileSelected(){

		//Debug.Log ("In handleConfigurationFileSelected");

		// make sure that the user has made a valid selection
		if(userSelectedConfigFile != "" && currentConfigFile != userSelectedConfigFile){

			// play audio so the user knows something has been selected
			GetComponent<AudioSource>().PlayOneShot(itemSelectedAudioClip);

			currentConfigFile = userSelectedConfigFile; // update the currentConfigFile to the one selected by the user

			// store this in player prefs so we'll remember it next time around
			StoreCurrentConfigFilePref();

            UpdateCurrentActivityTextFromCurrentConfigFile();
            
			

			ProcessNewFileSelected();
		}

	}

	void GenerateDefaultConfiguration(){


		Debug.Log ("Generating a Default Image Set");

		currentConfigFile = "Default";

		StoreCurrentConfigFilePref (); // store it for later use

		if (!Directory.Exists (GetFullPathFromConfigFile (currentConfigFile))) {
			// make the default directory
			Directory.CreateDirectory (GetFullPathFromConfigFile (currentConfigFile));
		}


		string [] imageFilenameArray = {"Card1_a.png", "Card1_b.png", "Card2_a.png", "Card2_b.png", "Card3_a.png", "Card3_b.png", "Card4.png"};

		for (int x = 0; x < imageFilenameArray.Length; x++) {

			Texture2D texture = Resources.Load ("DefaultActivity/" + Path.GetFileNameWithoutExtension(imageFilenameArray[x]), typeof(Texture2D)) as Texture2D;

			//Texture2D texture = Resources.Load (System.IO.Path.GetFileNameWithoutExtension ("DefaultMemoryImages/Card1_a.png")) as Texture2D;
			byte[] bytes = texture.EncodeToPNG ();
			File.WriteAllBytes (GetFullPathFromConfigFile (currentConfigFile) + "/" + imageFilenameArray[x], bytes);
		}


	

	}


	void ProcessNewFileSelected(){
		Debug.Log("Current config file: " + currentConfigFile);
		// we don't want to try to load a blank file, so make sure we generate the default file if it doesn't exist
		if (!Directory.Exists (GetFullPathFromConfigFile (currentConfigFile)) || currentConfigFile == "") {
			Debug.Log(GetFullPathFromConfigFile (currentConfigFile) + " EXISTS STATE = " + Directory.Exists (GetFullPathFromConfigFile (currentConfigFile)));
			GenerateDefaultConfiguration ();
		} 

		if (ConfigFileSelected != null){ // make sure that we have a listener
			ConfigFileSelected(GetFullPathFromConfigFile(currentConfigFile) );	
		}

		//List <string>imageFileListArray = GetImageFilesFromFolder(GetFullPathFromConfigFile(currentConfigFile));

		//ClockerRoundDefinition clockerRoundDefinition = GetRoundDefinitionFromCSVFile(GetFullPathFromConfigFile(currentConfigFile));

		//gameBrainScript.ProcessNewConfiguration(clockerRoundDefinition); // send our new list of words to the game brain
	}


    /*
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
					
					imageListArray.Add ( Path.GetFullPath (file)); // strip it down to just the filename
					//print (file);			
				}
			}
		}


		return imageListArray;
	

	}
	*/

    string GetFullPathToCacheDirectoryConfigFile(string filename)
    {
        // make sure the temp directory exists
        if (!Directory.Exists(GetFullPathFromConfigFile("_cache")))
        {
            Debug.Log("Cache directory doesn't exist, creating now");
            Directory.CreateDirectory(GetFullPathFromConfigFile("_cache"));
        }

        return GetFullPathFromConfigFile("_cache/"+ filename);

    }

    string GetFullPathToTempStorageDirectory(string filename)
    {
        // make sure the temp directory exists
        if (!Directory.Exists(GetFullPathFromConfigFile("_temp")))
        {
            Debug.Log("Temp directory doesn't exist, creating now");
            Directory.CreateDirectory(GetFullPathFromConfigFile("_temp"));
        }

        return GetFullPathFromConfigFile("_temp/" + filename);
    }

    string GetFullPathFromConfigFile(string filename){

		return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/SMALLabLearning/" + ConfigFilesRootFolder + "/" + filename;

	}

	private void SaveGameDuration(int duration){
		PlayerPrefs.SetInt ("GameDuration", duration);
	}
	bool isPlaying = false;

	private void UpdateShowConfigPrompt(){
		showConfigFileSelectionPrompt = !showConfigFileSelectionPrompt;

		/*
		if (MusicManager.Instance.isPlaying() && showConfigFileSelectionPrompt) {
			isPlaying = true;
			MusicManager.Instance.Pause ();
		}

		if (isPlaying && !showConfigFileSelectionPrompt) {
			MusicManager.Instance.UnPause ();
			isPlaying = false;
		}
		*/

			
		configBlackoutBackground.GetComponent<CanvasGroup>().interactable = showConfigFileSelectionPrompt;
		configBlackoutBackground.GetComponent<CanvasGroup>().blocksRaycasts = showConfigFileSelectionPrompt;
		configBlackoutBackground.GetComponent<CanvasGroup>().alpha = (showConfigFileSelectionPrompt) ? 1.0f : 0.0f;
	}



	void OnGUI(){
		// use this method so we can detect keyboard presses to load the panel
		GUI.depth = -9;

		// check for key events to toggle InfoDisplay activity state.  While in Editor mode.
		Event e = Event.current;
		if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.C){
			// Debug.Log("Pressed the Control-C");
			UpdateShowConfigPrompt();
			//Debug.Log("Got a command key");
		}







	}





}
