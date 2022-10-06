using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScenarioVersionCheckerScript : MonoBehaviour {

    [Header("ScriptVersion 3.1")]
    [Space(10)]

    public string currentScenarioDownloadUrl;

    private bool displayThisVersionNotCurrent = false;

    private WWW serverRequest;

	private float versionNumber;
	//private InfoDisplay infoDisplayScript;
	private ScenarioAnalyticsScript scenarioAnalyticsScript;

    Text errorHeader, errorMessage, errorButtonText;
    GameObject downloadButton;
    


    void Awake(){
		scenarioAnalyticsScript = GameObject.Find("ScenarioAnalytics").GetComponent<ScenarioAnalyticsScript>();
        //infoDisplayScript = GameObject.Find("InfoDisplay").GetComponent<InfoDisplay>();	

        errorHeader = gameObject.transform.Find("InfoPanel/Header").GetComponent<Text>();
        errorMessage = gameObject.transform.Find("InfoPanel/Message").GetComponent<Text>();
        errorButtonText = gameObject.transform.Find("InfoPanel/OkButton/Text").GetComponent<Text>();

        downloadButton = gameObject.transform.Find("InfoPanel/DownloadButton").gameObject;

        LoadVersionNumberFromInfoFile();
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	

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


	void CheckForNewerVersion(){
		
		//Note: your data can only be numbers and strings.  This is not a solution for object serialization or anything like that.
		JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
		//number
		j.AddField("API_key", scenarioAnalyticsScript.API_KEY); // get the api key from the analytics script
		//string
		j.AddField("serialnumber", scenarioAnalyticsScript.readSerialNumberFromXMLFile());
		j.AddField("scenario_id", scenarioAnalyticsScript.scenarioID);
		j.AddField("version_number", versionNumber.ToString());
		
		string jsonEncodedString = j.print();

		StartCoroutine(MakeCallToServer(jsonEncodedString));	
	
	}
		
	public IEnumerator MakeCallToServer(string jsonEncodedString){
		
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
        //postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());

        //scenarioAnalyticsScript.serverBaseUrl
        Debug.Log("Checking the scenario version");
        //serverRequest = new WWW("http://analytics.smallablearning.com/check_for_newer_scenario.php?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);
        this.serverRequest = new WWW(scenarioAnalyticsScript.serverBaseUrl + "/api/v1/scenarios/check_for_newer_scenario_version", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);
        

        yield return serverRequest;
		HandleServerResponse(serverRequest);
	}
	
	void HandleServerResponse(WWW serverResponse){

        Debug.Log("Response from scenario version check: " + serverResponse.text);

        JSONObject j = new JSONObject(serverResponse.text);
        JSONObject contentObj = j["content"]; // get the content for this returned data set
        JSONObject statusObj = j["status"];
        JSONObject errorObj = j["error"]; // get the content for this returned data set

        Debug.Log("Got version check response from server");
        // Print the error to the console
        if (errorObj.str != "" && errorObj != null){
			Debug.Log("request error: " + serverResponse.error);
		}else{
            Debug.Log("returned data: " + serverResponse.text.Replace("\n", "========="));
			
            if(statusObj.GetField("is_latest_version").b == true)
            {
                Debug.Log("Using the latest version version of the scenario");
            }
            else
            {
                if (contentObj.HasField("download_url"))
                {

                    HandleThisVersionIsNotCurrent((float)contentObj.GetField("current_scenario_version").n,
                                                    (float)contentObj.GetField("latest_scenario_version").n,
                                                    contentObj.GetField("download_url").str,
                                                    contentObj.GetField("message").str);
                }
                else
                {
                    HandleThisVersionIsNotCurrent((float)contentObj.GetField("current_scenario_version").n,
                                                    (float)contentObj.GetField("latest_scenario_version").n,
                                                    "https://resources.smallablearning.com/tech", // TODO: replace with message from the server
                                                    "A newer version of this scenario is available"); // TODO: replace with message from the server
                                                                                                      //contentObj.GetField("download_url").str,
                                                                                                      //contentObj.GetField("message").str);
                }
            }
			
            /*
			string jsonResponse = serverResponse.text;
			
			//http://www.unifycommunity.com/wiki/index.php?title=JSONObject
			JSONObject jsonObjectFromServer = new JSONObject(jsonResponse);
			//accessData(j);
			
			JSONObject stringObject = jsonObjectFromServer["is_version_current"];

            //Debug.Log("Is Version Current? ... " + stringObject.str);

            if (stringObject != null && stringObject.str == "No")
            { // if this version isn't current, notify the user
                HandleThisVersionIsNotCurrent();
            }
            */
			
		}
	}
	
	
	//access data (and print it)
	void accessData(JSONObject obj){
    switch(obj.type){
        case JSONObject.Type.OBJECT:
            for(int i = 0; i < obj.list.Count; i++){
                string key = (string)obj.keys[i];
                JSONObject j = (JSONObject)obj.list[i];
                Debug.Log(key);
                accessData(j);
            }
            break;
        case JSONObject.Type.ARRAY:
            foreach(JSONObject j in obj.list){
                accessData(j);
            }
            break;
        case JSONObject.Type.STRING:
            Debug.Log(obj.str);
            break;
        case JSONObject.Type.NUMBER:
            Debug.Log(obj.n);
            break;
        case JSONObject.Type.BOOL:
            Debug.Log(obj.b);
            break;
        case JSONObject.Type.NULL:
            Debug.Log("NULL");
            break;
		}
    }
	
	void OnEnable(){
		
		CheckForNewerVersion();
		
	}
	
	void HandleThisVersionIsNotCurrent(float currentVersion, float latestVersion, string downloadUrl, string message){
		displayThisVersionNotCurrent = true;

        currentScenarioDownloadUrl = downloadUrl;

        if (currentScenarioDownloadUrl != "")
        {
            downloadButton.SetActive(true);
        }
        else
        {
            downloadButton.SetActive(false);
        }

        UpdatePanelText("Newer Version Available", message, "Remind Me Later");
        ShowScenarioVersionCheckMessagePanel(true);

    }

    private void ShowScenarioVersionCheckMessagePanel(bool show)
    {
        //Debug.Log("Show Scenario Version Check Window: " + show);

        GetComponent<CanvasGroup>().interactable = show;
        GetComponent<CanvasGroup>().alpha = show ? 1.0f : 0.0f;
        GetComponent<CanvasGroup>().blocksRaycasts = show;
    }

    private void UpdatePanelText(string header, string message, string button)
    {
        errorHeader.text = header;
        errorMessage.text = message;

        errorButtonText.text = button;
    }

    public void OkButtonPressed()
    {

        //Debug.Log("Scenario Version Check: OK Button Pressed");
        ShowScenarioVersionCheckMessagePanel(false);
        
    }
    public void DownloadButtonPressed()
    {
        //Debug.Log("Scenario Version Check: DOWNLOAD Button Pressed");
        //Debug.Log("Will open: " + currentScenarioDownloadUrl);
        ShowScenarioVersionCheckMessagePanel(false);
        Application.OpenURL(currentScenarioDownloadUrl);

    }

    /*
    void OnGUI() {
		
		GUI.depth = -8;
		if(displayThisVersionNotCurrent){
			GUI.BeginGroup(new Rect((Screen.width/2) - 200, (Screen.height/2) - 125, 400, 250));
			GUI.Box (new Rect(0, 0, 400, 250), "Version Checker");
			
			GUI.Label(new Rect(40, 40, 360, 180), "A newer version of this scenario is available for download");
        	
			if (GUI.Button(new Rect(40, 200, 150, 30), "Visit Download Site")){
				Application.OpenURL("http://resources.smallablearning.com/tech");
			}
			if (GUI.Button(new Rect(210, 200, 150, 30), "Remind Me Later")){
        	    //print("Got a click");
				displayThisVersionNotCurrent = false;
			}

			
			
			
			GUI.EndGroup();
		}
    }

	
	
    void DoMyWindow(int windowID) {
		
        
    }
	*/
}
