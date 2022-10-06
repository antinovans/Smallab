using UnityEngine;
using System.Collections;
using System.Xml;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;

public class SerialNumberValidation : MonoBehaviour {
	
	[System.NonSerialized]
	public string API_KEY = "5F5048A8-18AF-11E1-9548-B8614824019B";
	
	public string mySerialNumber = "";
	
	string pathToPrefFile = "";
	
	private string retrievedCustomerName;
	private string retrievedCustomerComputer;
	private string retrievedProductName;
	
	ScenarioAnalyticsScript analyticsScript;
	private WWW serverRequest;
	
	public delegate void SerialValidationResponseEventHandler(string serialNumber, bool isValid, string errorMessage, bool connectedToLicenseServer);
	public event SerialValidationResponseEventHandler serialValidationResponseEventHandler;

	private string serverBaseUrl = "http://localhost:8888";

	// Use this for initialization
	void Start () {
		analyticsScript = GameObject.Find("ScenarioAnalytics").GetComponent<ScenarioAnalyticsScript>();
		
		//Debug.Log ("Device id: " + SystemInfo.deviceUniqueIdentifier);		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		//showSerialNumberStep();
		
	}
	
	
	
	public void SetNewSerialNumber(string number){
		mySerialNumber = number;	
	}
	
	
	
	public void CheckIfSerialNumberIsValid(){
		
		Debug.Log("Current serial number = " + mySerialNumber);
		
		if(mySerialNumber.Length == 0){
			serialValidationResponseEventHandler(mySerialNumber, false, "Unable to locate license key. Please run the SMALLab Calibration Scenario to license your system.", false);
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
					serialValidationResponseEventHandler(mySerialNumber, false, "Unable to connect to the license server. Please check your internet connection and try again", false);
				}
			}
			));
		}

	}

	private string GenerateDeviceUDID(){
		return SystemInfo.deviceUniqueIdentifier;
	}

	/*
	public void GetCustomerInfoForSerialNumber(string customerSerialNumber){
		
		//Note: your data can only be numbers and strings.  This is not a solution for object serialization or anything like that.
		JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
		//number
		j.AddField("API_key", API_KEY);
		//string
		j.AddField("serialnumber", customerSerialNumber);
		
		// always pass the device udid		
		j.AddField("device_udid", GenerateDeviceUDID());
		
		
		
		string jsonEncodedString = j.print();
		//Debug.Log("Retrieve Customer JSON: " + jsonEncodedString);
		
		mySerialNumber = customerSerialNumber;
		
		StartCoroutine(CheckLicenseServerConnection(
		(isConnected)=>{ // first, make sure that the machine can connect to the license server
			if(isConnected){
				StartCoroutine(MakeRequestForCustomerInfo(jsonEncodedString)); // if it can proceed
			}else{
				serialValidationResponseEventHandler(customerSerialNumber, false, "Unable to connect to the license server. Please check your internet connection and try again", false);
			}
		}
		
		));
		//yield return MakeRequestForCustomerInfo(jsonEncodedString);
		
		//return serialNumber;
	}
	*/

	/*
	public IEnumerator MakeRequestForCustomerInfo(string jsonEncodedString){
				
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
		postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());

		this.serverRequest = new WWW(serverBaseUrl + "/api/v1/license/check?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);
		
		yield return serverRequest;
		//yield serverRequest;
		
		//Debug.Log("Text: " + serverRequest.text);
		//print("returned data: " + serverRequest.data.Replace("\n", "========="));
		
		JSONObject j = new JSONObject(serverRequest.text);
		JSONObject contentObj = j["content"]; // get the content for this returned data set
		JSONObject statusObj = j["status"]; 
		JSONObject errorObj = j["error"]; // get the content for this returned data set


		//if(errorObj.str.Equals("")){
		if(statusObj.GetField("is_valid").b == true){

			retrievedCustomerName = contentObj.GetField("customer_name").ToString();
			retrievedCustomerComputer = contentObj.GetField("system_name").ToString();
			retrievedProductName = contentObj.GetField("product_name").ToString();
			//Debug.Log("Customer Name = " + retrievedCustomerName);
			//Debug.Log("Customer Computer = " + retrievedCustomerComputer);
			
			// this calls to the delegate
			if(serialValidationResponseEventHandler != null)
				serialValidationResponseEventHandler(mySerialNumber, retrievedProductName, retrievedCustomerName, retrievedCustomerComputer, errorObj.str, true);
			
		}else{
			retrievedCustomerName = "";
			retrievedCustomerComputer = "";
			retrievedProductName = "";
			Debug.Log("Error with serial number");
			Debug.Log("Error = " + errorObj.str);
			
			// this calls to the delegate
			if(serialValidationResponseEventHandler != null)
				serialValidationResponseEventHandler(mySerialNumber, retrievedProductName, retrievedCustomerName, retrievedCustomerComputer, errorObj.str, true);
			
		}
		
		//Debug.Log("Got customer name = " + o
		
		//Debug.log(arr[2].n);
		//accessData(j);
		//returned data: {"content":{"id":"6","customer_id":"3","product_id":"2","serialnumber":"be9d34af43b578756cc08d840a79fc58","name":"Testing Serial Number"},"type":"serialnumber_customer_info","error":""}
		//retrievedCustomerName = 
		//retrievedCustomerComputer=
		
		//return serverRequest;	
	}
	*/
	/*
	public IEnumerator MakeRequestForCustomerInfo(string jsonEncodedString){
				
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
		postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());
		
		this.serverRequest = new WWW("http://analytics.smallablearning.com/ci/index.php/customer_json/get_serial_number_info_with_udid?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);
		
		yield return serverRequest;
		//yield serverRequest;
		
		//Debug.Log("Text: " + serverRequest.text);
		//print("returned data: " + serverRequest.data.Replace("\n", "========="));
		
		JSONObject j = new JSONObject(serverRequest.text);
		JSONObject contentObj = j["content"]; // get the content for this returned data set
		
		JSONObject errorObj = j["error"]; // get the content for this returned data set
		
		if(errorObj.str.Equals("")){
			retrievedCustomerName = contentObj.GetField("customer_name").ToString();
			retrievedCustomerComputer = contentObj.GetField("system_name").ToString();
			retrievedProductName = contentObj.GetField("product_name").ToString();
			//Debug.Log("Customer Name = " + retrievedCustomerName);
			//Debug.Log("Customer Computer = " + retrievedCustomerComputer);
			
			// this calls to the delegate
			if(serialValidationResponseEventHandler != null)
				serialValidationResponseEventHandler(mySerialNumber, retrievedProductName, retrievedCustomerName, retrievedCustomerComputer, errorObj.str, true);
			
		}else{
			retrievedCustomerName = "";
			retrievedCustomerComputer = "";
			retrievedProductName = "";
			Debug.Log("Error with serial number");
			Debug.Log("Error = " + errorObj.str);
			
			// this calls to the delegate
			if(serialValidationResponseEventHandler != null)
				serialValidationResponseEventHandler(mySerialNumber, retrievedProductName, retrievedCustomerName, retrievedCustomerComputer, errorObj.str, true);
			
		}
		
		//Debug.Log("Got customer name = " + o
		
		//Debug.log(arr[2].n);
		//accessData(j);
		//returned data: {"content":{"id":"6","customer_id":"3","product_id":"2","serialnumber":"be9d34af43b578756cc08d840a79fc58","name":"Testing Serial Number"},"type":"serialnumber_customer_info","error":""}
		//retrievedCustomerName = 
		//retrievedCustomerComputer=
		
		//return serverRequest;	
	}
	*/

	/*
	public IEnumerator MakeRequestIfSerialNumberIsValid(string jsonEncodedString){
		
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
		postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());
		
		this.serverRequest = new WWW("http://analytics.smallablearning.com/ci/index.php/customer_json/is_serial_number_valid?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);
		
		yield return serverRequest;
		
		Debug.Log("Text: " + serverRequest.text);
		//print("returned data: " + serverRequest.data.Replace("\n", "========="));
		
		JSONObject j = new JSONObject(serverRequest.text);
		JSONObject contentObj = j["content"]; // get the content for this returned data set
		
		JSONObject errorObj = j["error"]; // get the content for this returned data set
		
		if(errorObj.str.Equals("")){
			retrievedCustomerName = contentObj.GetField("is_valid").ToString();
			retrievedCustomerComputer = contentObj.GetField("system_name").ToString();
			retrievedProductName = contentObj.GetField("product_name").ToString();
			//Debug.Log("Customer Name = " + retrievedCustomerName);
			//Debug.Log("Customer Computer = " + retrievedCustomerComputer);
			
			// this calls to the delegate
			if(serialValidationResponseEventHandler != null)
				serialValidationResponseEventHandler(mySerialNumber, retrievedProductName, retrievedCustomerName, retrievedCustomerComputer, errorObj.str, true);
			
		}else{
			Debug.Log("Error with serial number");
			Debug.Log("Error = " + errorObj.str);
			
			// this calls to the delegate
			if(serialValidationResponseEventHandler != null)
				serialValidationResponseEventHandler(mySerialNumber, retrievedProductName, retrievedCustomerName, retrievedCustomerComputer, errorObj.str, true);
			
		}
		
		//Debug.Log("Got customer name = " + o
		
		//Debug.log(arr[2].n);
		//accessData(j);
		//returned data: {"content":{"id":"6","customer_id":"3","product_id":"2","serialnumber":"be9d34af43b578756cc08d840a79fc58","name":"Testing Serial Number"},"type":"serialnumber_customer_info","error":""}
		//retrievedCustomerName = 
		//retrievedCustomerComputer=
		
		//return serverRequest;	
	}
	*/

		public IEnumerator MakeRequestIfSerialNumberIsValid(string jsonEncodedString){
		
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
		postHeader.Add("Content-Length", jsonEncodedString.Length.ToString());
		
		this.serverRequest = new WWW(serverBaseUrl + "/api/v1/license/check?", System.Text.Encoding.UTF8.GetBytes(jsonEncodedString), postHeader);
		
		yield return serverRequest;
		
		Debug.Log("Text: " + serverRequest.text);
		//print("returned data: " + serverRequest.data.Replace("\n", "========="));
		
		JSONObject j = new JSONObject(serverRequest.text);
		JSONObject contentObj = j["content"]; // get the content for this returned data set
		JSONObject statusObj = j["status"]; 
		JSONObject errorObj = j["error"]; // get the content for this returned data set


		//if(errorObj.str.Equals("")){
		if(statusObj.GetField("is_valid").b == true){
			/*
			retrievedCustomerName = contentObj.GetField("is_valid").ToString();
			retrievedCustomerComputer = contentObj.GetField("system_name").ToString();
			retrievedProductName = contentObj.GetField("product_name").ToString();
			//Debug.Log("Customer Name = " + retrievedCustomerName);
			//Debug.Log("Customer Computer = " + retrievedCustomerComputer);
			*/

			// this calls to the delegate
			if(serialValidationResponseEventHandler != null)
				serialValidationResponseEventHandler(mySerialNumber, true, "", true);
			
		}else{
			Debug.Log("Error with serial number");
			Debug.Log("Error = " + errorObj.str);

			// this calls to the delegate
			if(serialValidationResponseEventHandler != null)
				serialValidationResponseEventHandler(mySerialNumber, false, errorObj.str, true);
			
		}
		
		//Debug.Log("Got customer name = " + o
		
		//Debug.log(arr[2].n);
		//accessData(j);
		//returned data: {"content":{"id":"6","customer_id":"3","product_id":"2","serialnumber":"be9d34af43b578756cc08d840a79fc58","name":"Testing Serial Number"},"type":"serialnumber_customer_info","error":""}
		//retrievedCustomerName = 
		//retrievedCustomerComputer=
		
		//return serverRequest;	
	}
	
	
	
	void handleNonValidSerialNumberInput(){
		Debug.Log("Not valid serial number input");	
	}
	
	void handleNonValidFloatInput(){
		Debug.Log("Not valid float number input");	
	}
	
	
	public void SetPreferenceFile(string file){
		pathToPrefFile = file;
		
		mySerialNumber = readSerialNumberFromXMLFile(); // read the serial number from the preferences file
	}
	
	public void writeSerialNumberToPreferences(){
		
			
		try{
			
			
			
			XmlDocument doc = new XmlDocument();
			doc.Load(pathToPrefFile);
		
			
			XmlNodeList xnList = doc.SelectNodes("/smallablearning/license");
			foreach (XmlNode xn in xnList){
				
				Debug.Log("Writing this serial number: " + mySerialNumber);
				
 				xn["serialnumber"].InnerText = mySerialNumber;
  				
			} 
						
			doc.Save(pathToPrefFile); // write the new data out
			
		}catch(System.Exception e){
		
			Debug.Log(e.ToString());
			
		}	
	}
	
	
	public string readSerialNumberFromXMLFile(){
		Debug.Log("Reading serial number");
		string serialNumber = "";
			
		try{
			XmlDocument doc = new XmlDocument();
			//doc.Load(Application.dataPath + "/../../" + "SMALLabLearningPreferences_v1.0.xml");
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
        
		//Debug.Log("Retrieved serial number: " + serialNumber);
		
		return serialNumber;
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
	
	/*
	IEnumerator CheckLicenseServerConnection(Action<bool> action){
		WWW www = new WWW("http://analytics.smallablearning.com");
		yield return www;
		if (www.error != null) {
			action (false);
		} else {
			action (true);
		}
	} 
	*/
	IEnumerator CheckLicenseServerConnection(Action<bool> action){
		WWW www = new WWW(serverBaseUrl);
		yield return www;
		if (www.error != null) {
			action (false);
		} else {
			action (true);
		}
	} 
	
	public static string GetPublicIPAddress()
	{
		string url = "http://checkip.dyndns.org";
		System.Net.WebRequest req = System.Net.WebRequest.Create(url);
		System.Net.WebResponse resp = req.GetResponse();
		System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
		string response = sr.ReadToEnd().Trim();
		string[] a = response.Split(':');
		string a2 = a[1].Substring(1);
		string[] a3 = a2.Split('<');
		string a4 = a3[0];
		return a4;
	}
	
	
}
