using UnityEngine;
using System;
using System.Collections;

/*
 * VERSION 3.0
 */

public class TrackedObjects : MonoBehaviour {

	object[] allGameObjects;
	
	//private struct TrackedObject { public UInt32 id; public Vector3 position; public Quaternion quaternion;}
	
	public TrackedObject [] trackedObjectArray = {new TrackedObject(), new TrackedObject(), new TrackedObject(), new TrackedObject(), new TrackedObject(), new TrackedObject()};
	
	//public TrackedObject trackable1, trackable2, trackable2, trackable2, trackable2, trackable2;

	void Awake () {
		
		// start the numbering from 1 (rather than 0)
		for(int x = 1; x < trackedObjectArray.Length + 1; x++){
			//trackedObjectArray[x] = new TrackedObject();
			trackedObjectArray[x - 1].setID(x);
		}
	
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		broadcastTrackableData();
	}
	
	// update the location of the object
	public void setTrackedObjectQuaternion(int id, float x, float y, float z, float w){
		//Debug.Log("Setting trackable location with id = " + id + " to position (" + x + ", " + y + ", " + z + ")");
		//trackable1.position = new Vector3(x, y, z);		
		
		trackedObjectArray[id - 1].rotation.x = x;
		trackedObjectArray[id - 1].rotation.y = y;
		trackedObjectArray[id - 1].rotation.z = z;
		trackedObjectArray[id - 1].rotation.w = w;
	}
	public void setTrackedObjectQuaternion(int id, Quaternion quaternion){
		//Debug.Log("Setting trackable with id = " + id + " to quaternion (" + quaternion.x + ", " + quaternion.y + ", " + quaternion.z + ", " + quaternion.w + ")");
		//trackable1.position = new Vector3(x, y, z);		
		
		trackedObjectArray[id - 1].rotation = quaternion;

	}

    public void setTrackedObjectMeanMarkerError(int id, float meanMarkerError)
    {
        trackedObjectArray[id - 1].meanMarkerError = meanMarkerError;
    }

    // update the location of the object
    public void setTrackedObjectPosition(int id, float x, float y, float z){
		//Debug.Log("Setting trackable location with id = " + id + " to position (" + x + ", " + y + ", " + z + ")");
		//trackable1.position = new Vector3(x, y, z);		
		trackedObjectArray[id - 1].position.x = x;
		trackedObjectArray[id - 1].position.y = y;
		trackedObjectArray[id - 1].position.z = z;		
	}
	public void setTrackedObjectPosition(int id, Vector3 position){
		//Debug.Log("Setting trackable location with id = " + id + " to position (" + x + ", " + y + ", " + z + ")");
		//trackable1.position = new Vector3(x, y, z);		
		trackedObjectArray[id - 1].position = position;

	}
	
	// update the location of the object
	public void setTrackedObjectVelocity(int id, float x, float y, float z){
				
		
	}
	
	public Vector3 getTrackablePosition(int id){
		return trackedObjectArray[id].position;
	}
	public Quaternion getTrackableQuaternion(int id){
		return trackedObjectArray[id].rotation;
	}
	
	// this will send out the information for the trackables
	public void broadcastTrackableData(){
		//Debug.Log("broadcasting trackable data");
	
		allGameObjects = GameObject.FindSceneObjectsOfType(typeof (GameObject));
  		foreach (object o in allGameObjects){
       		GameObject g = (GameObject) o;
       		
			g.SendMessage("handleTrackedObjectData", trackedObjectArray, SendMessageOptions.DontRequireReceiver);
			
			/*
       		foreach(TrackedObject trackedObject in trackedObjectArray){
				g.SendMessage("handleTrackedObjectData", trackedObjectArray, SendMessageOptions.DontRequireReceiver);
			}
			*/
       		
   		}
			
	}
		
					
}
