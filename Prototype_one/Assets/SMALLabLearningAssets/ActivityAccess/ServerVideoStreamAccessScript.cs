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

public class ServerVideoStreamAccessScript : MonoBehaviour {

    [Header("ScriptVersion 3.1")]
    [Space(10)]

    bool isPreferencesFilePresent = false;
    string preferencesFilePath = "";


    [System.NonSerialized]
    public string API_KEY = "5F5048A8-18AF-11E1-9548-B8614824019B";


    string pathToPrefFile = "";
    private WWW serverRequest;


    ConfigFilePanelScript configFilePanelScript;
    private ScenarioAnalyticsScript scenarioAnalyticsScript;

    void Awake()
    {

        scenarioAnalyticsScript = GameObject.Find("ScenarioAnalytics").GetComponent<ScenarioAnalyticsScript>();

        configFilePanelScript = GameObject.Find("Canvas/ConfigurationDisplay").GetComponent<ConfigFilePanelScript>();

    }

    // Use this for initialization
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {

    }


    


    private void HandleNoPreferencesFile()
    {
        Debug.Log("No preferences file found!");

        //UpdatePanelText("Error", "Unable to locate your preferences file. Please run the SMALLab Calibration scenario and try again.", "OK");
        //handleServerResponseFromSerialNumberCheck("", true, false, true, "Unable to locate your preferences file. Please check your system configuration and try again.", false);

        //ShowLicenseCheckMessagePanel(true);
    }

    void handleServerResponseFromGetVideoStreamData(bool success, string errorMessage, bool connectedToLicenseServer, JSONObject data)
    {


        Debug.Log("Logging Success: " + success + ", Error: " + errorMessage + ", Server Connected: " + connectedToLicenseServer);

        configFilePanelScript.HandleVideoStreamDataFromServer(success, errorMessage, connectedToLicenseServer, data);
    }




    public void GetVideoStreamDataFromServer(int videoStreamId)
    {
        //string activityFileName = Path.GetFileNameWithoutExtension(activityFilePath);

        Debug.Log("Getting videostream data for = " + videoStreamId);

        //Note: your data can only be numbers and strings.  This is not a solution for object serialization or anything like that.
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("API_key", scenarioAnalyticsScript.API_KEY); // get the api key from the analytics script                                              //string
        j.AddField("serialnumber", scenarioAnalyticsScript.serialNumber);
        j.AddField("scenario_id", scenarioAnalyticsScript.scenarioID);
        j.AddField("user_id", scenarioAnalyticsScript.currentUserId);
        // always pass the device udid		
        j.AddField("device_udid", GenerateDeviceUDID());
        j.AddField("videostream_id", videoStreamId);
        string jsonEncodedString = j.print();
        //Debug.Log("log activity: " + jsonEncodedString);


        StartCoroutine(CheckLicenseServerConnection(
            (isConnected) => { // first, make sure that the machine can connect to the license server
                if (isConnected)
                {
                    Debug.Log("We're connected and will make request to log activity file loaded");
                    StartCoroutine(MakeRequestToGetVideoStreamData(jsonEncodedString)); // if it can proceed
                }
                else
                {
                    handleServerResponseFromGetVideoStreamData(false, "Unable to connect to server", false, null);
                }
            }
        ));

    }

    public IEnumerator MakeRequestToGetVideoStreamData(string jsonEncodedString)
    {

        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");

        this.serverRequest = new WWW(scenarioAnalyticsScript.serverBaseUrl + "/api/v1/activities/get_videostream_data?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);

        yield return serverRequest;

        Debug.Log("Text: " + serverRequest.text);
        //print("returned data: " + serverRequest.data.Replace("\n", "========="));

        JSONObject j = new JSONObject(serverRequest.text);
        JSONObject contentObj = j["content"]; // get the content for this returned data set
        JSONObject statusObj = j["status"];
        JSONObject errorObj = j["error"]; // get the content for this returned data set

        handleServerResponseFromGetVideoStreamData(statusObj.GetField("success").b, errorObj.str, true, contentObj);

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



    private string readSerialNumberFromXMLFile()
    {
        Debug.Log("Reading serial number");
        string serialNumber = "";

        try
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(preferencesFilePath);

            XmlNodeList xnList = doc.SelectNodes("smallablearning/license");
            foreach (XmlNode xn in xnList)
            {
                serialNumber = xn["serialnumber"].InnerText;
            }

        }
        catch (System.Exception e)
        {

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
