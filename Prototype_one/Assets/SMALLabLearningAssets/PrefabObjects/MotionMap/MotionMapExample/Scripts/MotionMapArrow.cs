//This script is set up specfically for artwork oriented along the Y axis. See the ArrowMarker prefabs in the examples folder. 
//Requires objects named Sphere, Shaft and Cone (case sensitive).
using UnityEngine;
using System.Collections;

public class MotionMapArrow : MonoBehaviour {

	public float arrowLengthScalar = 0.5f;//use 0.5 for 1:1 scaling
	public float offsetFromAxis = 0.03f;//where the arrows are drawn relative to the Map's axis position

	private Transform playerPos;//tracks the player's position
	private Transform myTransform;
	private bool isActive;//is this the active marker?
	private Vector3 previousPosition;//tracked to compare against the player's position
	private Vector3 targetLength;//the distance between marker position and player position
	private Transform arrowShapeTransform;//the shaft of the arrow (art must be named "Shaft")
	private Transform coneTransform;//the cone of the arrow (art must be named "Cone")
	private Transform sphere;//only need this for handling the renderer
	private int myAxis;//used to determine orientation for offsetting from the grid centerline
	private float coneScalar;

	#region Update
	void Update ()
	{
		if (isActive)
		{
			targetLength = transform.InverseTransformPoint(playerPos.position);//returns the distance from the player
			targetLength *= arrowLengthScalar;//adjust to the desired time scale
			
			ConfigureArrowFromVelocity();
			
		}
	}
	#endregion
	
	void ConfigureArrowFromVelocity(){
					if(previousPosition != playerPos.position)
			{
				arrowShapeTransform.localPosition = new Vector3(0,0,0);
				switch (myAxis)//which way is the grid aligned? 
				{
					case 0 : //grid is X Axis
						if (targetLength.y > 7.152565E-08)//7.152565E-08 to work around floating point problems, > zero.
							{
								coneTransform.localEulerAngles = new Vector3(270, 180, 0);//orient the cone
								coneScalar = 0.5f;
								transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, playerPos.localPosition.z - offsetFromAxis);//position to the positive side of the grid centerline 
							}
							else if (targetLength.y < 7.152565E-08)//if < 0 reverse the orientation and positioning
							{
								coneTransform.localEulerAngles = new Vector3(90, 0, 0);
								coneScalar = -0.5f;
								transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, playerPos.localPosition.z + offsetFromAxis);
							}
							break;
					case 1 : //grid is Y or Z axes (they work the same)
						if (targetLength.y > 7.152565E-08)
							{
								coneTransform.localEulerAngles = new Vector3(270, 180, 0);
								coneScalar = 0.5f;
								transform.localPosition = new Vector3(playerPos.localPosition.x + offsetFromAxis, transform.localPosition.y, transform.localPosition.z);
							}
							else if (targetLength.y < 7.152565E-08)
							{
								coneTransform.localEulerAngles = new Vector3(90, 0, 0);
								coneScalar = -0.5f;
								transform.localPosition = new Vector3(playerPos.localPosition.x - offsetFromAxis, transform.localPosition.y, transform.localPosition.z);
							}
							break;
				}

				arrowShapeTransform.localScale = new Vector3(arrowShapeTransform.localScale.x, targetLength.y, arrowShapeTransform.localScale.z); //scale the shaft portion 
				arrowShapeTransform.localPosition = new Vector3(0.0f, arrowShapeTransform.localScale.y, 0.0f); //move the shaft up so that it seems to extend from the base (specific for Y aligned art)
				
				
				// uncommented this if you want to disable rendering arrows while the player is moving but only once a leg is completed
				//arrowShapeTransform.renderer.enabled = false;
				//coneTransform.renderer.enabled = false;
				
				previousPosition = playerPos.position; // keep track of this updated position
			}
	
			coneTransform.localPosition = new Vector3(0.0f, (arrowShapeTransform.localScale.y * 2.0f) + (coneTransform.localScale.y * coneScalar), 0.0f);
	} // ends ConfigureArrowFromVelocity
	
	#region Scale the object to the physical dimensions. Can't do this through the broadcast because the object doesn't exist then.
	void ScaleToScene(Transform newPosition)
	{
		//the following is specific to this object. normally would be in Start() but the object didn't exist then.
		playerPos = newPosition;
		myTransform = new GameObject().transform;
		myTransform.position = new Vector3(playerPos.position.x, playerPos.position.y, playerPos.position.z);
		myTransform.forward = new Vector3(playerPos.forward.x, playerPos.forward.y, playerPos.forward.z); 
		myTransform.localScale = new Vector3(playerPos.localScale.x, playerPos.localScale.y, playerPos.localScale.z);
		transform.up = myTransform.forward;//current marker art setup is aligned on Y instead of Z
		isActive = true;
		arrowShapeTransform = transform.Find("Shaft");
		coneTransform = transform.Find("Cone");
		sphere = transform.Find("Sphere");
		sphere.GetComponent<Renderer>().enabled = false;//workaround for rendering glitch
		arrowShapeTransform.GetComponent<Renderer>().enabled = false;//workaround for rendering glitch
		coneTransform.GetComponent<Renderer>().enabled = false;//workaround for rendering glitch

		//The following is required for scaling to the scene since we missed the broadcast
		// make this scale to the dimensions of the containing interactive space 
		Vector3 spaceDimensions = ((Dimensions)GameObject.Find("InteractiveSpaceDimensions").GetComponent("Dimensions")).dimensions;
		transform.localScale = Vector3.Scale(new Vector3(spaceDimensions.x, spaceDimensions.x, spaceDimensions.x), transform.localScale); // multiply each component by the other
		Destroy(myTransform.gameObject);
		//end required

		//StartCoroutine("WaitAFrame");//workaround for the arrow rendering one frame before repositioning. 
		sphere.GetComponent<Renderer>().enabled = true;
		arrowShapeTransform.GetComponent<Renderer>().enabled = true; //uncomment if you want to show while player is moving
		coneTransform.GetComponent<Renderer>().enabled = true; //uncomment if you want to show while player is moving
		
		
		
				
	}
	#endregion
	
	#region Activate: make the object the active marker and tell it which way to align
	void Activate(int axis)
	{
		myAxis = axis;
		isActive = true;
	}
	#endregion
	
	#region Deactivate
	void Deactivate()
	{
		isActive = false;
//		sphere.renderer.enabled = true; //don't need to turn these on if they're already turned on in WaitAFrame()
		
		arrowShapeTransform.GetComponent<Renderer>().enabled = true;
		coneTransform.GetComponent<Renderer>().enabled = true;		
		
		//Debug.Log("Length: " + targetLength.y);
		
		if(Mathf.Abs(targetLength.y) < 0.0001f){ // only render the full arrow if it's long enough - otherwise we just show the sphere
			arrowShapeTransform.GetComponent<Renderer>().enabled = false;
			coneTransform.GetComponent<Renderer>().enabled = false;
		}else{
			arrowShapeTransform.GetComponent<Renderer>().enabled = true;
			coneTransform.GetComponent<Renderer>().enabled = true;
		}
		
		
	}
	#endregion

	#region WaitAFrame waits a frame before turning arrow renderers on
	/*
	IEnumerator WaitAFrame()
	{
		yield return 1;
		sphere.renderer.enabled = true;
		arrowShapeTransform.renderer.enabled = true; //uncomment if you want to show while player is moving
		coneTransform.renderer.enabled = true; //uncomment if you want to show while player is moving
	}
	*/
	#endregion
	
	
	// tell it whether to start out pointing in the negative or positive direction
	public void SetInitialVelocityDirection(int direction){
		
		float initialVelocityScalar = 0.0000001f;
				
		if(direction < 0){
			targetLength.y = -initialVelocityScalar;
		}
		else if(direction > 0){
			targetLength.y = initialVelocityScalar;
		}
		else
			targetLength.y = 0.0f;
		
		// update it immediately to avoid a flash
		ConfigureArrowFromVelocity();
	}
	
	/*
	public int GetCurrentVelocityDirection(){
		if(targetLength.y > 0.0f)
			return 1;
		else if(targetLength.y < 0.0f)
			return -1;
		else
			return 0;
	}
	*/
}