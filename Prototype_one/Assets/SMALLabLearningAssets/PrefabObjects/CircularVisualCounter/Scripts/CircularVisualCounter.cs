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

public class CircularVisualCounter : MonoBehaviour
{

	#region Public properties
	public GameObject marker;//the visual object that increments/decrements with count
	public int maxCount;//the maximum number of markers there can be for this counter
	public int defaultCount;//the starting number of markers displayed
	public float markerDistance;//the distance from the image plane center (radius the markers place along).
	public Material markerMaterial;//sets the marker material
	public Material planeMaterial;//sets the image plane material
	public bool testing;//if checked, keyboard input will work: a = increment; s = decrement; r = reset
	[HideInInspector]
	public int currentCount;//the current number of markers displayed. Hidden in the inspector but accessible from script.
	#endregion

	#region Private properties	
	private Transform plane;//the image plane's transform (used for determining marker positions).
	private Vector3[] positions;//an array of positions calculated in Start()
	private GameObject[] markers;//an array of the current visible markers
	private ArrayList tempMarkers;//an array used to temporarily store markers for increment, decrement and resetting.
	private float angle;//the angle between markers based on the maxCount.
	private float startAngle;//used for determining the initial marker position
	private Vector3 dimensions;
	private Vector3 markerScale;
	#endregion

	#region Initialization
	void Start ()
	{
		tempMarkers = new ArrayList();

		if (defaultCount > maxCount)//catch if inspector value was entered higher than the maxCount
		{
			defaultCount = maxCount;
		}
		
		currentCount = defaultCount;

		angle = (float)360.0f/maxCount;//determine the angle between markers at maxCount

		//Determine the first marker position so that the default count is centered at the bottom of the circle
//		startAngle = -transform.eulerAngles.y + 270.0f - ((((float)defaultCount * 0.5f) - 0.5f) * angle);//counterclockwise set up
		startAngle = -transform.eulerAngles.y + 270.0f + ((((float)defaultCount * 0.5f) - 0.5f) * angle);//clockwise set up

		plane = this.transform.Find("Plane");//find the image plane. Must be named Plane, case sensitive.
		plane.gameObject.GetComponent<Renderer>().material = planeMaterial;//set the plane material

		//account for scene scale. 
		//Using the broadcast message means Start needs to yield a frame because scaling needs to be done before getting positions.
		//Instead of waiting for the broadcast, this just pulls it from the dimensions script.
		dimensions = ((Dimensions)GameObject.Find("InteractiveSpaceDimensions").GetComponent("Dimensions")).dimensions;
		transform.localScale = Vector3.Scale(dimensions, transform.localScale); // multiply each component by the other			
		transform.position = Vector3.Scale(dimensions, transform.position); // multiply each component by the other

		markerDistance *= transform.localScale.x;//adjust the radius distance to the scene scale.

		//cache all of the positions for the maximum number of markers. Calls GetPosition() method.
		positions = new Vector3[maxCount];
		for (int i = 0; i < positions.Length; i++)
		{
			positions[i] = GetPosition(markerDistance, startAngle);//gets this position
			//increment for the next position
//			startAngle += angle;//adds markers counterclockwise
			startAngle -= angle;//adds markers clockwise
		}
				
		//Instantiate the default number of markers and cache them in an array. 
		markers = new GameObject[defaultCount];
		for (int i = 0; i < markers.Length; i++)
		{
			markers[i] = Instantiate(marker, positions[i], Quaternion.identity) as GameObject;
			markers[i].GetComponent<Renderer>().material = markerMaterial;//set the marker material

			//parent marker to the counter object
			markers[i].transform.parent = transform;

			// account for scene scaling 
			markers[i].transform.localScale = Vector3.Scale(transform.localScale, markers[i].transform.localScale);

			//cached scale is used to account for scene scaler when increasing count or resetting
			markerScale = markers[i].transform.localScale;
			
			tempMarkers.Add(markers[i]);//add the object to the temp array for later handling
		}
	}
	#endregion
	
	
	#region Update. Used only if testing is true
	void Update ()
	{
		if (testing)
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				Reset();
			}

			if (Input.GetKeyDown(KeyCode.A))
			{
				IncreaseCount();
			}

			if (Input.GetKeyDown(KeyCode.S))
			{
				DecreaseCount();
			}
		}
	}
	#endregion

	#region Increase the count
	public void IncreaseCount()
	{
		if (currentCount < maxCount)//don't increase if maxCount is reached. 
		{
			//need to instantiate in an arraylist because markers count can't change.
			tempMarkers.Add(Instantiate(marker, positions[currentCount], Quaternion.identity) as GameObject);

			//match the markers array to the temp array as game objects
			markers = new GameObject[tempMarkers.Count];
			for (int i = 0; i < tempMarkers.Count; i++)
			{
				markers[i] = tempMarkers[i] as GameObject;
				markers[i].GetComponent<Renderer>().material = markerMaterial;//set the marker material

				//parent marker to the counter object
				markers[i].transform.parent = transform;
				
				// account for scene scaling 
				markers[i].transform.localScale = markerScale;
			}
			currentCount += 1;//increase the current count
		}
	}
	#endregion

	#region Decrease the count
	public void DecreaseCount()
	{
		if (currentCount > 0)//don't decrease if count is zero
		{
			Destroy(markers[currentCount - 1]);//destroy the last marker in the array
			tempMarkers.RemoveAt(currentCount - 1);//remove the last marker from the temp array

			//match the markers array to the temp array as game objects
			markers = new GameObject[tempMarkers.Count];
			for (int i = 0; i < tempMarkers.Count; i++)
			{
				markers[i] = tempMarkers[i] as GameObject;
			}
			currentCount -= 1;//decrease the current count
		}
	}
	#endregion

	#region Reset to the default
	public void Reset()
	{
		//clear the current markers
		for (int i = 0; i < markers.Length; i++)
		{
			Destroy(markers[i]);
		}
		tempMarkers.Clear();

		//create a new array of markers at the default count
		markers = new GameObject[defaultCount];
		for (int i = 0; i < defaultCount; i ++)
		{
			tempMarkers.Add(Instantiate(marker, positions[i], Quaternion.identity) as GameObject);
			markers[i] = tempMarkers[i] as GameObject;
			markers[i].GetComponent<Renderer>().material = markerMaterial;//set the marker material
			
			//parent marker to the counter object
			markers[i].transform.parent = transform;
				
			// account for scene scaling 
			markers[i].transform.localScale = markerScale;
		}
		currentCount = defaultCount;//reset the current count
	}
	#endregion

	#region Get the current count. Can be called externally. Equivalent to calling CircularVisualCounter.currentCount.
	public int GetCurrentCount()
	{
		return currentCount;
	}
	#endregion
	
	#region GetPosition returns a Vector3 position along the circumferance of a circle
	//markerDistance is passed as the radius
	//startAngle is passed as the angle
	public Vector3 GetPosition(float myRadius, float myAngle)
	{
		// Convert from degrees to radians via multiplication by PI/180 for Cos and Sin
		float x = (myRadius * Mathf.Cos(myAngle * Mathf.Deg2Rad)) + plane.position.x;//relative to the plane's center
		float z = (myRadius * Mathf.Sin(myAngle * Mathf.Deg2Rad)) + plane.position.z;
		return new Vector3 (x, plane.position.y, z);//align with plane's Y position
    }
    #endregion
}