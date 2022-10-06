using UnityEngine;
using System.Collections;
using System.Xml;
using System.Text;

public class InputSourceSwitchScript : MonoBehaviour {

    /** VERSION 1.1 **/

	private bool isUsingOptitrackInputSource = true;

	private GameObject currentOptitrackInputSource;
	public GameObject optitrackPrefabInputSource, motiveServerPrefabInputSource, mouseInputSource;

    public bool shouldUseSMALLabMotiveServer = true;

    private void Awake()
    {
        shouldUseSMALLabMotiveServer = ReadShouldUseSMALLabMotiveServerFromXMLPreferences();
    }


    // Use this for initialization
    void Start () {

        if (shouldUseSMALLabMotiveServer)
        {
            currentOptitrackInputSource = (GameObject)Instantiate(motiveServerPrefabInputSource, Vector3.zero, Quaternion.identity);
        }
        else
        {
            currentOptitrackInputSource = (GameObject)Instantiate(optitrackPrefabInputSource, Vector3.zero, Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnGUI(){
		Event e = Event.current;

		if (e.type == EventType.KeyDown && e.control && e.shift && e.keyCode == KeyCode.M){ // cntl - l = play
			HandleSwitchInputSource();
		}

	}

	private void HandleSwitchInputSource(){

		if(!isUsingOptitrackInputSource){
			isUsingOptitrackInputSource = true;

			if(currentOptitrackInputSource == null){
                if (shouldUseSMALLabMotiveServer)
                {
                    currentOptitrackInputSource = (GameObject)Instantiate(motiveServerPrefabInputSource, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    currentOptitrackInputSource = (GameObject)Instantiate(optitrackPrefabInputSource, Vector3.zero, Quaternion.identity);
                }
			}


			mouseInputSource.SetActive(false);

		}else{

			isUsingOptitrackInputSource = false;

			if(currentOptitrackInputSource != null){
                if (shouldUseSMALLabMotiveServer)
                {
                    currentOptitrackInputSource.GetComponent<MulticastReceive_SMALLabMotiveServer>().CloseUDPConnection();
                }
                else
                {
                    currentOptitrackInputSource.GetComponent<MulticastReceive>().CloseUDPConnection();
                }
		    }
			Destroy (currentOptitrackInputSource);

			mouseInputSource.SetActive(true);

		}

		Debug.Log("received command to switch the input source");
	}


    private bool ReadShouldUseSMALLabMotiveServerFromXMLPreferences()
    {

        bool useSMALLabMotiveServer = false;

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
            XmlNode smallabMotiveServerNode = doc.SelectSingleNode("/smallablearning/smallab/naturalpoint/smallabmotiveserver");

            if (smallabMotiveServerNode != null)
            {
                if(smallabMotiveServerNode.InnerText == "true")
                {
                    useSMALLabMotiveServer = true;
                }
                else
                { // if the preference is invalid for whatever reason, set the default to false
                    useSMALLabMotiveServer = false;
                }
            }
            else
            { // if the preference hasn't been set, default to NatNet 2.0
                useSMALLabMotiveServer = false;
            }

        }
        catch (System.Exception e)
        {

            Debug.Log(e.ToString());

        }

        Debug.Log("Use SMALLab Motive Server: " + useSMALLabMotiveServer);

        return useSMALLabMotiveServer;

    }
}
