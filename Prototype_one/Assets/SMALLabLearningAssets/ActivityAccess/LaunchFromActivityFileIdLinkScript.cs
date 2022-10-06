using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LaunchFromActivityFileIdLinkScript : MonoBehaviour {

    [Header("ScriptVersion 1.1")]
    [Space(10)]
    public string urlScheme = "";

    Text debugDisplay;


    string previousLaunchLinkScheme = "";
    string launchLinkUrlScheme = "";

    ScenarioAnalyticsScript scenarioAnalyticsScript;

    private void Awake()
    {
        //debugDisplay = GameObject.Find("Canvas/ConfigurationDisplay/ConfigurationPanel/DebugLaunch").GetComponent<Text>();
        ProcessCommandLineArgs();

        scenarioAnalyticsScript = GameObject.Find("!StandardComponents/ScenarioAnalytics").GetComponent<ScenarioAnalyticsScript>();
    }

    private void Start()
    {

        // if we have a valid id from the launch update our internal user id
        scenarioAnalyticsScript.StartLoggingWithUserId(GetUserIdFromLaunchParameters());

    }

    void ProcessCommandLineArgs()
    {
        string[] args = Environment.GetCommandLineArgs();

        if (args.Length > 1)
        {
            launchLinkUrlScheme = args[1];
            //debugDisplay.text = launchLinkUrlScheme;
        }
        else
        {
            urlScheme += "\nNo args from launch\n";
        }

    }

    public int GetUserIdFromLaunchParameters()
    {
        
        //debugDisplay.text = debugDisplay.text + "\nuser_id: " + GetValueFromLaunchParameters("user_id");
        return GetValueFromLaunchParameters("user_id");
    }

    public int GetActivityIdFromLaunchParameters()
    {
        //debugDisplay.text = debugDisplay.text + "\nactivity_id: " + GetValueFromLaunchParameters("activity_id");
        return GetValueFromLaunchParameters("activity_id");
    }

    public int GetActivityFileIdFromLaunchParameters()
    {
        //return 1847;
        //return 1849;

        //debugDisplay.text = debugDisplay.text + "\nactivity_file_id: " + GetValueFromLaunchParameters("activity_file_id");
        return GetValueFromLaunchParameters("activityfile_id");
    }

    public int GetVideoStreamIdFromLaunchParameters()
    {
        //debugDisplay.text = debugDisplay.text + "\nvideostream_id: 14";
        //debugDisplay.text = debugDisplay.text + "\nvideostream_id: " + GetValueFromLaunchParameters("videostream_id");
        return GetValueFromLaunchParameters("videostream_id");
    }

    private int GetValueFromLaunchParameters(string key)
    {
        //Debug.Log("launchLinkUrlScheme = " + launchLinkUrlScheme);
        if (launchLinkUrlScheme == "" || launchLinkUrlScheme == "-projectpath")
        {
            return -1;
        }
        else
        {
            string[] keyValueArray = launchLinkUrlScheme.Split('?')[1].Split('&');
            foreach (string keyValuePair in keyValueArray)
            {
                string[] keyValuePairArray = keyValuePair.Split('=');
                if (keyValuePairArray[0] == key)
                {
                    int value = -1;
                    if (Int32.TryParse(keyValuePairArray[1], out value))
                    {
                        return value;
                    }
                    else
                    {
                        return -1;
                    }

                }
            }
        }

        return -1;
    }

    public int LaunchLinkVideoId()
    {
        return -1;

        string commandline = Environment.CommandLine;
        launchLinkUrlScheme += "Commandline: " + commandline + "\n";

        string[] args = Environment.GetCommandLineArgs();

        launchLinkUrlScheme += "Number of args = " + args.Length + "\n";

        foreach (string arg in args)
        {
            launchLinkUrlScheme += arg + "\n";
        }

    }
}
