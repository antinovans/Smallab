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
using System;
using System.Collections.Generic;

public class SmallabLineChart3D : MonoBehaviour {

	#region Properties
	// Private properties
	private Dictionary<SmallabLine3D, TrackedObject> _trackedObjects;
	private bool _pause = false;
	private float _startTime = 0;
	private bool _visible = true;
	
	// Public properties
	public SmallabLine3D[] Lines;
	public float TimerTime = 1.0f;
	
	private Vector3 _dimensions;
	public Vector3 Dimensions
	{
		get { return _dimensions; }
	}
	public bool Paused
	{
		get { return _pause; }
	}
	public bool Visible
	{
		get { return _visible; }
	}
	#endregion

	#region EventHandlers
	void Start()
	{
		_startTime = Time.time;
		_trackedObjects = new Dictionary<SmallabLine3D, TrackedObject>();		
	}
	
	void Update()
	{
		if ((Time.time - _startTime) >= TimerTime)
		{
			// Loop through all the lines in this chart and plot the points
			foreach(SmallabLine3D line in Lines)
			{
				// Make sure we have an entry for this line
				if (_trackedObjects.ContainsKey(line))
				{
					PlotCurrentPosition(line, _trackedObjects[line].position);
				}
			}
			_startTime = Time.time;
		}
	}
	#endregion
	
	#region Public Methods
	// DisplayChart	- Shows or hides the chart
	//
	// On Entry:
	//		show	- if true, show the chart, else hide the chart
	//
	public void DisplayChart(bool show)
	{
		// Loop through all the lines in this chart and tell them to show/hide points
		foreach(SmallabLine3D line in Lines)
		{		
			line.DisplayLine(show);
		}
		_visible = show;
	}
		
	// handlePhysicalDimensions	- this method is broadcast by script, InteractiveSpaceDimensions
	//								- the dimensions will be used to scale 3D lines.
	// On Entry:
	//		dimensions	- the 3D space dimensions in engineering units 
	//
	public void handlePhysicalDimensions(Vector3 dimensions)
	{
		// Cache our dimensions.
		_dimensions = dimensions;
		// Loop through all the lines in this chart and set their local scales
		foreach(SmallabLine3D line in Lines)
			line.Dimensions = dimensions;
	}
	
	// handleTrackedObjectData	- this method is broadcast by script, Trackables, which sends
	//
	// On Entry:
	//		trackedObjects	- an array of TrackedObject objects.
	//
	public void handleTrackedObjectData(TrackedObject[] trackedObjects)
	{
		foreach(TrackedObject trackedObject in trackedObjects)
		{
			foreach(SmallabLine3D line in Lines)
			{
				if (line.TrackedObjectID == trackedObject.id)
				{
					if (!_trackedObjects.ContainsKey(line))
						_trackedObjects.Add(line, trackedObject);
					else
						_trackedObjects[line] = trackedObject;
				}
			}
		}
		
	}
	
	public void OnPause(){
		Pause(true);
	}
	public void OnPlay(){
		Pause(false);
	}
	public void OnReset(){
		Reset();	
	}
	
	// Pause	- Stops or resume point collection
	//
	// On Entry:
	//		pause	- if true, stop collecting points, else resume collecting points
	//
	public void Pause(bool pause)
	{
		// Loop through all the lines in this chart and tell them to show/hide points
		foreach(SmallabLine3D line in Lines)
		{		
			line.Pause(pause);
		}
		_pause = pause;
	}
		
	// PlotCurrentPosition
	//
	// On Entry:
	//		line	- the line to which a new value will be added
	//		value	- a Vector3 structure that specifies the position in 3D space
	//				  (these are floats)
	//
	public void PlotCurrentPosition(SmallabLine3D line, Vector3 value)
	{
		if (line != null && _pause == false)
		{
			// Append it to the line
			line.PlotCurrentPosition(gameObject.transform, value);
		}
	}	
	
	public void Reset()
	{
		SmallabLine3D line;

		for(int i=0; i < Lines.Length; i++)
		{
			line = Lines[i];
				line.Reset();
		}
	}
	#endregion
}
