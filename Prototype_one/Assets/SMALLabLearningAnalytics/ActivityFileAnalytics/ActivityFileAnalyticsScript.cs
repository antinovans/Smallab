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
using System.Text;

public class ActivityFileAnalyticsScript : MonoBehaviour {

    [Header("ScriptVersion 3.5")]
    [Space(10)]

    bool isPreferencesFilePresent = false;
	string preferencesFilePath = "";


	[System.NonSerialized]
	public string API_KEY = "5F5048A8-18AF-11E1-9548-B8614824019B";
	
	
	string pathToPrefFile = "";
	private WWW serverRequest;

    //private string serverBaseUrl = "http://resources.smallablearning.com";
    //private string serverBaseUrl = "https://resources.com";

    ConfigFilePanelScript configFilePanelScript;
    private ScenarioAnalyticsScript scenarioAnalyticsScript;

    private bool requestIsProcessing = false;

    void Awake(){
        

        scenarioAnalyticsScript = GameObject.Find("ScenarioAnalytics").GetComponent<ScenarioAnalyticsScript>();

        configFilePanelScript = GameObject.Find("Canvas/ConfigurationDisplay").GetComponent<ConfigFilePanelScript>();
        configFilePanelScript.ConfigFileSelected += HandleConfigFileSelected; // add a delegate event handler so we know when something happened
        configFilePanelScript.ConfigOptionChanged += HandleConfigOptionChanged; // add a delegate event handler so we know when something happened

    }

    // Use this for initialization
    void Start () {

        //LogNewActivityFileLoaded("Realnumbers");
    }


	
	// Update is called once per frame
	void Update () {
	
	}


    public void HandleConfigFileSelected(string filename)
    {

        Debug.Log("In ActivityFileAnalyticsScript -- Got this selected config file: " + filename);
        LogNewActivityFileLoaded(filename);
    }

    public void HandleConfigOptionChanged()
    {
        /* FOR NOW WE DON'T DO ANYTHING ON THIS OPTION */

    }


    private void HandleNoPreferencesFile(){
		Debug.Log("No preferences file found!");

		//UpdatePanelText("Error", "Unable to locate your preferences file. Please run the SMALLab Calibration scenario and try again.", "OK");
		//handleServerResponseFromSerialNumberCheck("", true, false, true, "Unable to locate your preferences file. Please check your system configuration and try again.", false);

		//ShowLicenseCheckMessagePanel(true);
	}

	void handleServerResponseFromLogActivityFileLoaded(bool success, string errorMessage, bool connectedToLicenseServer){


        Debug.Log("Logging Success: " + success + ", Error: " + errorMessage + ", Server Connected: " + connectedToLicenseServer);

        requestIsProcessing = false;
	}
	
	
	

	public void LogNewActivityFileLoaded(string activityFilePath){

        string activityFileName = Path.GetFileNameWithoutExtension(activityFilePath);

        // for activities that have a file (rather than a directory) we need to figure out the path
        if (activityFilePath.EndsWith(".txt") || activityFilePath.EndsWith(".csv") || activityFilePath.EndsWith(".json") || activityFilePath.EndsWith(".xml"))
        {

            activityFilePath = Path.GetDirectoryName(activityFilePath); // since this will be a .txt file, we need to grab the folder that contains the file
        }


        

        int activityFileId = -1;

        if (Directory.Exists(activityFilePath)) // see if we're dealing with a directory
        {
            if(File.Exists(activityFilePath + "/.info.json")) // check if it has a .info file that we can read
            {
                JSONObject infoJSON = new JSONObject(ReadJSONDataFromTextFile(activityFilePath + "/.info.json"));

                activityFileId = (int)infoJSON.GetField("id").n;
                activityFileName = infoJSON.GetField("name").str;
            }
        }

        Debug.Log("Logging new activity file loaded Current serial number = " + scenarioAnalyticsScript.serialNumber + ", loaded activity file: " + activityFileName);

        //Note: your data can only be numbers and strings.  This is not a solution for object serialization or anything like that.
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("API_key", scenarioAnalyticsScript.API_KEY); // get the api key from the analytics script                                              //string
        j.AddField("serialnumber", scenarioAnalyticsScript.serialNumber);
        j.AddField("scenario_id", scenarioAnalyticsScript.scenarioID);
        j.AddField("user_id", scenarioAnalyticsScript.currentUserId);
        // always pass the device udid		
        j.AddField("device_udid", GenerateDeviceUDID());
        j.AddField("session_id", scenarioAnalyticsScript.sessionId);
        j.AddField("activity_file_id", activityFileId);
        j.AddField("activity_file_name", activityFileName);
        string jsonEncodedString = j.print();
        //Debug.Log("log activity: " + jsonEncodedString);


        if (!requestIsProcessing)
        {
            StartRequest(jsonEncodedString);
        }
        else
        {
            StartCoroutine(WaitForPreviousRequestToFinish(jsonEncodedString));
        }

        /*
        StartCoroutine(CheckLicenseServerConnection(
            (isConnected) => { // first, make sure that the machine can connect to the license server
                if (isConnected)
                {
                    Debug.Log("We're connected and will make request to log activity file loaded");

                    if (!requestIsProcessing) // if there are no server requests in progress, go ahead and process the request
                    {
                        StartCoroutine(MakeRequestToLogActivityFileLoaded(jsonEncodedString)); // if it can proceed
                    }
                    else // if we're in the middle of processing another request, wait for it to complete and then send it
                    {
                        Debug.LogWarning("Waiting so we don't interrupt a request in progress");
                        StartCoroutine(WaitForPreviousRequestToFinish(jsonEncodedString));
                    }
                }
                else
                {
                    handleServerResponseFromLogActivityFileLoaded(false, "Unable to connect to server", false);
                }
            }
        ));
        */

    }

    IEnumerator WaitForPreviousRequestToFinish(string jsonEncodedString)
    {
        while (requestIsProcessing)
        {
            Debug.Log("Waiting for previous request to finish");
            yield return new WaitForSeconds(0.2f);
        }

        StartRequest(jsonEncodedString);
    }

    private void StartRequest(string jsonEncodedString)
    {
        requestIsProcessing = true;

        StartCoroutine(CheckLicenseServerConnection(
               (isConnected) =>
               { // first, make sure that the machine can connect to the license server
                    if (isConnected)
                   {
                       Debug.Log("We're connected and will make request to log activity file loaded");
                       StartCoroutine(MakeRequestToLogActivityFileLoaded(jsonEncodedString)); // if it can proceed
                    }
                   else
                   {
                       handleServerResponseFromLogActivityFileLoaded(false, "Unable to connect to server", false);
                   }
               }
           ));
    }

	public IEnumerator MakeRequestToLogActivityFileLoaded(string jsonEncodedString){

        Debug.Log("Sending request to log activity file loaded");
        

        Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
		//postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());


		
		this.serverRequest = new WWW(scenarioAnalyticsScript.serverBaseUrl + "/api/v1/analytics/activity_file_loaded?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);
        Debug.Log("Sending this request: " + jsonEncodedString + " to: " + this.serverRequest.url);

        yield return serverRequest;
		
		Debug.Log("Text: " + serverRequest.text);
		//print("returned data: " + serverRequest.data.Replace("\n", "========="));
		
		JSONObject j = new JSONObject(serverRequest.text);
		JSONObject contentObj = j["content"]; // get the content for this returned data set
		JSONObject statusObj = j["status"]; 
		JSONObject errorObj = j["error"]; // get the content for this returned data set


        handleServerResponseFromLogActivityFileLoaded(statusObj.GetField("success").b, errorObj.str, true);

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

    private string GenerateDeviceUDID()
    {
        return SystemInfo.deviceUniqueIdentifier;
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
	
}
