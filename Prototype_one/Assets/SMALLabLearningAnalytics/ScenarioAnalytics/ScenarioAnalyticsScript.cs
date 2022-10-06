/*

Copyright © 2012.  SMALLab Learning, LLC.  All Rights Reserved
 
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class ScenarioAnalyticsScript : MonoBehaviour {

    [Header("ScriptVersion 3.3")]
    [Space(10)]
    public int scenarioID = 1;

    [System.NonSerialized]
	public string API_KEY = "5F5048A8-18AF-11E1-9548-B8614824019B";
	
	[System.NonSerialized]
	public int LAUNCHED_SCENARIO = 1; // these codes should match the id's in the database table 'event'
	[System.NonSerialized]
	public int EXITED_SCENARIO = 2;
    [System.NonSerialized]
    public int PING_ALIVE_SCENARIO = 3;
    
    public int currentUserId = -1;

    private WWW serverRequest;

    //[System.NonSerialized]
    public string serverBaseUrl = "https://resources.smallablearning.com";
    //public string serverBaseUrl = "https://resources.com";

    [System.NonSerialized]
	public string serialNumber = "";
	
	public bool sendAnalyticsData = true;

    [System.NonSerialized]
    public string sessionId; 
	
	//InfoDisplay infoDisplayScript;

	private float versionNumber;

    private bool continueSendingPingData = false;
    private float secondsToWaitBeforeSendingPingData = 60.0f;


    void Awake(){

        serialNumber = readSerialNumberFromXMLFile();

        sessionId = GenerateSessionId();

		//infoDisplayScript = GameObject.Find("InfoDisplay").GetComponent<InfoDisplay>();
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
	
	void LogActivity(int eventCode, bool isExitingApplication){
		
		//Note: your data can only be numbers and strings.  This is not a solution for object serialization or anything like that.
		JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
		//number
		j.AddField("API_key", API_KEY);
		//string
		j.AddField("serialnumber", serialNumber);
		j.AddField("scenario_id", scenarioID);
        j.AddField("session_id", sessionId);
        j.AddField("user_id", currentUserId);
        j.AddField("version_number", versionNumber);
		j.AddField("event_id", eventCode);
		//array
		/*
		JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
		j.AddField("field3", arr);

		arr.Add(1);
		arr.Add(2);
		arr.Add(3);
		 */
		string jsonEncodedString = j.print();
		//Debug.Log(jsonEncodedString);
		
		if(sendAnalyticsData){ // allow a developer to turn on and off sending the analytics during development
		
			if(!isExitingApplication){ // if we're not exiting, use the coroutine so we don't slow things down
				StartCoroutine(MakeCallToServer(jsonEncodedString));
			}else{ // otherwise use the blocking call without waiting for a response
				MakeExitingCallToServer(jsonEncodedString);	
			}
		}
		
	
	}
	
	void MakeExitingCallToServer(string jsonEncodedString){
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
        //postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());

        this.serverRequest = new WWW(serverBaseUrl + "/api/v1/analytics/log_scenario_event?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);

        //serverRequest = new WWW("http://analytics.smallablearning.com/log_scenario_event.php?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);
		//HandleServerResponse(serverRequest);
	}
	
	public IEnumerator MakeCallToServer(string jsonEncodedString){
		
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
        //postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());
        
        this.serverRequest = new WWW(serverBaseUrl + "/api/v1/analytics/log_scenario_event?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);

        Debug.Log("Sending this request: " + jsonEncodedString  + " to: " + this.serverRequest.url);

        //serverRequest = new WWW("http://analytics.smallablearning.com/log_scenario_event.php?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);

        yield return serverRequest;
        //Debug.Log("Text: " + serverRequest.text);
        HandleServerResponse(serverRequest);
	}
	
	void HandleServerResponse(WWW serverResponse){
		
		
		// Print the error to the console
		if (serverResponse.error != null){
			Debug.Log("request error: " + serverResponse.error);
		}else{
			Debug.Log("request success");
			print("returned data: " + serverRequest.text.Replace("\n", "========="));
		}
		
		/*
		//foreach (KeyValuePair pair in serverResponse.responseHeaders){
		Dictionary<string, string> result = serverResponse.responseHeaders;
		//Dictionary myHash = serverResponse.responseHeaders;
		foreach (KeyValuePair<string, string> pair in result){
		//for (key in myHash.Keys) {
		//for(key in serverResponse.responseHeaders){
			Debug.Log ("key:"+pair.Key+", value: "+ pair.Value);
    	}
		
		Debug.Log("serverResponse = " + serverResponse.data);
		*/
	}

    IEnumerator PeriodicallyPingAliveData()
    {
        while (continueSendingPingData)
        {
            yield return new WaitForSeconds(secondsToWaitBeforeSendingPingData);

            if (sendAnalyticsData)
            {
                LogActivity(PING_ALIVE_SCENARIO, false);
            }

        }
    }


    void OnEnable(){
		
		
		
	}
	
	void OnDisable(){
        continueSendingPingData = false;

        LogActivity(EXITED_SCENARIO, true);
	}

    public void StartLoggingWithUserId(int userId)
    {
        currentUserId = userId;
        LogActivity(LAUNCHED_SCENARIO, false);

        // start the alive ping to run every minute
        continueSendingPingData = true;
        StartCoroutine(PeriodicallyPingAliveData());
    }

        


    public string readSerialNumberFromXMLFile(){
		
		string serialNumber = "";
			
		try{
			XmlDocument doc = new XmlDocument();

			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor){
				doc.Load (System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) 
			          + "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml");
			}else{
				doc.Load (System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) 
			          + "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml");
			}
			
			XmlNodeList xnList = doc.SelectNodes("smallablearning/license");
			foreach (XmlNode xn in xnList){
 				serialNumber = xn["serialnumber"].InnerText;	
			} 
						
		}catch(System.Exception e){
		
			Debug.Log(e.ToString());
			
		}
            
		return serialNumber;
	}


    private string GenerateSessionId()
    {
        return System.Guid.NewGuid().ToString();
    }
}
