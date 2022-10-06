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

public class ObjectRotationScript : MonoBehaviour {

	
	public int myTrackedObjectID = 1;
	public bool disableYAxis;

	// Use this for initialization
	void Start () {
		disableYAxis = true;
	}
	
	// Update is called once per frame
	void Update () {


	}
	
	void handleTrackedObjectData(TrackedObject [] trackedObjectArray){
		
		foreach(TrackedObject trackedObject in trackedObjectArray){ // go through each of the tracked objects
			
			if(trackedObject.id == myTrackedObjectID){ // make sure that it's a match to the id that I want
                if (disableYAxis)
                {
					Vector3 newPos = new Vector3(trackedObject.position.x, 0 , trackedObject.position.z);
					if (this.GetComponent<Rigidbody>())
					{ //If there is an attached Rigidbody on this tracked object
						this.GetComponent<Rigidbody>().MovePosition(newPos); // Interpolate the position to the same as the tracked object
						this.GetComponent<Rigidbody>().MoveRotation(trackedObject.rotation); // Interpolate the rotation to the same as the tracked object
					}
					else
					{ //If this tracked object does not have a Rigidbody
						transform.position = newPos; // set the position the same as the tracked object
						transform.rotation = trackedObject.rotation; // set the rotation the same as the tracked object
					}
				}
                else
                {
					if (this.GetComponent<Rigidbody>())
					{ //If there is an attached Rigidbody on this tracked object
						this.GetComponent<Rigidbody>().MovePosition(trackedObject.position); // Interpolate the position to the same as the tracked object
						this.GetComponent<Rigidbody>().MoveRotation(trackedObject.rotation); // Interpolate the rotation to the same as the tracked object
					}
					else
					{ //If this tracked object does not have a Rigidbody
						transform.position = trackedObject.position; // set the position the same as the tracked object
						transform.rotation = trackedObject.rotation; // set the rotation the same as the tracked object
					}
				}
                
            }
		}
		
	}
	
	void handlePhysicalDimensions(Vector3 dimensions){
		
		// scale this game object by the same as the space dimensions
		transform.localScale = Vector3.Scale(dimensions, transform.localScale); // multiply each component by the other
		//transform.position = Vector3.Scale(dimensions, transform.position);
	}

}
