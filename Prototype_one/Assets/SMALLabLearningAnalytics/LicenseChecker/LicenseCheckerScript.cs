using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class LicenseCheckerScript : MonoBehaviour {

    [Header("ScriptVersion 2.0")]
    [Space(10)]
    public string mySerialNumber = "";

    bool isPreferencesFilePresent = false;
	string preferencesFilePath = "";

	Text errorHeader, errorMessage, errorButtonText;



	[System.NonSerialized]
	public string API_KEY = "5F5048A8-18AF-11E1-9548-B8614824019B";
	
	
	
	string pathToPrefFile = "";
	private WWW serverRequest;
    

    private ScenarioAnalyticsScript scenarioAnalyticsScript;

    bool shouldExitAndClearSerialNumber = false;

	void Awake(){

        scenarioAnalyticsScript = GameObject.Find("ScenarioAnalytics").GetComponent<ScenarioAnalyticsScript>();

        errorHeader = gameObject.transform.Find("InfoPanel/Header").GetComponent<Text>();
		errorMessage = gameObject.transform.Find("InfoPanel/Message").GetComponent<Text>();
		errorButtonText = gameObject.transform.Find("InfoPanel/OkButton/Text").GetComponent<Text>();
	}

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
			mySerialNumber = readSerialNumberFromXMLFile(); // read the serial number from the preferences file
			CheckIfSerialNumberIsValid();
		}else{
			HandleNoPreferencesFile();
		}
	}


	
	// Update is called once per frame
	void Update () {
	
	}


	private void ShowLicenseCheckMessagePanel(bool show){
		GetComponent<CanvasGroup>().interactable = show;
		GetComponent<CanvasGroup>().alpha = show ? 1.0f : 0.0f;
		GetComponent<CanvasGroup>().blocksRaycasts = show;
	}

	private void UpdatePanelText(string header, string message, string button){
		errorHeader.text = header;
		errorMessage.text = message;

		errorButtonText.text = button;
	}

	public void OkButtonPressed(){

		//Application.Quit();

		if(shouldExitAndClearSerialNumber){
			Application.Quit();
		}else{
			ShowLicenseCheckMessagePanel(false);
		}
	}

	private void HandleNoPreferencesFile(){
		//Debug.Log("No preferences file found!");

		//UpdatePanelText("Error", "Unable to locate your preferences file. Please run the SMALLab Calibration scenario and try again.", "OK");
		handleServerResponseFromSerialNumberCheck("", true, false, true, "Unable to locate your preferences file. Please check your system configuration and try again.", false);

		//ShowLicenseCheckMessagePanel(true);
	}

	void handleServerResponseFromSerialNumberCheck(string serialNumber, bool isLicenseExpired, bool isLicenseInGracePeriod, bool shouldClearLocalSerialNumber, string errorMessage, bool connectedToLicenseServer){
		Debug.Log("Response from serial number check: isLicenseExpired = " + isLicenseExpired + ", isLicenseInGracePeriod: " + isLicenseInGracePeriod + ", shouldClearLocalSerialNumber: " + shouldClearLocalSerialNumber + ", connectedToLicenseServer: " + connectedToLicenseServer);

		shouldExitAndClearSerialNumber = false;

        //if (checkIfSerialNumberIsValid(serialNumberInput)){ // make sure that we have an actual valid float input
        if(!isLicenseExpired && !isLicenseInGracePeriod && !shouldClearLocalSerialNumber){
        
            /*
			currentErrorForSerialNumber = "";
			currentCustomerNameForSerialNumber = customerName;
			currentCustomerSystemNameForSerialNumber = customerComputer;
			currentProductName = productName;
			mySerialNumber = serialNumber;
			//shouldShowCustomerInfoForSerialNumber = true;
			ShowSerialNumberInfoDisplayPanel(true);
			*/
            

        } else if(isLicenseExpired && isLicenseInGracePeriod && !shouldClearLocalSerialNumber){

            UpdatePanelText("Warning", errorMessage, "OK");
			ShowLicenseCheckMessagePanel(true);

		} else if(shouldClearLocalSerialNumber){

            shouldExitAndClearSerialNumber = true;
			handleNonValidSerialNumberInput(errorMessage);

		} else if(!connectedToLicenseServer){

			handleNonValidSerialNumberInput(errorMessage);

		} else{

			//shouldExitAndClearSerialNumber = true;
			//handleNonValidSerialNumberInput(errorMessage);
			
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

		if(shouldExitAndClearSerialNumber){
			ClearSerialNumberFromXMLFile();
			Debug.Log("Deleted local serial number");	
		}

		/*
		UpdateSerialNumberDisplayInfo();
		ShowSerialNumberInfoDisplayPanel(true);
		*/

		UpdatePanelText("Error", errorMessage, "OK");
		ShowLicenseCheckMessagePanel(true);
	}

	bool CheckForPreferencesFile(){
		
		return File.Exists(preferencesFilePath);
	}

	public void CheckIfSerialNumberIsValid(){
		
		Debug.Log("Current serial number = " + mySerialNumber);
		
		if(mySerialNumber.Length == 0){
			handleServerResponseFromSerialNumberCheck(mySerialNumber, true, false, true, "Unable to locate your serial number. Please check your system configuration and try again.", false);
		}else{
			//Note: your data can only be numbers and strings.  This is not a solution for object serialization or anything like that.
			JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
			//number
			j.AddField("API_key", API_KEY);
			//string
			j.AddField("serialnumber", mySerialNumber);
			// always pass the device udid		
			j.AddField("device_udid", GenerateDeviceUDID());
			string jsonEncodedString = j.print();
			//Debug.Log("Retrieve Customer JSON: " + jsonEncodedString);
			
		
		
			StartCoroutine(CheckLicenseServerConnection(
				(isConnected)=>{ // first, make sure that the machine can connect to the license server
				if(isConnected){
					StartCoroutine(MakeRequestIfSerialNumberIsValid(jsonEncodedString)); // if it can proceed
				}else{
					handleServerResponseFromSerialNumberCheck(mySerialNumber, false, false, false, "Unable to connect to the license server. Please check your internet connection and try again.", false);
				}
			}
			));
		}

	}

	public IEnumerator MakeRequestIfSerialNumberIsValid(string jsonEncodedString){
		
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
		//postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());


		
		this.serverRequest = new WWW(scenarioAnalyticsScript.serverBaseUrl + "/api/v1/license/check?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);

        Debug.Log("Sending this request: " + jsonEncodedString + " to: " + this.serverRequest.url);

        yield return serverRequest;
		
		Debug.Log("Text: " + serverRequest.text);
		//print("returned data: " + serverRequest.data.Replace("\n", "========="));
		
		JSONObject j = new JSONObject(serverRequest.text);
		JSONObject contentObj = j["content"]; // get the content for this returned data set
		JSONObject statusObj = j["status"]; 
		JSONObject errorObj = j["error"]; // get the content for this returned data set


		handleServerResponseFromSerialNumberCheck(mySerialNumber, statusObj.GetField("is_expired").b, statusObj.GetField("is_grace_period").b, statusObj.GetField("terminate_use").b, errorObj.str, true);

		/*
		//if(errorObj.str.Equals("")){
		if(statusObj.GetField("is_valid").b == true){
			
			retrievedCustomerName = contentObj.GetField("is_valid").ToString();
			retrievedCustomerComputer = contentObj.GetField("system_name").ToString();
			retrievedProductName = contentObj.GetField("product_name").ToString();
			//Debug.Log("Customer Name = " + retrievedCustomerName);
			//Debug.Log("Customer Computer = " + retrievedCustomerComputer);


			// this calls to the delegate
			handleServerResponseFromSerialNumberCheck(mySerialNumber, true, statusObj.GetField("is_grace_period").b "", true);
			
		}else{
			Debug.Log("Error with serial number");
			Debug.Log("Error = " + errorObj.str);

			handleServerResponseFromSerialNumberCheck(mySerialNumber, false, errorObj.str, true);
			
		}
		*/
		
		//Debug.Log("Got customer name = " + o
		
		//Debug.log(arr[2].n);
		//accessData(j);
		//returned data: {"content":{"id":"6","customer_id":"3","product_id":"2","serialnumber":"be9d34af43b578756cc08d840a79fc58","name":"Testing Serial Number"},"type":"serialnumber_customer_info","error":""}
		//retrievedCustomerName = 
		//retrievedCustomerComputer=
		
		//return serverRequest;	
	}

	IEnumerator CheckLicenseServerConnection(Action<bool> action){
		WWW www = new WWW(scenarioAnalyticsScript.serverBaseUrl);
		yield return www;
		if (!string.IsNullOrEmpty(www.error)){
			Debug.Log("Got this error with the connection to " + scenarioAnalyticsScript.serverBaseUrl + " - " + www.error);
			action (false);
		} else {
			action (true);
		}
	}

	private string GenerateDeviceUDID(){
		return SystemInfo.deviceUniqueIdentifier;
	}


	private string readSerialNumberFromXMLFile(){
		Debug.Log("Reading serial number");
		string serialNumber = "";
			
		try{
			XmlDocument doc = new XmlDocument();

			doc.Load (preferencesFilePath);
			
			XmlNodeList xnList = doc.SelectNodes("smallablearning/license");
			foreach (XmlNode xn in xnList){
 				serialNumber = xn["serialnumber"].InnerText;	
			} 
						
		}catch(System.Exception e){
		
			Debug.Log(e.ToString());
			
		}
        
		//Debug.Log("Retrieved serial number: " + serialNumber);
		
		return serialNumber;
	}

	private void ClearSerialNumberFromXMLFile(){
		try{
			
			string pathToPrefFile = "";
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor){
				pathToPrefFile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) 
				+ "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml";
			}else{
				pathToPrefFile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) 
			          + "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml";
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(pathToPrefFile);
			
			XmlNodeList xnList = doc.SelectNodes("/smallablearning/license");
			foreach (XmlNode xn in xnList){
				
				//Debug.Log("Writing this serial number: " + mySerialNumber);
				
 				xn["serialnumber"].InnerText = "";
  				
			} 
						
			doc.Save(pathToPrefFile); // write the new data out
			
		}catch(System.Exception e){
		
			Debug.Log(e.ToString());
			
		}
	}
	
	
	private string GetActiveMacAddress()
	{
		const int MIN_MAC_ADDR_LENGTH = 12;
		string macAddress = string.Empty;
		long maxSpeed = -1;
		
		foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
		{
			//Debug.Log("Found MAC Address: " + nic.GetPhysicalAddress() + " Type: " + nic.NetworkInterfaceType);
			
			string tempMac = nic.GetPhysicalAddress().ToString();
			if (nic.Speed > maxSpeed &&
			    !string.IsNullOrEmpty(tempMac) &&
			    tempMac.Length >= MIN_MAC_ADDR_LENGTH)
			{
				//Debug.Log("New Max Speed = " + nic.Speed + ", MAC: " + tempMac);
				maxSpeed = nic.Speed;
				macAddress = tempMac;
			}
		}
		
		return macAddress;
	}
	
	private string [] GetAllMacAddresses()
	{
		const int MIN_MAC_ADDR_LENGTH = 12;
		string macAddress = string.Empty;
		long maxSpeed = -1;
		
		List<string> macAddressList = new List<string> ();
		
		foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
		{
		
			Debug.Log("Found MAC Address: " + nic.GetPhysicalAddress() + " Type: " + nic.NetworkInterfaceType);
			
			macAddressList.Add (nic.GetPhysicalAddress().ToString());
			
			string tempMac = nic.GetPhysicalAddress().ToString();
			if (nic.Speed > maxSpeed &&
			    !string.IsNullOrEmpty(tempMac) &&
			    tempMac.Length >= MIN_MAC_ADDR_LENGTH)
			{
				//Debug.Log("New Max Speed = " + nic.Speed + ", MAC: " + tempMac);
				maxSpeed = nic.Speed;
				macAddress = tempMac;
			}
		}
		
		return macAddressList.ToArray();
	}
}
