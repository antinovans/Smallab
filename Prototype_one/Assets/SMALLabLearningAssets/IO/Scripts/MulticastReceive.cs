/*

Copyright © 2011.  Arizona Board of Regents.  All Rights Reserved
 
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Xml;

public class MulticastReceive : MonoBehaviour 
{
	
    /*
     * VERSION 3.0
    */

	//EditorState editorState = new EditorState("editorstate");
	InternalTrackedObject [] bodyArray;
	
	private bool enableUPDRead = true;
	
	//private string strMessage = ""; 
	private struct ReceivedMessage { public float fTime; public string strIP; public bool bIsReady;} 
	private struct ReceiveOperationInfo { public UdpClient objUDPClient; public IPEndPoint objIPEndPoint;} 
	//private string strServerNotReady = "notready";// "I wanna be a server!"; 
    //private string strServerReady = "areready"; // "All hail, I am a server"; 
    List<ReceivedMessage> lstReceivedMessages; 
    
    private struct InternalTrackedObject { public UInt32 id; public Vector3 position; public Quaternion quaternion; public float meanMarkerError; public short trackingFlag;}
    TrackedObjects trackedObjects;
    
    // reusable variables for parsing the tracked data
    UInt32 numMarkers;
    UInt32 numRigidBodies; // this is set by the incoming tracked data
	string[] dataString;
	string strReceivedMessage;
	int byteIndexCounter;
	int messageID;
	int bytesInPayload;
	UInt32 frameNumber;
	UInt32 numberOfModels;
	UInt32 numUnLabeledMarkers;
	float meanMarkerError;
	float markerSize;
	float body_pos_x, body_pos_y, body_pos_z;
	UInt32 markerID;

    //private static readonly IPAddress GroupAddress = IPAddress.Parse("224.0.0.1"); // this is the optitrack data
    private static readonly IPAddress GroupAddress = IPAddress.Parse("239.255.42.99"); // this is the optitrack data
    //private static readonly IPAddress GroupAddress = IPAddress.Parse("127.0.0.1"); // this is the optitrack data
    private const int GroupPort = 60001; // this is optitrack data port to work on Mac and Windows
	//private const int GroupPort = 1510; // this is optitrack data port to work on Mac and Windows
	
	UdpClient listener;
	IPEndPoint groupEP;

	
	public float NatNetVersion = -1.0f;
	
	
	public TrackedObject [] rawTrackedObjectArray = {new TrackedObject(), new TrackedObject(), new TrackedObject(), new TrackedObject(), new TrackedObject(), new TrackedObject()};


	void Awake(){
		
		// Get the NatNet Version from preferences so we can properly parse the incoming stream from Tracking Tools/Motive
		NatNetVersion = ReadNatNetVersionFromXMLPreferences ();
		
	}


	void Start () 
    { 
       // reference the object that will actually store the tracked object positions and rotations
       trackedObjects = (TrackedObjects) GameObject.Find("Trackables").GetComponent("TrackedObjects");
       
    } 
    
    void FixedUpdate(){
    	
    	// only do this at the rendering frame rate
    	if(trackedObjects){ // make sure that we have this object
    		for(int x = 0; x < rawTrackedObjectArray.Length; x++){
				
				if(rawTrackedObjectArray[x].id != -1){ // make sure that we have a valid object id
					//Debug.Log("Sending data: id " + rawTrackedObjectArray[x].id);
	    			trackedObjects.setTrackedObjectPosition(rawTrackedObjectArray[x].id, rawTrackedObjectArray[x].position);
	    			trackedObjects.setTrackedObjectQuaternion(rawTrackedObjectArray[x].id, rawTrackedObjectArray[x].rotation);
                    trackedObjects.setTrackedObjectMeanMarkerError(rawTrackedObjectArray[x].id, rawTrackedObjectArray[x].meanMarkerError);

                }
				
    	    	//trackedObjects.setTrackedObjectLocation(x, objectArray[x].x, objectArray[x].y, objectArray[x].z);
    			//trackedObjects.setTrackedObjectQuaternion(x, objectQuatArray[x].x, objectQuatArray[x].y, objectQuatArray[x].z, objectQuatArray[x].w);
    		}
    	}
    }
    
    
    
    void OnEnable(){
    	trackedObjects = (TrackedObjects) GameObject.Find("Trackables").GetComponent("TrackedObjects");
       
    	Debug.Log("UDP Setup");
		UDPSetup();
		

		enableUPDRead = true;
		BeginAsyncReceive(); // this is the thread
    	
    	

	}
    
    void OnDisable(){
    	enableUPDRead = false;	
    	
    }
    
	public void CloseUDPConnection(){
		enableUPDRead = false;
		listener.Close ();
	}

	void UDPSetup(){


        // ORIGINAL SETUP
        /*
		listener = new UdpClient(GroupPort); // original
		listener.JoinMulticastGroup(GroupAddress);
		groupEP = new IPEndPoint(GroupAddress, GroupPort);
		//groupEP = new IPEndPoint(IPAddress.Any, GroupPort);
		*/


        listener = new UdpClient(GroupPort); // original
        //groupEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), GroupPort);
        //groupEP = new IPEndPoint(IPAddress.Loopback, GroupPort);
        groupEP = new IPEndPoint(IPAddress.Any, GroupPort);
        //groupEP = new IPEndPoint(GroupAddress, GroupPort);
        listener.JoinMulticastGroup(GroupAddress);

        /*
		listener = new UdpClient();
		IPEndPoint localEp = new IPEndPoint(IPAddress.Loopback, GroupPort);

		listener.Client.Bind(localEp);

		listener.JoinMulticastGroup(GroupAddress);
		//groupEP = new IPEndPoint(GroupAddress, GroupPort);
		*/

        /*
		listener = new UdpClient(GroupPort); // original
                                             //listener.MulticastLoopback = true;
                                             //listener.JoinMulticastGroup(GroupAddress);
                                             //listener.Connect(new IPEndPoint(IPAddress.Loopback, GroupPort));
        listener.JoinMulticastGroup(GroupAddress);
        //listener.Connect(new IPEndPoint(GroupAddress, GroupPort));


        groupEP = new IPEndPoint(GroupAddress, GroupPort);
		//groupEP = new IPEndPoint(IPAddress.Parse("169.254.228.216"), GroupPort);
        */
    }




    // Method to start an Async receive procedure 
    private void BeginAsyncReceive() 
    { 
        //Debug.Log("Inside BeginReceive"); 
        ReceiveOperationInfo objInfo = new ReceiveOperationInfo(); 
        objInfo.objUDPClient = listener; 
        //objInfo.objIPEndPoint = new IPEndPoint(IPAddress.Broadcast, scrConnectionManager.iUDPPortNumber); 
        listener.BeginReceive(new AsyncCallback(EndAsyncReceive), objInfo);
    } 
    
	private void EndAsyncReceive(IAsyncResult objResult){
		


		if (NatNetVersion < 2.6) {
			ProcessDataForNatNetVersion_02_00 (objResult);
		} else if (NatNetVersion >= 2.6 && NatNetVersion < 3.0) {
			// this is for Motive Version 1.6, NatNet Version 2.6
			ProcessDataForNatNetVersion_02_60 (objResult);
		} else if (NatNetVersion >= 3.0) {
			ProcessDataForNatNetVersion_03_00 (objResult);
		}

	}
    
    /*
	private void ProcessDataForNatNetVersion_Unknown(IAsyncResult objResult) 
	{        
		
		byte[] optitrackdata = listener.Receive(ref groupEP); 
		
		if (optitrackdata.Length > 0) {
			//Debug.Log("Message received"); 
			strReceivedMessage = System.Text.Encoding.ASCII.GetString(optitrackdata);
			//parseIncomingData(strReceivedMessage);
			
			dataString = strReceivedMessage.Split(' ');
			
			//Debug.Log("Received this data: " + strReceivedMessage);
			
			byteIndexCounter = 0;
			
			//byte [] optitrackdata = datagramPacket.getData();
			
			//byte[] messageIDBytes = new byte[]{optitrackdata[0], optitrackdata[1]};
			
			// this is the type of data coming in
			//int messageID = byteConversion.arr2short(optitrackdata, byteIndexCounter);
			messageID = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
			byteIndexCounter += 2;
			
			//bytesInPayload = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
			//byteIndexCounter += 2;
			
			if(messageID != 7){
				Debug.Log ("Unknown NatNetVersion. Message ID = " + messageID);
			}else{
				//GetNatNetVersion();
			}
			
			
		}
		if(enableUPDRead){
			BeginAsyncReceive(); 
		}
		
	}
	*/

	// Callback method from the UDPClient, when the async receive procedure received a message 
    private void ProcessDataForNatNetVersion_02_00(IAsyncResult objResult) 
    {        
        
        byte[] optitrackdata = listener.Receive(ref groupEP); 
        
        if (optitrackdata.Length > 0) {
        	//Debug.Log("Message received"); 
        	strReceivedMessage = System.Text.Encoding.ASCII.GetString(optitrackdata);
        	//parseIncomingData(strReceivedMessage);
        
        	dataString = strReceivedMessage.Split(' ');
    		
    		//Debug.Log("Received this data: " + strReceivedMessage);
    		
    		byteIndexCounter = 0;
    		
    		//byte [] optitrackdata = datagramPacket.getData();
		
			//byte[] messageIDBytes = new byte[]{optitrackdata[0], optitrackdata[1]};
	    
			// this is the type of data coming in
			//int messageID = byteConversion.arr2short(optitrackdata, byteIndexCounter);
			messageID = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
			byteIndexCounter += 2;
					
			bytesInPayload = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
			byteIndexCounter += 2;
		
			frameNumber = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;
		
			numberOfModels = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;
		
			numUnLabeledMarkers = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;
			
			//Debug.Log("Message ID = " + messageID + ", Bytes in Payload = " + bytesInPayload + ", Frame Number = " + frameNumber + ", Number of Models = " + numberOfModels + ", Num Unlabeled Markers = " + numUnLabeledMarkers);
			

		
			// step through a triple for each of the markers
			for(int x = 0; x < numUnLabeledMarkers; x++){
				byteIndexCounter += 4; // read x
				byteIndexCounter += 4; // read y
				byteIndexCounter += 4; // read z
			}
		
			numRigidBodies = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;
		
			// now initialize the array depending on the number of rigid bodies that are present
			bodyArray = new InternalTrackedObject[numRigidBodies];
		
			//if(numRigidBodies != 2)
			//	Debug.Log("Number of rigid bodies = " + numRigidBodies);
		
			// go through each of the rigid bodies
			for(int x = 0; x < numRigidBodies; x++){
				bodyArray[x] = new InternalTrackedObject();
			
				// this is the id of the rigid body
				bodyArray[x].id = BitConverter.ToUInt32(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 
			
				// x position of the body
				bodyArray[x].position.x = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 
			
				// optitrack vertical dimension
				bodyArray[x].position.y = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 
			
				// optitrack depth direction
				bodyArray[x].position.z = (-1.0f * BitConverter.ToSingle(optitrackdata, byteIndexCounter)); // read x
				//bodyArray[x].position.z = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x	
				byteIndexCounter += 4;
			
				// read the quaternion rotation values
				bodyArray[x].quaternion.x = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 
			
				bodyArray[x].quaternion.y = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read y
				byteIndexCounter += 4; 
			
				bodyArray[x].quaternion.z = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read z
				bodyArray[x].quaternion.z *= -1.0f;
				byteIndexCounter += 4; 
			
				bodyArray[x].quaternion.w = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read w
				byteIndexCounter += 4; 
			
				numMarkers = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
				byteIndexCounter += 4;
			
				
			
				for(int y = 0; y < numMarkers; y++){ // for each marker in the body, read the positions
					body_pos_x = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
					byteIndexCounter += 4; 
				
					body_pos_y = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
					byteIndexCounter += 4; 
				
					body_pos_z = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
					byteIndexCounter += 4;
				
					if(NatNetVersion >=2){
						// get the reading of the error
						markerID = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
						byteIndexCounter += 4;
					
						// get the reading of the error
						markerSize = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
						byteIndexCounter += 4;
					} // ends check if NatNetVersion 2
				
				} // ends the for loop through each marker in the body
			
		
				if(NatNetVersion >= 2){
					// get the reading of the error
					bodyArray[x].meanMarkerError = BitConverter.ToSingle(optitrackdata, byteIndexCounter);
					byteIndexCounter += 4;
					
					
				} // ends the check for the NatNetVersion
			
				
			} // ends the for loop through each of the rigid bodies

			// now that we've looped through, update the values based on what we got from the tracker			
			for(int x = 0; x < numRigidBodies; x++){
				
				//float dist = Vector3.Distance(bodyArray[x].position, new Vector3(0.0f, 0.0f, 0.0f));
				
				if(Vector3.Distance(bodyArray[x].position, new Vector3(0.0f, 0.0f, 0.0f)) != 0.0f){
					
					rawTrackedObjectArray[x].id = (int) bodyArray[x].id;
					//Debug.Log("set the id = " + bodyArray[x].id);
					rawTrackedObjectArray[x].position = bodyArray[x].position;
					rawTrackedObjectArray[x].rotation = Quaternion.Inverse(bodyArray[x].quaternion); // invert it to match Unity
                    rawTrackedObjectArray[x].meanMarkerError = bodyArray[x].meanMarkerError;
                    //Debug.Log("Mean marker error for object: " + x + " = " + bodyArray[x].meanMarkerError);
                }
				//Debug.Log("Mean marker error for object: " + x + " = " + bodyArray[x].meanMarkerError);
				
					/*
			if(bodyArray[x].id == 1){
					objectArray[0] = new Vector3(bodyArray[x].position.x, bodyArray[x].position.y, bodyArray[x].position.z);
					objectQuatArray[0] = new Quaternion(bodyArray[x].quaternion.x, bodyArray[x].quaternion.y, bodyArray[x].quaternion.z, bodyArray[x].quaternion.w);
					//Debug.Log("Setting trackable location with id = " + bodyArray[x].id + " to position (" + bodyArray[x].position.x + ", " + bodyArray[x].position.y + ", " + bodyArray[x].position.z + ")");
				}
				else if(bodyArray[x].id == 2){
					objectArray[1] = new Vector3(bodyArray[x].position.x, bodyArray[x].position.y, bodyArray[x].position.z);
					objectQuatArray[1] = new Quaternion(bodyArray[x].quaternion.x, bodyArray[x].quaternion.y, bodyArray[x].quaternion.z, bodyArray[x].quaternion.w);

					//Debug.Log("Setting trackable location with id = " + bodyArray[x].id + " to position (" + bodyArray[x].position.x + ", " + bodyArray[x].position.y + ", " + bodyArray[x].position.z + ")");
				//Debug.Log("updating object with id = 2");
				*/
			} // ends the for loop iterating through each of the rigid bodies
			
		} // ends the if the message length is greater than 0
    	
	   	if(enableUPDRead){
	       BeginAsyncReceive(); 
	   	}

	   	
    } 
    
    
	private void ProcessDataForNatNetVersion_02_60(IAsyncResult objResult) 
	{        
		//Debug.Log ("I got data from motive!");
		byte[] optitrackdata = listener.Receive(ref groupEP); 

		if (optitrackdata.Length > 0) {
			//Debug.Log("Message received"); 
			strReceivedMessage = System.Text.Encoding.ASCII.GetString(optitrackdata);
			//parseIncomingData(strReceivedMessage);
			
			dataString = strReceivedMessage.Split(' ');
			
			//Debug.Log("Received this data: " + strReceivedMessage);
			
			byteIndexCounter = 0;
			
			//byte [] optitrackdata = datagramPacket.getData();
			
			//byte[] messageIDBytes = new byte[]{optitrackdata[0], optitrackdata[1]};
			
			// this is the type of data coming in
			//int messageID = byteConversion.arr2short(optitrackdata, byteIndexCounter);
			messageID = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
			byteIndexCounter += 2;
			
			bytesInPayload = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
			byteIndexCounter += 2;
			
			frameNumber = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;
			
			numberOfModels = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;
			
			numUnLabeledMarkers = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;
			
			//Debug.Log("Message ID = " + messageID + ", Bytes in Payload = " + bytesInPayload + ", Frame Number = " + frameNumber + ", Number of Models = " + numberOfModels + ", Num Unlabeled Markers = " + numUnLabeledMarkers);
			
			
			
			// step through a triple for each of the markers
			for(int x = 0; x < numUnLabeledMarkers; x++){
				byteIndexCounter += 4; // read x
				byteIndexCounter += 4; // read y
				byteIndexCounter += 4; // read z
			}
			
			numRigidBodies = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;
			
			// now initialize the array depending on the number of rigid bodies that are present
			bodyArray = new InternalTrackedObject[numRigidBodies];
			
			//if(numRigidBodies != 2)
			//Debug.Log("Number of rigid bodies = " + numRigidBodies);
			
			// go through each of the rigid bodies
			for(int x = 0; x < numRigidBodies; x++){
				bodyArray[x] = new InternalTrackedObject(); 
				
				// this is the id of the rigid body
				bodyArray[x].id = BitConverter.ToUInt32(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 
				//Debug.Log ("Rigid Body ID: " + bodyArray[x].id);
				
				//byteIndexCounter += 2;
				//bodyArray[x].id = BitConverter.ToUInt16(optitrackdata, byteIndexCounter); // read x
				//byteIndexCounter += 2; 
				//bodyArray[x].id = BitConverter.ToUInt32(optitrackdata, byteIndexCounter); // read x
				//byteIndexCounter += 4; 
				
				// x position of the body
				bodyArray[x].position.x = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 
				
				// optitrack vertical dimension
				bodyArray[x].position.y = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 
				
				// optitrack depth direction
				bodyArray[x].position.z = (-1.0f * BitConverter.ToSingle(optitrackdata, byteIndexCounter)); // read x
				//bodyArray[x].position.z = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x	
				byteIndexCounter += 4;
				
				// read the quaternion rotation values
				bodyArray[x].quaternion.x = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 
				
				bodyArray[x].quaternion.y = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read y
				byteIndexCounter += 4; 
				
				bodyArray[x].quaternion.z = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read z
				bodyArray[x].quaternion.z *= -1.0f;
				byteIndexCounter += 4; 
				
				bodyArray[x].quaternion.w = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read w
				byteIndexCounter += 4; 
				
				
				numMarkers = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
				byteIndexCounter += 4;
				
				
				//Debug.Log ("Rigid Body ID: " + bodyArray[x].id + ", POSITION: (" + bodyArray[x].position.x + ", " + bodyArray[x].position.y + ", " + bodyArray[x].position.z + ")");
				
				
				for(int y = 0; y < numMarkers; y++){ // for each marker in the body, read the positions
					body_pos_x = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
					byteIndexCounter += 4; 
					
					body_pos_y = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
					byteIndexCounter += 4; 
					
					body_pos_z = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
					byteIndexCounter += 4;
					
					if(NatNetVersion >=2){
						// get the reading of the error
						markerID = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
						byteIndexCounter += 4;
						
						// get the reading of the error
						markerSize = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
						byteIndexCounter += 4;
						
						
					} // ends check if NatNetVersion 2
					
				} // ends the for loop through each marker in the body
				
				
				// get the reading of the error
				bodyArray[x].meanMarkerError = BitConverter.ToSingle(optitrackdata, byteIndexCounter);
				byteIndexCounter += 4;
					
				
				// In NatNet 2.7, they added a new parameter which is the state of the trackable
				bodyArray[x].trackingFlag = BitConverter.ToInt16(optitrackdata, byteIndexCounter);
				byteIndexCounter += 2;
				
				//Debug.Log ("Mean Error: " + bodyArray[x]. meanMarkerError + ", Tracking Flag: " + bodyArray[x].trackingFlag);
				
				
			} // ends the for loop through each of the rigid bodies
			//Debug.Log ("");
			//Debug.Log ("START PROCESSING RIGID BODIES");
			// now that we've looped through, update the values based on what we got from the tracker			
			for(int x = 0; x < numRigidBodies; x++){
				
				//Debug.Log ("Processing rigid body with id = " + (int) bodyArray[x].id);
				//float dist = Vector3.Distance(bodyArray[x].position, new Vector3(0.0f, 0.0f, 0.0f));
				
				if(Vector3.Distance(bodyArray[x].position, new Vector3(0.0f, 0.0f, 0.0f)) != 0.0f){
					
					rawTrackedObjectArray[x].id = (int) bodyArray[x].id;
					//Debug.Log("set the id = " + bodyArray[x].id);
					rawTrackedObjectArray[x].position = bodyArray[x].position;
					rawTrackedObjectArray[x].rotation = Quaternion.Inverse(bodyArray[x].quaternion); // invert it to match Unity
                    rawTrackedObjectArray[x].meanMarkerError = bodyArray[x].meanMarkerError;

                    //Debug.Log("Mean marker error for object: " + x + " = " + bodyArray[x].meanMarkerError);
                }
				//Debug.Log("Mean marker error for object: " + x + " = " + bodyArray[x].meanMarkerError);

			} // ends the for loop iterating through each of the rigid bodies

		} // ends the if the message length is greater than 0

		if(enableUPDRead){
			BeginAsyncReceive(); 
		}
		
		
	} 

	private void ProcessDataForNatNetVersion_03_00(IAsyncResult objResult) 
	{        
		byte[] optitrackdata = listener.Receive(ref groupEP); 

		if (optitrackdata.Length > 0) {
			//Debug.Log("Message received"); 
			strReceivedMessage = System.Text.Encoding.ASCII.GetString(optitrackdata);
			//parseIncomingData(strReceivedMessage);

			dataString = strReceivedMessage.Split(' ');

			//Debug.Log("Received this data: " + strReceivedMessage);

			byteIndexCounter = 0;

			//byte [] optitrackdata = datagramPacket.getData();

			//byte[] messageIDBytes = new byte[]{optitrackdata[0], optitrackdata[1]};

			// this is the type of data coming in
			//int messageID = byteConversion.arr2short(optitrackdata, byteIndexCounter);
			messageID = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
			byteIndexCounter += 2;

			bytesInPayload = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
			byteIndexCounter += 2;

			frameNumber = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;

			numberOfModels = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;

			numUnLabeledMarkers = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;

			//Debug.Log("Message ID = " + messageID + ", Bytes in Payload = " + bytesInPayload + ", Frame Number = " + frameNumber + ", Number of Models = " + numberOfModels + ", Num Unlabeled Markers = " + numUnLabeledMarkers);



			// step through a triple for each of the markers
			for(int x = 0; x < numUnLabeledMarkers; x++){
				byteIndexCounter += 4; // read x
				byteIndexCounter += 4; // read y
				byteIndexCounter += 4; // read z
			}

			numRigidBodies = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
			byteIndexCounter += 4;

			// now initialize the array depending on the number of rigid bodies that are present
			bodyArray = new InternalTrackedObject[numRigidBodies];

			//if(numRigidBodies != 2)
			//Debug.Log("Number of rigid bodies = " + numRigidBodies);

			// go through each of the rigid bodies
			for(int x = 0; x < numRigidBodies; x++){
				bodyArray[x] = new InternalTrackedObject(); 

				// this is the id of the rigid body
				bodyArray[x].id = BitConverter.ToUInt32(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 
				//Debug.Log ("Rigid Body ID: " + bodyArray[x].id);

				//byteIndexCounter += 2;
				//bodyArray[x].id = BitConverter.ToUInt16(optitrackdata, byteIndexCounter); // read x
				//byteIndexCounter += 2; 
				//bodyArray[x].id = BitConverter.ToUInt32(optitrackdata, byteIndexCounter); // read x
				//byteIndexCounter += 4; 

				// x position of the body
				bodyArray[x].position.x = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 

				// optitrack vertical dimension
				bodyArray[x].position.y = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 

				// optitrack depth direction
				bodyArray[x].position.z = (-1.0f * BitConverter.ToSingle(optitrackdata, byteIndexCounter)); // read x
				//bodyArray[x].position.z = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x	
				byteIndexCounter += 4;

				// read the quaternion rotation values
				bodyArray[x].quaternion.x = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
				byteIndexCounter += 4; 

				bodyArray[x].quaternion.y = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read y
				byteIndexCounter += 4; 

				bodyArray[x].quaternion.z = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read z
				bodyArray[x].quaternion.z *= -1.0f;
				byteIndexCounter += 4; 

				bodyArray[x].quaternion.w = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read w
				byteIndexCounter += 4; 

				/** IN NAT NET 3.0, they removed the marker data per rigid bodies 
				 * 
				 * http://v20.wiki.optitrack.com/index.php?title=NatNet_SDK_3.0#Update_Notes
				 * Minor changes on the data structure 
				/*
				numMarkers = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
				byteIndexCounter += 4;


				Debug.Log ("Rigid Body ID: " + bodyArray[x].id + ", POSITION: (" + bodyArray[x].position.x + ", " + bodyArray[x].position.y + ", " + bodyArray[x].position.z + ") Num Markers = " + numMarkers);


				for(int y = 0; y < numMarkers; y++){ // for each marker in the body, read the positions
					
					body_pos_x = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
					byteIndexCounter += 4; 

					body_pos_y = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
					byteIndexCounter += 4; 

					body_pos_z = BitConverter.ToSingle(optitrackdata, byteIndexCounter); // read x
					byteIndexCounter += 4;

					Debug.Log("Marker position " + y + " = (" + body_pos_x + ", " + body_pos_y + ", " + body_pos_z + ")");

					if(NatNetVersion >= 2){
						// get the reading of the error
						markerID = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
						byteIndexCounter += 4;

						// get the reading of the error
						markerSize = BitConverter.ToUInt32(optitrackdata, byteIndexCounter);
						byteIndexCounter += 4;


					} // ends check if NatNetVersion 2

				} // ends the for loop through each marker in the body
				*/

				// get the reading of the error
				bodyArray[x].meanMarkerError = BitConverter.ToSingle(optitrackdata, byteIndexCounter);
				byteIndexCounter += 4;


				// In NatNet 2.7, they added a new parameter which is the state of the trackable
				bodyArray[x].trackingFlag = BitConverter.ToInt16(optitrackdata, byteIndexCounter);
				byteIndexCounter += 2;

				//Debug.Log ("Mean Error: " + bodyArray[x]. meanMarkerError + ", Tracking Flag: " + bodyArray[x].trackingFlag);


			} // ends the for loop through each of the rigid bodies

			/**************************************************/
			/**************************************************/
			/**************************************************/
			//Debug.Log ("");
			//Debug.Log ("START PROCESSING RIGID BODIES");
			// now that we've looped through, update the values based on what we got from the tracker			
			for(int x = 0; x < numRigidBodies; x++){

				//Debug.Log ("Processing rigid body with id = " + (int) bodyArray[x].id);
				//float dist = Vector3.Distance(bodyArray[x].position, new Vector3(0.0f, 0.0f, 0.0f));

				if(Vector3.Distance(bodyArray[x].position, new Vector3(0.0f, 0.0f, 0.0f)) != 0.0f){

					rawTrackedObjectArray[x].id = (int) bodyArray[x].id;
					//Debug.Log("set the id = " + bodyArray[x].id);
					rawTrackedObjectArray[x].position = bodyArray[x].position;
					rawTrackedObjectArray[x].rotation = Quaternion.Inverse(bodyArray[x].quaternion); // invert it to match Unity
                    rawTrackedObjectArray[x].meanMarkerError = bodyArray[x].meanMarkerError;
                }
				//Debug.Log("Mean marker error for object: " + x + " = " + bodyArray[x].meanMarkerError);

			} // ends the for loop iterating through each of the rigid bodies

		} // ends the if the message length is greater than 0

		if(enableUPDRead){
			BeginAsyncReceive(); 
		}


	} 
    
    
	void GetNatNetVersion(){
		
		// send NAT_PING = 0 - must be 2 byte int
		// client app name = "smallab" - must be 256 chars
		// client app version = 0.91.0.0 - major.minor.build.revision - must be unsigned char[4]
		// nat net version = 0.0.0.0 - must be unsigned char[4]
		
		// {0, 'smallab', '0.0.0.0', '0.0.0.0'}
		//byte [] messageToSend = Encoding.ASCII.GetBytes ("00");
		//byte [] messageToSend = BitConverter.GetBytes(0);
		
		short shortValue = 225;
		byte[] tempBytes = BitConverter.GetBytes(shortValue);
		//if (BitConverter.IsLittleEndian)
		//	Array.Reverse(tempBytes);
		byte[] intBytes = tempBytes;
		
		
		byte [] clientAppNameBytes = new byte [256];
		string clientName = "MyAppName";
		System.Buffer.BlockCopy( Encoding.ASCII.GetBytes (clientName), 0, clientAppNameBytes, 0, Encoding.ASCII.GetBytes (clientName).Length );
				
		byte [] clientVersionBytes = Encoding.ASCII.GetBytes(new char[]{'0', '0', '0', '0'}); // it's an array of 4 chars, a char is 2 bytes
		byte [] clientNatNetVersionBytes = Encoding.ASCII.GetBytes(new char[]{'0', '0', '0', '0'});
		
		byte[] messageToSend = new byte[ intBytes.Length + clientAppNameBytes.Length + clientVersionBytes.Length + clientNatNetVersionBytes.Length ];
		System.Buffer.BlockCopy( intBytes, 0, messageToSend, 0, intBytes.Length );
		System.Buffer.BlockCopy( clientAppNameBytes, 0, messageToSend, intBytes.Length, clientAppNameBytes.Length );
		System.Buffer.BlockCopy( clientVersionBytes, 0, messageToSend, intBytes.Length + clientAppNameBytes.Length, clientVersionBytes.Length );
		System.Buffer.BlockCopy( clientNatNetVersionBytes, 0, messageToSend, intBytes.Length + clientAppNameBytes.Length + clientVersionBytes.Length, clientNatNetVersionBytes.Length );
		
		Debug.Log ("Sending request to NatNet Server of length: " + messageToSend.Length);
		listener.Send (messageToSend, messageToSend.Length, groupEP);
		
		// should return name, version, NAT_PINGRESPONES
		
		//SyncReceive();
		//enableUPDRead = true;
		//BeginAsyncReceive();
		// this is to indicate that we're compatible with NatNet 2.7
		//NatNetVersion = 2.7f;
		
		
		
	}


	private float ReadNatNetVersionFromXMLPreferences(){

			float version = 0.0f;
			
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
				
				// get the xml node that should contain the version
				XmlNode natNetVersionNode = doc.SelectSingleNode("/smallablearning/smallab/naturalpoint/natnetversion");
				
				if(natNetVersionNode != null){
					float newValue;
					if(float.TryParse(natNetVersionNode.InnerText, out newValue)){
						version = newValue;
					}else{ // if the preference is invalid for whatever reason, set the default to NatNet 2.0
						version = 2.0f;
					}
				}else{ // if the preference hasn't been set, default to NatNet 2.0
					version = 2.0f;
				}
				
			}catch(System.Exception e){
				
				Debug.Log(e.ToString());
				
			}
			
			Debug.Log("Retrieved Nat Net Version: " + version);
			
			return version;
	
	}


	
	private void ProcessServerDescriptionData(byte[] optitrackdata){
		
		if (optitrackdata.Length > 0) {
			//Debug.Log("Message received"); 
			strReceivedMessage = System.Text.Encoding.ASCII.GetString(optitrackdata);
			//parseIncomingData(strReceivedMessage);
			
			dataString = strReceivedMessage.Split(' ');
			
			//Debug.Log("Received this data: " + strReceivedMessage);
			
			byteIndexCounter = 0;
			
			//byte [] optitrackdata = datagramPacket.getData();
			
			//byte[] messageIDBytes = new byte[]{optitrackdata[0], optitrackdata[1]};
			
			// this is the type of data coming in
			//int messageID = byteConversion.arr2short(optitrackdata, byteIndexCounter);
			messageID = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
			byteIndexCounter += 2;
			
			bytesInPayload = BitConverter.ToUInt16(optitrackdata, byteIndexCounter);
				 byteIndexCounter += 2;
		}
	}
		// Method to start a Sync receiving (will hang until a message is received) 
    private void SyncReceive() 
    { 
        //Debug.Log("Inside SyncReceive"); 
        //IPEndPoint objSendersIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] objByteMessage = listener.Receive(ref groupEP); 
        //Debug.Log("Message received"); 
        string strReceivedMessage = System.Text.Encoding.ASCII.GetString(objByteMessage); 
        //Debug.Log("Message reads: " + strReceivedMessage);
        
		if (objByteMessage.Length > 0) {
			//Debug.Log("Message received"); 
			strReceivedMessage = System.Text.Encoding.ASCII.GetString(objByteMessage);
			dataString = strReceivedMessage.Split(' ');
			
			byteIndexCounter = 0;
			messageID = BitConverter.ToUInt16(objByteMessage, byteIndexCounter);
			byteIndexCounter += 2;
			
				//Debug.Log ("Received a message with id: " + messageID);
			
			// it's a NAT_PINGRESPONSE response if it comes as a 1
			if(messageID == 1){
				
			}
			
		}
    } 
    // Method to start this object recieving LAN Broadcast messages 
    private void StartReceiving() 
    { 
        //lstReceivedMessages.Clear(); 
        //BeginAsyncReceive(); 
        //SyncReceive(); 
    } 
    // Method to stop this object recieving LAN Broadcast messages 
    private void StopReceiving() 
    { 
        //IPEndPoint temp = new IPEndPoint(IPAddress.Any, 0); 
        //objUDPClient.EndReceive(CurrentReceiveProcedure, ref temp); 
    } 

	
}
