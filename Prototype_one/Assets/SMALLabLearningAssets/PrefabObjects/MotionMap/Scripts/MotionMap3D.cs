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
using System;

public class MotionMap3D : MonoBehaviour
{	
	#region Public Properties
	public enum axis {XAxis = 0, YAxis = 1, ZAxis = 2};//constratin movement to a given axis
	public axis axisConstraint;//expose it in the Inspector
	public enum align {Left = 0, Center = 1, Right = 2};//constratin movement to a given axis
	public align gridAlignment;//expose it in the Inspector
	public float gridOffset = 0.25f;//distance from 0,0,0 for the main axis
	public Transform myTrackedObject;//the player object
	public GameObject mapAxis;//the grid transform is currently used for a work around
	public GameObject gridLine;//the grid transform is currently used for a work around
	public int gridInterval;//number of gridlines
	public GameObject markerGraphic;//the marker game object
	public bool testing = false;//if this is checked markers will be spawned at the testingMarkerInterval
								//if testing is not checked, and outside call to the method PlotCurrentPoint() must be made.
	public int testingMaxMarkers;//the maximum number of markers when testing
	public float testingMarkerInterval;//how often markers can spawn when testing.
	#endregion

	#region Private Properties
	private MotionMapPlayer mmP;//the script on the player to access the ID from
	private int trackedID;//the player's ID
	private Vector3 gridPos;//main axis position. gridOffset is added to this for grid positioning
	private GameObject currentGrid;//used for instantiating the gridLines
	private Vector3 lastGridPos;//used to position the next grid line
	private int setAxis; //used by the arrow marker to orient itself properly X = 0, Y = 1, Z = 1 (Y and Z are the same relative)
	private GameObject currentMarker;//the marker being spawned
	private float markerCountDown;//incremented and compared to testingMarkerInterval, spawns marker when greater
	private ArrayList markers;//stores all markers for traversing
	private Vector3 lastPosition;//the last player position
	private float x;//used for positioning the player while constraining to the X axis
	private float y;//used for positioning the player while constraining to the Y axis
	private float z;//used for positioning the player while constraining to the Z axis
	private bool marking;//used to limit plotting to one point per call
	private Vector3 gridScale;//used for scaling instantiated objects to the root object's local scale (not space dimensions).
	
	private int currentVelocityDirection;
	#endregion

	#region Initialization
	void Awake()
	{	
		gridScale = transform.localScale;
		switch(axisConstraint)//rotate the player in the axis constraint direction and set up the grid
		{
			case axis.XAxis : //Along the X axis
				myTrackedObject.forward = new Vector3(1,0,0);//player object rotation
				
				#region Set up the grid on the X axis
				mapAxis = Instantiate(mapAxis, new Vector3(0,0,0), Quaternion.identity) as GameObject;
				mapAxis.transform.localScale = Vector3.Scale(mapAxis.transform.localScale, gridScale);
				mapAxis.transform.forward = new Vector3(1,0,0);
				mapAxis.transform.parent = transform;
				for(int i = 0; i <= gridInterval; i++)
				{
					currentGrid = Instantiate(gridLine, mapAxis.transform.position, mapAxis.transform.rotation) as GameObject;
					currentGrid.transform.localScale = Vector3.Scale(currentGrid.transform.localScale, gridScale);
					currentGrid.transform.parent = mapAxis.transform;
					if (i == 0)
					{
						currentGrid.transform.position = new Vector3(mapAxis.transform.position.x - mapAxis.GetComponent<Renderer>().bounds.extents.x, mapAxis.transform.position.y, mapAxis.transform.position.z);
					}
					else
					{
						currentGrid.transform.position = new Vector3(lastGridPos.x + ((mapAxis.GetComponent<Renderer>().bounds.extents.x * 2)/gridInterval), lastGridPos.y, lastGridPos.z);						
					}
					lastGridPos = currentGrid.transform.position;
				}
				
				switch(gridAlignment)
				{
					case align.Left :
						gridPos = mapAxis.transform.localPosition;
						gridPos.z -= gameObject.transform.localScale.z * gridOffset;
						break;
					case align.Center :
						gridPos = mapAxis.transform.localPosition;
						break;
					case align.Right :
						gridPos = mapAxis.transform.localPosition;
						gridPos.z += gameObject.transform.localScale.z * gridOffset;
						break;
				}
				mapAxis.transform.position = gridPos;
				#endregion

				setAxis = 0;//sent to the marker object to determine orientation
				break;
				
			case axis.YAxis : //Along the Y axis
				myTrackedObject.forward = new Vector3(0,1,0);//player object rotation

				#region Set up the grid on the Y axis
				mapAxis = Instantiate(mapAxis, new Vector3(0,0,0), Quaternion.identity) as GameObject;
				mapAxis.transform.localScale = Vector3.Scale(mapAxis.transform.localScale, gridScale);
				mapAxis.transform.forward = new Vector3(0,1,0);
				mapAxis.transform.position = new Vector3(mapAxis.transform.position.x, mapAxis.transform.position.y + mapAxis.GetComponent<Renderer>().bounds.extents.y, mapAxis.transform.position.z);
				mapAxis.transform.parent = transform;
				for(int i = 0; i <= gridInterval; i++)
				{
					currentGrid = Instantiate(gridLine, mapAxis.transform.position, mapAxis.transform.rotation) as GameObject;
					currentGrid.transform.localScale = Vector3.Scale(currentGrid.transform.localScale, gridScale);
					currentGrid.transform.parent = mapAxis.transform;
					if (i == 0)
					{
						currentGrid.transform.position = new Vector3(mapAxis.transform.position.x, mapAxis.transform.position.y - mapAxis.GetComponent<Renderer>().bounds.extents.y, mapAxis.transform.position.z);
					}
					else
					{
						currentGrid.transform.position = new Vector3(lastGridPos.x, lastGridPos.y + ((mapAxis.GetComponent<Renderer>().bounds.extents.y * 2)/gridInterval), lastGridPos.z);						
					}
					lastGridPos = currentGrid.transform.position;
				}

				//Y is special case. For proper Grid Alignment, camera must be facing InteractiveSpaceGuides 
				//looking in the positive Z direction or Right/Left will be reversed.
				switch(gridAlignment)
				{
					case align.Left :
						gridPos = mapAxis.transform.localPosition;
						gridPos.x -= gameObject.transform.localScale.x * gridOffset;
						break;
					case align.Center :
						gridPos = mapAxis.transform.localPosition;
						break;
					case align.Right :
						gridPos = mapAxis.transform.localPosition;
						gridPos.x += gameObject.transform.localScale.x * gridOffset;
						break;
				}
				mapAxis.transform.position = gridPos;
				#endregion

				setAxis = 1;
				break;
				
			case axis.ZAxis : //Along the Z axis
				myTrackedObject.forward = new Vector3(0,0,1);//player object rotation

				#region Set up the grid on the Z axis
				mapAxis = Instantiate(mapAxis, new Vector3(0,0,0), Quaternion.identity) as GameObject;
				mapAxis.transform.localScale = Vector3.Scale(mapAxis.transform.localScale, gridScale);
				mapAxis.transform.forward = new Vector3(0,0,1);
				mapAxis.transform.parent = transform;
				for(int i = 0; i <= gridInterval; i++)
				{
					currentGrid = Instantiate(gridLine, mapAxis.transform.position, mapAxis.transform.rotation) as GameObject;
					currentGrid.transform.localScale = Vector3.Scale(currentGrid.transform.localScale, gridScale);
					currentGrid.transform.parent = mapAxis.transform;
					if (i == 0)
					{
						currentGrid.transform.position = new Vector3(mapAxis.transform.position.x, mapAxis.transform.position.y, mapAxis.transform.position.z - mapAxis.GetComponent<Renderer>().bounds.extents.z);
					}
					else
					{
						currentGrid.transform.position = new Vector3(lastGridPos.x, lastGridPos.y, lastGridPos.z + ((mapAxis.GetComponent<Renderer>().bounds.extents.z * 2)/gridInterval));	
					}
					lastGridPos = currentGrid.transform.position;
				}
				
				switch(gridAlignment)
				{
					case align.Left :
						gridPos = mapAxis.transform.localPosition;
						gridPos.x -= gameObject.transform.localScale.x * gridOffset;
						break;
					case align.Center :
						gridPos = mapAxis.transform.localPosition;
						break;
					case align.Right :
						gridPos = mapAxis.transform.localPosition;
						gridPos.x += gameObject.transform.localScale.x * gridOffset;
						break;
				}
				mapAxis.transform.position = gridPos;
				#endregion

				setAxis = 1;
				break;
		}		
		markers = new ArrayList();
		marking = false;//able to plot (note for testing Plot may fire earlier than desired).
		markerCountDown = 0.0f;
	}
	#endregion

	#region Start (initialize items dependent on other objects)
	void Start()
	{
		mmP = myTrackedObject.GetComponent("MotionMapPlayer") as MotionMapPlayer;
		trackedID = mmP.wandID;//store the player ID so we don't have to look it up all the time
	}
	#endregion

	#region Update, only used during testing. Per frame calls are broadcast from the Trackables object.
	void Update()
	{
		if (testing)//if testing, increment the counter
		{
			markerCountDown += Time.deltaTime;
			if (Input.GetKeyDown(KeyCode.R))//Clears the current markers if tester presses the r key
			{
				ResetMarkers();//this needs an outside call when not testing
			}

			//only plot if we've moved and the marker interval was reached
			if ((myTrackedObject.position != lastPosition) && (markerCountDown >= testingMarkerInterval))
			{
				marking = false;
			}
			else 
			{
				marking = true;//don't plot more than once
			}

			if (markers.Count < testingMaxMarkers)//if conditions are true, plot the marker
			{
				if (!marking)
				{
					PlotCurrentPoint();
					markerCountDown = 0.0f;//reset the counter
				}
			}

//			lastPosition = myTrackedObject.position;//store for comparison

		}
	}
	#endregion
	
	#region Handle the tracked object data, broadcast from the Trackables object
	void handleTrackedObjectData(TrackedObject[] trackedObjectData) // go through each of the tracked objects
	{
		lastPosition = myTrackedObject.position;//store for comparison

		foreach(TrackedObject trackedObject in trackedObjectData)
		{
			if (trackedID == trackedObject.id)//is this the correct object?
			{
				//if(!marking){ // only accept input data if we're not marking time (when we call stop plotting)
				
				switch(axisConstraint)//depending on the map's axis constraint
				{
					case axis.XAxis :
						myTrackedObject.position = new Vector3(trackedObject.position.x, lastPosition.y, lastPosition.z); //set the X position the same as the tracked object, lastPosition Y & Z should never change.
						break;
					case axis.YAxis :
						myTrackedObject.position = new Vector3(lastPosition.x, trackedObject.position.y, lastPosition.z); //set the Y position the same as the tracked object
						break;
					case axis.ZAxis :
						myTrackedObject.position = new Vector3(lastPosition.x, lastPosition.y, trackedObject.position.z); //set the Z position the same as the tracked object
						break;
				}
			//} // ends !marking
			}
		}
		
	}
	#endregion

	#region Plot the marker points
	public void PlotCurrentPoint()
	{
		if (!marking)
		{
			if (currentMarker != null)
			{
				// get the velocity of the current marker before we deactivate it
//PeteM change 07/13/11: added the line below and commented out the direct call
				currentVelocityDirection = Mathf.RoundToInt(currentMarker.transform.InverseTransformPoint(myTrackedObject.position).magnitude);
//				currentVelocityDirection = currentMarker.GetComponent<MotionMapArrow>().GetCurrentVelocityDirection();
				//Debug.Log("Current velocity direction = " + currentVelocityDirection);
//end PeteM change 07/13/11

				currentMarker.SendMessage("Deactivate", SendMessageOptions.DontRequireReceiver);//deactivate the last marker
			}
			
			//create a new marker
			currentMarker = Instantiate(markerGraphic, myTrackedObject.position, myTrackedObject.rotation) as GameObject;
			
			//scale it to the scene scale. this needs to be called because the object doesn't exist when scale is broadcast
			currentMarker.SendMessage("ScaleToScene", myTrackedObject, SendMessageOptions.DontRequireReceiver);
			
			currentMarker.transform.parent = transform;//parent it to the map object
			
			//activate the current marker
			currentMarker.SendMessage("Activate", setAxis, SendMessageOptions.DontRequireReceiver);
			
			
			// tell the new marker arrow which direction to face initially based upon the current velocity
//PeteM change 07/13/11: added the line below and commented out the direct call			
			currentMarker.SendMessage("SetInitialVelocityDirection", currentVelocityDirection, SendMessageOptions.DontRequireReceiver);
//			currentMarker.GetComponent<MotionMapArrow>().SetInitialVelocityDirection(currentVelocityDirection);
//end PeteM change 07/13/11
			
			markers.Add(currentMarker);//add it to an array of plotted markers
		}
	}
	#endregion
	
	#region StopPlotting
	public void StopPlotting()
	{
		marking = true;//override so the Plot function can't fire in case there's an errant call
		if (currentMarker != null)//if there's a current marker, tell it to deactivate
		{
			currentMarker.SendMessage("Deactivate", SendMessageOptions.DontRequireReceiver);
		}
	}
	#endregion
	
	#region Clear the markers to start new
	public void ResetMarkers()
	{
		for (int i = 0; i < markers.Count; i++)
		{
			Destroy((GameObject)markers[i]);
		}
		markers = new ArrayList();

		myTrackedObject.position = lastPosition;//reset the player to start position
		lastPosition = myTrackedObject.position;//store for Vector3.Distance comparison

		markerCountDown = 0.0f;//reset the counter
		marking = false;//able to plot (note for testing Plot may fire earlier than desired).
	}
	#endregion

	#region Scale the objects to the physical space dimensions
	void handlePhysicalDimensions(Vector3 dimensions)
	{
		// scale this game object by the same as the space dimensions
		transform.localScale = Vector3.Scale(dimensions, transform.localScale); // multiply each component by the other			

		myTrackedObject.position = mapAxis.transform.position;
		lastPosition = myTrackedObject.position;
	}
	#endregion
}