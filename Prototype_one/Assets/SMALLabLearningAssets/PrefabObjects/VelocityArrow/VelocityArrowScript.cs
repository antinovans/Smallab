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

public class VelocityArrowScript : MonoBehaviour {

	//TrackedObjects trackedObjects;
	Vector3 windowedVelocity;
	Vector3 instantVelocity;
	Vector3 previousPosition;

	Vector3 workingVelocity;

	public float arrowLengthScalar = 1.0f;
	
	public int myTrackedObjectID = 1;

	// this determines the amount of filtering
	float kFilteringFactor;

	Transform arrowShapeTransform;
	Transform coneTransform;
	
	float timeIncrement = 0.04f;
	float currentTime = 0.0f;
	
	Vector3 currentTrackablePosition = Vector3.zero;
	
	public float speed = 0.0f;

	// Use this for initialization
	void Start () {
		//trackedObjects = (TrackedObjects) GameObject.Find("Trackables").GetComponent("TrackedObjects");
		kFilteringFactor = 0.1f; // determine how smooth the transitions are
		
		arrowShapeTransform = transform.Find("Shaft");
		coneTransform = transform.Find("Cone");
		
		
		
	}
	
	
	// Update is called once per frame
	void FixedUpdate () {
		
		
		
		// set the position of the parent object
		//transform.position = trackedObjects.trackable1.position;
		
		//transform.localScale = new Vector3(transform.localScale.x, trackedObjects.trackable1.position.x, transform.localScale.z );

		// figure out the instant velocity
		//instantVelocity =  (previousPosition - transform.position) / Time.deltaTime;
		
		currentTime += Time.deltaTime;
		
		if(currentTime >= timeIncrement){
			
			//currentTrackablePosition = new Vector3(transform.position.x, 0.5f, transform.position.z); // update the position for computing the velocity
			
			currentTrackablePosition = transform.position;
			
			if(currentTime == 0.0f)
				currentTime = 0.04f;
		
			instantVelocity = (currentTrackablePosition - previousPosition) / currentTime;
			
			previousPosition = currentTrackablePosition; // keep track of this updated position
	
			// filter so we only get gravity, not instantaneous shakes of the device
			windowedVelocity.x = (instantVelocity.x * kFilteringFactor) + (windowedVelocity.x * (1.0f - kFilteringFactor));
			windowedVelocity.y = (instantVelocity.y * kFilteringFactor) + (windowedVelocity.y * (1.0f - kFilteringFactor));
			windowedVelocity.z = (instantVelocity.z * kFilteringFactor) + (windowedVelocity.z * (1.0f - kFilteringFactor));
		
			
			//workingVelocity = windowedVelocity;
		
			workingVelocity = instantVelocity;
			
			/*
			if(Mathf.Abs(workingVelocity.x) < 0.002f)
				workingVelocity.x = 0.0f;
			if(Mathf.Abs(workingVelocity.y) < 0.002f)
				workingVelocity.y = 0.0f;
			if(Mathf.Abs(workingVelocity.z) < 0.002f)
				workingVelocity.z = 0.0f;
			*/
			
			
			RotateLookAt();
		
		
			//speed = Mathf.Sqrt(workingVelocity.magnitude) * arrowLengthScalar;
			//transform.position = currentTrackablePosition;
			//speed = ((Mathf.Sqrt(workingVelocity.magnitude)) * kFilteringFactor) + (speed * (1.0f - kFilteringFactor));
			
			//speed = (Mathf.Sqrt(workingVelocity.magnitude));
			//speed = workingVelocity.magnitude * 400.0f;
			
			speed = (Mathf.Sqrt(workingVelocity.magnitude));
			
			Debug.Log ("Time Interval = " + currentTime + ", Position = " + (currentTrackablePosition.ToString("F4")) + ", Velocity = " + (workingVelocity.ToString("F4")) + ", Speed = " + speed);
		
			//
			
			//arrowShapeTransform.localScale = workingVelocity;
			arrowShapeTransform.localScale = new Vector3(arrowShapeTransform.localScale.x, speed, arrowShapeTransform.localScale.z); // set how long the shaft portion is
			
			//arrowShapeTransform.localScale = new Vector3(arrowShapeTransform.localScale.x, workingVelocity.magnitude * arrowLengthScalar, arrowShapeTransform.localScale.z); // set how long the shaft portion is
			arrowShapeTransform.localPosition = new Vector3(0.0f, arrowShapeTransform.localScale.y, 0.0f); // move the shaft up so that it seems to extend from the base
		
			coneTransform.localPosition = new Vector3(0.0f, (arrowShapeTransform.localScale.y * 2.0f) + (coneTransform.localScale.y * 0.5f), 0.0f);
		
			currentTime = 0.0f;
		} // ends the check on the velocity
	}
	
	void RotateLookAt(){
		
		//var targetPoint = ray.GetPoint(hitdist);
		Vector3 targetPoint = transform.position - (workingVelocity);
		//targetPoint.y += 60.0f;
		
		        
        // Determine the target rotation.  This is the rotation if the transform looks at the target point.
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
		//Quaternion targetRotation = new Quaternion();
        
        // Smoothly rotate towards the target point.
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);

	    transform.rotation = targetRotation;
        
        float upDownRotation = -targetRotation.eulerAngles.x;
        
		//upDownRotation = 0.0f;
        //upDownRotation -= 90.0f;
        
        //Debug.Log ("Up Down Rotation = " + upDownRotation);
        //if(upDownRotation < 180.0f)
        
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + 90.0f, transform.localEulerAngles.y - 90.0f, transform.localEulerAngles.z + 90.0f + upDownRotation);
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 90.0f, transform.localEulerAngles.z + 90.0f + upDownRotation);
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 0.0f, transform.localEulerAngles.z + 0.0f + upDownRotation);
        
		
		//Debug.Log ("Velocity = " + (workingVelocity.ToString("F2")) + " Target = " + targetPoint.ToString("F2") + " Rotation = " + transform.localEulerAngles.ToString("F2"));
		
		
		
	}
	
	public void setPosition(Vector3 newPosition){
		transform.position = newPosition;
	}
	
	void handleTrackedObjectData(TrackedObject [] trackedObjectArray){
		
			
		
		foreach(TrackedObject trackedObject in trackedObjectArray){ // go through each of the tracked objects
			
			if(trackedObject.id == myTrackedObjectID){ // make sure that it's a match to the id that I want
				
				//currentTrackablePosition = trackedObject.position;
				//currentTrackablePosition = new Vector3(currentTrackablePosition.x + 0.001f, currentTrackablePosition.y, currentTrackablePosition.z);
		
				//currentTrackablePosition = new Vector3(currentTrackablePosition.x + 0.01f, currentTrackablePosition.y, currentTrackablePosition.z);
				
				transform.position = trackedObject.position; // set the position the same as the tracked object
			}
		}
		
	}
	
	void handlePhysicalDimensions(Vector3 dimensions){
		// scale this game object by the same as the space dimensions
		transform.localScale = Vector3.Scale(new Vector3(dimensions.x, dimensions.x, dimensions.x), transform.localScale); // multiply each component by the other			
	}
	
	
	
} // class VelocityArrowScript
