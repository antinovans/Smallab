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
using System.Collections;

public class MouseInputScript : MonoBehaviour {

	TrackedObjects trackedObjects;
	
	public Camera camera;
	
	Vector3 newPosition;
	Vector3 rawInput;
	public int trackableID = 1;
	
	float yDelta, yPosition;
	
	void Awake(){
		
		yDelta = 0.0f;
		yPosition = 0.0f;
	}

	// Use this for initialization
	void Start () {
		trackedObjects = (TrackedObjects) GameObject.Find("Trackables").GetComponent("TrackedObjects");
	}
	
	void Update(){
	
			if(Input.GetKeyDown("1")){
				trackableID = 1;
			}
			if(Input.GetKeyDown("2")){
				trackableID = 2;
			}
			if(Input.GetKeyDown("3")){
				trackableID = 3;
			}
			if(Input.GetKeyDown("4")){
				trackableID = 4;
			}
			if(Input.GetKeyDown("5")){
				trackableID = 5;
			}
			if(Input.GetKeyDown("6")){
				trackableID = 6;
			}
			
			
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(trackedObjects){ // make sure we have a valid object before we try to update it
			yDelta = Input.GetAxis("Mouse ScrollWheel");
			yPosition = yPosition + yDelta;
			
			// make sure that we can't go higher or lower than the physical space that we're trying to simulate
			if(yPosition > transform.localScale.y)
				yPosition = transform.localScale.y;
			if(yPosition < 0.0f)
				yPosition = 0.0f;
				
				
			//print("y position = " + yPosition + ", local scale = " + transform.localScale.y);
			
			newPosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.transform.position.y - yPosition));
			trackedObjects.setTrackedObjectPosition(trackableID, newPosition);
				
		}
		
	}
	
	/*
	void handleTrackedObjectData(TrackedObject [] trackedObjectArray){
		
		foreach(TrackedObject trackedObject in trackedObjectArray){ // go through each of the tracked objects
			
			if(trackedObject.id == trackableID){ // make sure that it's a match to the id that I want
				transform.position = trackedObject.position; // set the position the same as the tracked object
				transform.rotation = trackedObject.rotation; // set the rotation the same as the tracked object
			}
		}
		
	}
	*/
	
	
	void handlePhysicalDimensions(Vector3 dimensions){
		
		// scale this game object by the same as the space dimensions
		transform.localScale = Vector3.Scale(dimensions, transform.localScale); // multiply each component by the other
		//transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
	}
}
