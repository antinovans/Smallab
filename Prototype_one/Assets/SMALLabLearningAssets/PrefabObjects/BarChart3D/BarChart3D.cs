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



//---------------------------------------------------------------------------------------------------------------------
// Custom3DBarGraph.cs
//---------------------------------------------------------------------------------------------------------------------
// This class creates controls the display of a custom 3D model bar graph composed of an inner and outer
// graph model shell as well as a 3D model for user target indication represented by a plane and 2 arrows 
// at each side.  The value streamed into this script will control the scale of the inner model as well as the
// position of the target model.
//---------------------------------------------------------------------------------------------------------------------
// Author : Jeff Paxson
// Date : 6/20/2011
//---------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class BarChart3D : MonoBehaviour {
	
	// User Set min and max values for the graph
	public float graphMaxValue = 10.0f;
	public float graphMinValue = 0.0f;
	public float graphValue = 0.0f;
	public float graphTargetValue = 75.0f;
	public float graphTargetProximity = 10.0f;
	
	// Text color and billboard controls
	public Color textColor;
	public bool textShouldBillboard = true;
	
	// 3D Graph Obj Color and Visibility controls
	private Color originalColor;
	private Color blinkColor;
	public Color closeToTargetColor;
	public Color aboveTargetColor;
	public float blinkDuration = 0.5f;
	private bool isBlinking = false;
	
	// Handles to 3D Graph Object Components
	private GameObject targetObj;
	private GameObject outerGraphObj;
	private GameObject innerGraphObj;
	private GameObject textObj;
	private MeshRenderer colorableMeshRenderer;
	
	// Use this for initialization
	void Start () 
	{
		// Get Handles to our 3D Graph Components
		targetObj = GameObject.Find(gameObject.name + "/BarGraphMarkers");
		outerGraphObj = GameObject.Find(gameObject.name + "/BorderBox");
		innerGraphObj = GameObject.Find(gameObject.name + "/BoxInner");
		textObj = GameObject.Find(gameObject.name + "/GraphValueText");
		
		// Set The Text Color
		SetTextColor(textColor);
		
		// Set default graph values and target values
		SetGraphValue(graphValue);
		SetGraphTargetValue(graphTargetValue);
		
		// See if user wants billboarding text, and if not remove the billboard script component
		if(!textShouldBillboard)
		{
			Destroy(textObj.GetComponent<ObjectBillboard>());
		}
		
		// Get a handle to the MeshRenderer of the object peice that we want to blink.
		colorableMeshRenderer = targetObj.GetComponentInChildren<MeshRenderer>();
		
		// Get it's Original Color so we can put it back to normal when we want.
		originalColor = colorableMeshRenderer.material.color;
		
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		// See if we should be blinking and if so do we have a valid MeshRenderer
		// to do the actual blinking.
		if(isBlinking && colorableMeshRenderer)
		{
			float lerp = Mathf.PingPong(Time.time, blinkDuration) / blinkDuration;
			Color newColor = Color.Lerp(originalColor, blinkColor, lerp);
			colorableMeshRenderer.material.SetColor("_Color", newColor);
			colorableMeshRenderer.material.SetColor("_Emission", newColor);
		}
	}
	
	//---------------------------------------------------------------------------
	// Color Controls For 3D Components
	//---------------------------------------------------------------------------
	public void SetCloseToTargetColor(Color newColor)
	{
		closeToTargetColor = newColor;
	}
	
	public Color GetCloseToTargetColor()
	{
		return closeToTargetColor;
	}
	
	public void SetAboveTargetColor(Color newColor)
	{
		aboveTargetColor = newColor;
	}
	
	public Color GetAboveTargetColor()
	{
		return aboveTargetColor;
	}
	
	//---------------------------------------------------------------------------
	// Blink Controls
	//---------------------------------------------------------------------------
	private void StartBlink(Color bColor)
	{
		// Set our blinking flag and color
		blinkColor = bColor;
		isBlinking = true;
	}
	
	private void StopBlink()
	{
		// Stop Blinking
		isBlinking = false;
		
		// Reset colorableMeshRenderer's color back to normal
		if(colorableMeshRenderer)
		{
			colorableMeshRenderer.material.SetColor("_Color", originalColor);
			colorableMeshRenderer.material.SetColor("_Emission", originalColor);
		}
	}
	
	// Set the blinkDuration to caller provided > 0 value;
	public void SetBlinkDuration( float blinkTime )
	{
		// Make sure we got a non-zero or less blink time
		if(blinkTime > 0.0f)
		{
			blinkDuration = blinkTime;
		}
	}
	
	// Return to caller the current blinkDuration
	public float GetBlinkDuration()
	{
		return blinkDuration;
	}
	
	//---------------------------------------------------------------------------
	// Manipulation of The 3D Text Components
	//---------------------------------------------------------------------------
	// Set the string to display
	private void SetText( string newText )
	{
		textObj.GetComponent<TextMesh>().text = newText;
	}
	
	// Set the text color to caller provided value
	public void SetTextColor( Color newColor )
	{
		textObj.GetComponent<Renderer>().material.color = newColor;
	}
	
	// Return to caller the current text color.
	public Color GetTextColor ()
	{
		return textObj.GetComponent<Renderer>().material.color;
	}
	
	//---------------------------------------------------------------------------
	// Graph Value Manipulation
	//---------------------------------------------------------------------------
	// Set graph value to caller specified value
	public void SetGraphValue ( float value )
	{
		// Clamp our input value to our min and max allowables and set the new value.
		value = Mathf.Clamp(value, graphMinValue, graphMaxValue);
		graphValue = value;
		
		// Set display text to input value
		SetText(graphValue.ToString());
		
		// Set the Scale of our innerGraphObj
		float newScale = (graphValue - graphMinValue)/(graphMaxValue - graphMinValue);
		innerGraphObj.transform.localScale = new Vector3(innerGraphObj.transform.localScale.x, newScale, innerGraphObj.transform.localScale.z);
		
		// Test proximity value
		float testVal = graphTargetValue - graphTargetProximity;
		if(graphValue > graphTargetValue)
		{
			StartBlink(aboveTargetColor);
		}
		else if(graphValue < testVal)
		{
			StopBlink();
		}
		else
		{
			StartBlink(closeToTargetColor);
		}
	}
	
	// Return to caller the current graphValue
	public float GetGraphValue ()
	{
		return graphValue;
	}
	
	public void SetGraphTargetValue( float value )
	{
		// Clamp our input value to our min and max allowables and set the new value.
		value = Mathf.Clamp(value, graphMinValue, graphMaxValue);
		graphTargetValue = value;
		
		// Do calculations to get our Y scale for inner indicator box
		// Some hard coded values for the default graph height, etc.  I
		// didn't want to put a bunch of work here handling all possible scales
		// more gracefully when the default graph size seems arbitrary.
		// The following works well but will allow the user to scrunch the
		// graph in odd ways.
		/*
		float graphBoxHt = 7.3f  * transform.localScale.y; // 7.3 is the default scale of the model
		float graphWidth = 3.0f  * transform.localScale.x;
		float graphDepth = 3.0f  * transform.localScale.z;
		float textHeight = 7.5f  * transform.localScale.y;
		
		float graphUnitSpan = graphMaxValue - graphMinValue;
		float graphUnit =  graphBoxHt / graphUnitSpan;
		float unitDiff = graphTargetValue - graphMinValue;
		float heightDiff = (unitDiff * graphUnit);
		*/
		// 7.3 is the height of the model
		float heightDiff = 7.3f * ((graphTargetValue - graphMinValue)/(graphMaxValue - graphMinValue));
		
		//Debug.Log("Height Difference in percentage = " + heightDiff/7.3f);
		
		// Set the vertical position of the target.
		//targetObj.transform.localPosition = new Vector3(transform.localPosition.x, heightDiff, transform.localPosition.z);
		targetObj.transform.localPosition = new Vector3(transform.localPosition.x, heightDiff, 0.0f);
	
	}
	
	// Set graph values to caller provided ones.  returns true on success, false on failure
	public bool SetGraphValues( float min, float max, float target, float proximity, float value )
	{
		// Make sure target lies between min and max
		if(target >= min && target <= max && value >= min && value <= max)
		{
			// Make sure proximity is >= 0
			if(proximity >= 0)
			{
				graphValue = value;
				graphMinValue = min;
				graphMaxValue = max;
				graphTargetValue = target;
				graphTargetProximity = proximity;
				
				// Apply changes to object elements.
				SetGraphValue(graphValue);
				SetGraphTargetValue(graphTargetValue);
				return true;
			}
			else
				return false;
		}
		else
			return false;
	}
	
	// Return to caller the graphTargetValue
	public float GetGraphTargetValue()
	{
		return graphTargetValue;
	}
	
	// Return to caller the current graphMaxValue
	public float GetGraphMaxValue()
	{
		return graphMaxValue;
	}
	
	// Return to caller the current graphMinValue
	public float GetGraphMinValue()
	{
		return graphMinValue;
	}
	
	// Return to caller the current graphTargetProximity value
	public float GetGraphTargetProximity()
	{
		return graphTargetProximity;
	}
	
	//---------------------------------------------------------------------------
	// Position and Rotation Manipulation.
	//---------------------------------------------------------------------------
	// Set the Graph's position to the caller provided float values
	public void SetGraphPosition( float xPos, float yPos, float zPos )
	{
		Vector3 newPosition = new Vector3(xPos, yPos, zPos);
		transform.position = newPosition;
	}
	
	// Set the Graph's position to caller provided Vector3
	public void SetGraphPosition(Vector3 newPos)
	{
		transform.position = newPos;
	}
	
	// Return to caller the Graph's current position
	public Vector3 GetGraphPosition()
	{
		return transform.position;
	}
	
	// Set the Graph's rotation to the caller provided float Euler Angle Values
	public void SetGraphRotation( float xRot, float yRot, float zRot )
	{
		Vector3 newRotations = new Vector3(xRot, yRot, zRot);
		transform.eulerAngles = newRotations;
	}
	
	// Set the Graph's position to caller provided Vector3 comprised of Euler Angle values.
	public void SetGraphRotation( Vector3 newRots )
	{
		transform.eulerAngles = newRots;
	}
	
	// Return to caller the Graph's current rotation in Euler Angles
	public Vector3 GetGraphRotation()
	{
		return transform.eulerAngles;
	}
	
	void handlePhysicalDimensions(Vector3 dimensions)
	{
		// scale this game object by the same as the space dimensions
		//transform.localScale = Vector3.Scale(dimensions, transform.localScale); // multiply each component by the other			
		transform.localScale = Vector3.Scale(new Vector3(dimensions.x, dimensions.x, dimensions.x), transform.localScale); // multiply each component by the other			

		transform.position = Vector3.Scale(dimensions, transform.position); // multiply each component by the other	
	}
}
