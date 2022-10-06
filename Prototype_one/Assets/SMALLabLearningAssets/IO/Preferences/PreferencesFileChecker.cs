using UnityEngine;
using System.Collections;
using System.IO;

public class PreferencesFileChecker : MonoBehaviour {
	
	bool isPreferencesFilePresent = false;
	bool isPreferencesFileFoundWarningDismissed = false;
	bool isSerialNumberValid = false;
	
	SerialNumberValidation serialNumberValidation;
	
	//public GUIStyle warningGUIStyle;
	public GUIStyle boxStyle;
	//public GUIStyle okButtonStyle;
	
	string preferencesFilePath = "";
	
	string serialNumberUserInput = "";
	
	// Use this for initialization
	void Start () {
	
		
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor){
			preferencesFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) 
			          + "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml";
		}else{
			preferencesFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) 
			          + "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml";
		}
			
		// check to see if we actually have a preferences file
		isPreferencesFilePresent = CheckForPreferencesFile();
		
			
		if(isPreferencesFilePresent){ // if we have a pref file, check the serial number
			serialNumberValidation = gameObject.GetComponent<SerialNumberValidation>();
			serialNumberValidation.serialValidationResponseEventHandler += handleServerResponseFromSerialNumberCheck;

			serialNumberValidation.SetPreferenceFile(preferencesFilePath);	
			//isSerialNumberValid = serialNumberValidation.CheckIfSerialNumberIsValid();
			serialNumberValidation.CheckIfSerialNumberIsValid(); // fire event to check if the serial number is valid or not, then listen for the response
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	bool CheckForPreferencesFile(){
		
		return File.Exists(preferencesFilePath);
	}

	void handleServerResponseFromSerialNumberCheck(string serialNumber, bool isValid, string errorMessage, bool connectedToLicenseServer){
		//Debug.Log("GOT IT!!!");
		
		//if (checkIfSerialNumberIsValid(serialNumberInput)){ // make sure that we have an actual valid float input
		if(isValid){
			/*
			currentErrorForSerialNumber = "";
			currentCustomerNameForSerialNumber = customerName;
			currentCustomerSystemNameForSerialNumber = customerComputer;
			currentProductName = productName;
			mySerialNumber = serialNumber;
			//shouldShowCustomerInfoForSerialNumber = true;
			ShowSerialNumberInfoDisplayPanel(true);
			*/
		} else if(!connectedToLicenseServer){

			handleNonValidSerialNumberInput(errorMessage);

		} else{
			
			handleNonValidSerialNumberInput(errorMessage);
			
		}
		
		
		
	}
	
	void handleNonValidSerialNumberInput(string errorMessage){
		//invalidSerialNumber = mySerialNumber;

		/*
		mySerialNumber = "";
		userInput = mySerialNumber;
		serialNumberInput = mySerialNumber;
		writeSerialNumberToPreferences();

		currentErrorForSerialNumber = errorMessage;
		//currentErrorForSerialNumber = "Invalid Serial Number.  Please enter again.";
		*/
		Debug.Log("Not valid serial number - Error Message: " + errorMessage);	

		/*
		UpdateSerialNumberDisplayInfo();
		ShowSerialNumberInfoDisplayPanel(true);
		*/
	}

	
	bool CheckIfSerialNumberIsValid(){
		
		return false;
	}
	
	
	void OnGUI(){
		/*
		GUI.depth = -8;
		if(!isPreferencesFilePresent && !isPreferencesFileFoundWarningDismissed){
						
			GUI.BeginGroup(new Rect(Screen.width/2 - 300, Screen.height/2 - 200, 600, 300));
				GUI.Box(new Rect(0, 0, 600, 300), "Warning", boxStyle);
				GUI.Label(new Rect(30, 30, 540, 200), "Unable to locate your preferences file.  This may cause unexpected problems running the scenario.  Please update your system configuration.  Expecting to find the preferences file here: \n\n" + Path.GetFullPath(preferencesFilePath));
				if(GUI.Button(new Rect(250, 220, 100, 40), "OK")){
					Application.Quit();
					//isPreferencesFileFoundWarningDismissed = true;
				}
			GUI.EndGroup();
		}else if(!isSerialNumberValid){
				
			GUI.BeginGroup(new Rect(Screen.width/2 - 300, Screen.height/2 - 200, 600, 300));
				GUI.Box(new Rect(0, 0, 600, 300), "Invalid Serial Number", boxStyle);
				GUI.Label(new Rect(40, 120, 540, 30), "Please enter your serial number:");
				serialNumberUserInput = GUI.TextField(new Rect(40, 150, 240, 20), serialNumberUserInput, 50);
		
				if(GUI.Button(new Rect(190, 220, 100, 20), "Save")){
					serialNumberValidation.SetNewSerialNumber(serialNumberUserInput);
					isSerialNumberValid = serialNumberValidation.CheckIfSerialNumberIsValid();
					if(isSerialNumberValid){
						serialNumberValidation.writeSerialNumberToPreferences();
					}
				}
				if(GUI.Button(new Rect(310, 220, 100, 20), "Quit")){
					Application.Quit();
				}
			GUI.EndGroup();
		}
		*/
		
	}
	
}
