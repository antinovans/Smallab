using UnityEngine;
using System.Collections;

public class ObjectPositionScaleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
	

	void handleTrackedObjectData(TrackedObject [] trackedObjectArray){

		
	}
	
	void handlePhysicalDimensions(Vector3 dimensions){
		
		// scale this game object by the same as the space dimensions
		transform.localScale = Vector3.Scale(dimensions, transform.localScale); // multiply each component by the other
		transform.position = Vector3.Scale(dimensions, transform.position);
	}

}
