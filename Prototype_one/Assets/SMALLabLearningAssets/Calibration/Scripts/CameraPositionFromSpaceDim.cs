using UnityEngine;
using System.Collections;

public class CameraPositionFromSpaceDim : MonoBehaviour {

	//Vector3 spaceDimensions;
	
	
	// Use this for initialization
	void Start () {
		
		/*
		// make this scale to the dimensions of the containing interactive space 
		spaceDimensions = ((Dimensions)GameObject.Find("InteractiveSpaceDimensions").GetComponent("Dimensions")).dimensions;
		
		float posY = spaceDimensions.x;
		Vector3 cameraPosition = new Vector3(0.0f, posY, 0.0f);
		
		transform.position = cameraPosition;
		*/

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void handlePhysicalDimensions(Vector3 dimensions){
		
		float posY = dimensions.x * 4.0f;
		Vector3 cameraPosition = new Vector3(0.0f, posY, 0.0f);
		
		transform.position = cameraPosition;
				
		// scale this game object by the same as the space dimensions
		//transform.localScale = Vector3.Scale(dimensions, transform.localScale); // multiply each component by the other
		
	}
}
