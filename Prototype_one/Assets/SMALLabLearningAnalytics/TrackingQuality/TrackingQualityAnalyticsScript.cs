using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

public class TrackingQualityAnalyticsScript : MonoBehaviour {


    [Header("ScriptVersion 3.1")]
    [Space(10)]

    public float [] trackedObjectAverageMeanErrorArray = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    //public int [] trackedObjectAverageMeanErrorFrameCountArray = {0, 0, 0, 0, 0, 0 };
    public int[] trackedObjectDroppedFrameCountArray = { 0, 0, 0, 0, 0, 0 };

    //private int numberOfFramesToComputeRollingErrorAverage = 180;

    private int numFramesThisCycle = 0;

    private WWW serverRequest;

    

    private ScenarioAnalyticsScript scenarioAnalyticsScript;

    private float secondsToWaitBeforeSendingData = 30.0f;
    private bool continueSendingData = false;

    private float NatNetVersion = -1.0f;

    void Awake()
    {
        scenarioAnalyticsScript = GameObject.Find("ScenarioAnalytics").GetComponent<ScenarioAnalyticsScript>();
        
        // Get the NatNet Version from preferences so we can properly parse the incoming stream from Tracking Tools/Motive
        NatNetVersion = ReadNatNetVersionFromXMLPreferences();
    }

    // Use this for initialization
    void Start () {

        continueSendingData = true;
        StartCoroutine(PeriodicallyLogTrackingData());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void RegisterDroppedFrame(int trackableId)
    {
        trackedObjectDroppedFrameCountArray[trackableId - 1] += 1;
    }


    private void AddNewErrorToRollingAverage(int trackableId, float newMeanMarkerError)
    {

        // new average = old average + (next data - old average) / next count
        //trackedObjectAverageMeanErrorArray[trackableId - 1] = trackedObjectAverageMeanErrorArray[trackableId - 1] + (newMeanMarkerError - trackedObjectAverageMeanErrorArray[trackableId - 1]) / (trackedObjectAverageMeanErrorFrameCountArray[trackableId - 1] + 1);
        trackedObjectAverageMeanErrorArray[trackableId - 1] = trackedObjectAverageMeanErrorArray[trackableId - 1] + (newMeanMarkerError - trackedObjectAverageMeanErrorArray[trackableId - 1]) / numFramesThisCycle;
        //trackedObjectAverageMeanErrorFrameCountArray[trackableId - 1] += 1;
    }

    void handleTrackedObjectData(TrackedObject[] trackedObjectArray)
    {

        numFramesThisCycle++;

        foreach (TrackedObject trackedObject in trackedObjectArray)
        { // go through each of the tracked objects
            if (trackedObject.meanMarkerError != 0.0f) // don't register this if it's not an actual error number
            {
                //Debug.Log("Error for object id: " + trackedObject.id + " = " + trackedObject.meanMarkerError);

                AddNewErrorToRollingAverage(trackedObject.id, trackedObject.meanMarkerError * 1000.0f); // multiply by 1000 because this is the mean error in mm
            }
            else
            {
                RegisterDroppedFrame(trackedObject.id);
            }
            //trackedObject.maen
           

        }

    }

    private void ResetValuesForNewWindow()
    {
        numFramesThisCycle = 0;

        for (int x = 0; x < trackedObjectAverageMeanErrorArray.Length; x++)
        {
            trackedObjectAverageMeanErrorArray[x] = 0.0f;
            //trackedObjectAverageMeanErrorFrameCountArray[x] = 0;
            trackedObjectDroppedFrameCountArray[x] = 0;
        }
    }

    private float ComputeAverageErrorForAllActiveWands()
    {
        int wandCount = 0;
        float totalError = 0.0f;

        foreach (float error in trackedObjectAverageMeanErrorArray)
        {
            if (error != 0.0f)
            {
                totalError += error;
                wandCount++;
            }

        }

        if (wandCount > 0)
        {
            //Debug.Log("Total Error before division = " + totalError);
            totalError = totalError / (float)wandCount;
        }
        else
        {
            totalError = -1.0f;
        }

        //Debug.Log("Num wands included in the average: " + wandCount);
        return totalError;
        
    }

    IEnumerator PeriodicallyLogTrackingData()
    {
        while (continueSendingData)
        {
            ResetValuesForNewWindow();
            yield return new WaitForSeconds(secondsToWaitBeforeSendingData);

            /*
            Debug.Log("Will send data: " + trackedObjectAverageMeanErrorArray[0] +
                                       ", " + trackedObjectAverageMeanErrorArray[1] +
                                       ", " + trackedObjectAverageMeanErrorArray[2] +
                                       ", Average: " + ComputeAverageErrorForAllActiveWands());
            */
            LogMotiveMeanErrorData();

            
        }
    }

    private float ReadNatNetVersionFromXMLPreferences()
    {

        float version = 0.0f;

        try
        {
            XmlDocument doc = new XmlDocument();
            //doc.Load(Application.dataPath + "/../../" + "SMALLabLearningPreferences_v1.0.xml");

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                doc.Load(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData)
                      + "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml");
            }
            else
            {
                doc.Load(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)
                          + "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml");
            }

            // get the xml node that should contain the version
            XmlNode natNetVersionNode = doc.SelectSingleNode("/smallablearning/smallab/naturalpoint/natnetversion");

            if (natNetVersionNode != null)
            {
                float newValue;
                if (float.TryParse(natNetVersionNode.InnerText, out newValue))
                {
                    version = newValue;
                }
                else
                { // if the preference is invalid for whatever reason, set the default to NatNet 2.0
                    version = 2.0f;
                }
            }
            else
            { // if the preference hasn't been set, default to NatNet 2.0
                version = 2.0f;
            }

        }
        catch (System.Exception e)
        {

            Debug.Log(e.ToString());

        }

        Debug.Log("Retrieved Nat Net Version: " + version);

        return version;

    }


    /**********************************************************/
    /*************** ANALYTICS LOGGING ************************/
    /**********************************************************/

    public void LogMotiveMeanErrorData()
    {
        //string activityFileName = Path.GetFileNameWithoutExtension(activityFilePath);

        //Debug.Log("Logging new activity file loaded Current serial number = " + scenarioAnalyticsScript.serialNumber + ", loaded activity file: " + activityFileName);

        //Note: your data can only be numbers and strings.  This is not a solution for object serialization or anything like that.
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("API_key", scenarioAnalyticsScript.API_KEY); // get the api key from the analytics script                                              //string
        j.AddField("serialnumber", scenarioAnalyticsScript.serialNumber);
        j.AddField("scenario_id", scenarioAnalyticsScript.scenarioID);
        j.AddField("session_id", scenarioAnalyticsScript.sessionId);
        j.AddField("user_id", -1);
        // always pass the device udid		
        j.AddField("device_udid", GenerateDeviceUDID());

        j.AddField("mean_error_all_active_wands", ComputeAverageErrorForAllActiveWands());
        j.AddField("mean_error_wand_01", trackedObjectAverageMeanErrorArray[0]);
        j.AddField("mean_error_wand_02", trackedObjectAverageMeanErrorArray[1]);
        j.AddField("mean_error_wand_03", trackedObjectAverageMeanErrorArray[2]);
        j.AddField("mean_error_wand_04", trackedObjectAverageMeanErrorArray[3]);
        j.AddField("mean_error_wand_05", trackedObjectAverageMeanErrorArray[4]);
        j.AddField("mean_error_wand_06", trackedObjectAverageMeanErrorArray[5]);

        j.AddField("percent_dropped_frames_wand_01", (float)trackedObjectDroppedFrameCountArray[0]/ (float)numFramesThisCycle);
        j.AddField("percent_dropped_frames_wand_02", (float)trackedObjectDroppedFrameCountArray[1] / (float)numFramesThisCycle);
        j.AddField("percent_dropped_frames_wand_03", (float)trackedObjectDroppedFrameCountArray[2] / (float)numFramesThisCycle);
        j.AddField("percent_dropped_frames_wand_04", (float)trackedObjectDroppedFrameCountArray[3] / (float)numFramesThisCycle);
        j.AddField("percent_dropped_frames_wand_05", (float)trackedObjectDroppedFrameCountArray[4] / (float)numFramesThisCycle);
        j.AddField("percent_dropped_frames_wand_06", (float)trackedObjectDroppedFrameCountArray[5] / (float)numFramesThisCycle);
        j.AddField("natnet_version", NatNetVersion);


        //j.AddField("activity_file_name", activityFileName);
        string jsonEncodedString = j.print();
        //Debug.Log("log activity: " + jsonEncodedString);


        StartCoroutine(CheckLicenseServerConnection(
            (isConnected) => { // first, make sure that the machine can connect to the license server
                if (isConnected)
                {
                    Debug.Log("We're connected and will make request to log activity file loaded");
                    StartCoroutine(MakeRequestToLogTrackingData(jsonEncodedString)); // if it can proceed
                }
                else
                {
                    handleServerResponseFromLogTrackingData(false, "Unable to connect to server", false);
                }
            }
        ));

    }

    public IEnumerator MakeRequestToLogTrackingData(string jsonEncodedString)
    {

        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");
        //postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());



        this.serverRequest = new WWW(scenarioAnalyticsScript.serverBaseUrl + "/api/v1/analytics/log_motive_mean_error?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);

        yield return serverRequest;

        Debug.Log("Text: " + serverRequest.text);
        //print("returned data: " + serverRequest.data.Replace("\n", "========="));

        JSONObject j = new JSONObject(serverRequest.text);
        JSONObject contentObj = j["content"]; // get the content for this returned data set
        JSONObject statusObj = j["status"];
        JSONObject errorObj = j["error"]; // get the content for this returned data set


        handleServerResponseFromLogTrackingData(statusObj.GetField("success").b, errorObj.str, true);

    }

    void handleServerResponseFromLogTrackingData(bool success, string errorMessage, bool connectedToLicenseServer)
    {
        Debug.Log("Logging Success: " + success + ", Error: " + errorMessage + ", Server Connected: " + connectedToLicenseServer);
    }





    IEnumerator CheckLicenseServerConnection(Action<bool> action)
    {
        WWW www = new WWW(scenarioAnalyticsScript.serverBaseUrl);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Got this error with the connection to " + scenarioAnalyticsScript.serverBaseUrl + " - " + www.error);
            action(false);
        }
        else
        {
            action(true);
        }
    }


    

    private string GenerateDeviceUDID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}
