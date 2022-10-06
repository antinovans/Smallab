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

public class JoystickOptitrackSimulator : MonoBehaviour {

	TrackedObjects trackedObjects;
	
	float x;
	float y;
	float speedScalar;
	public bool useJoystickSim = true;
	
	Vector3 newPosition;
	Vector3 rawInput;
	public int trackableID = 1;

	void Awake(){
		speedScalar = 1.0f;
		
		string [] joystickNames = Input.GetJoystickNames();
		int counter;
		for(counter = 0; counter < joystickNames.Length; counter++){
			Debug.Log ("joystick " + counter + " = " + joystickNames[counter]);
		}
		//Debug.Log (Input.GetJoystickNames()[i]+" is moved");
			
	}

	// Use this for initialization
	void Start () {
		trackedObjects = (TrackedObjects) GameObject.Find("Trackables").GetComponent("TrackedObjects");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		rawInput = Vector3.zero; // clear it out
		
		if(useJoystickSim){
		
			
			if(Input.GetKey(KeyCode.LeftArrow)){
				rawInput = new Vector3(-1.0f, rawInput.y, rawInput.z);	
			}
			if(Input.GetKey(KeyCode.RightArrow)){
				rawInput = new Vector3(1.0f, rawInput.y, rawInput.z);	
			}
			if(Input.GetKey(KeyCode.UpArrow)){
				rawInput = new Vector3(rawInput.x, rawInput.y, 1.0f);	
			}
			if(Input.GetKey(KeyCode.DownArrow)){
				rawInput = new Vector3(rawInput.x, rawInput.y, -1.0f);	
			}
			if(Input.GetKey(KeyCode.O)){
				rawInput = new Vector3(rawInput.x, 1.0f, rawInput.z);	
			}
			if(Input.GetKey(KeyCode.L)){
				rawInput = new Vector3(rawInput.x, -1.0f, rawInput.z);	
			}

			
			// get the values ranging from -1 to 1
			/** if you're not getting movement along the y-axis, you need to setup your inputs in the project
			/* go Edit -> Project Settings -> Input edite on of the axes to be labeled 'YMovement' and define keys to go along with it */
			//rawInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("YMovement"), Input.GetAxis("Vertical"));
			
			//Debug.Log ("got values " + rawInput.x + ", " + rawInput.y + ", " + rawInput.z + ")");
			
			// have it move, but scaled to this scalar speed per second
			rawInput = rawInput * (speedScalar * Time.deltaTime);
    
			//Debug.Log("X position = " + x);
			if(trackedObjects){ // make sure we have a valid object before we try to update it
				//trackedObjects.setTrackedObjectVelocity(trackableID, x, y, z);
				newPosition = transform.position + rawInput;
				if(newPosition.x > transform.localScale.x * 0.5f)
					newPosition.x = transform.localScale.x * 0.5f;
				if(newPosition.x < transform.localScale.x * -0.5f)
					newPosition.x = transform.localScale.x * -0.5f;
				if(newPosition.y > transform.localScale.y * 1.0f)
					newPosition.y = transform.localScale.y * 1.0f;
				if(newPosition.y < 0.0f)
					newPosition.y = 0.0f;
				if(newPosition.z > transform.localScale.z * 0.5f)
					newPosition.z = transform.localScale.z * 0.5f;
				if(newPosition.z < transform.localScale.z * -0.5f)
					newPosition.z = transform.localScale.z * -0.5f;
				
	
				trackedObjects.setTrackedObjectPosition(trackableID, newPosition);
				
			}
		}
	}
	
	void handleTrackedObjectData(TrackedObject [] trackedObjectArray){
		
		foreach(TrackedObject trackedObject in trackedObjectArray){ // go through each of the tracked objects
			
			if(trackedObject.id == trackableID){ // make sure that it's a match to the id that I want
				transform.position = trackedObject.position; // set the position the same as the tracked object
				transform.rotation = trackedObject.rotation; // set the rotation the same as the tracked object
			}
		}
		
	}
	
	
	void handlePhysicalDimensions(Vector3 dimensions){
		
		// scale this game object by the same as the space dimensions
		transform.localScale = Vector3.Scale(dimensions, transform.localScale); // multiply each component by the other
		//transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
	}
}
