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
using System.Collections;

public enum TestShape {
	Torus = 0,
	Helix
}

public class SmallabLineChart3DTest : MonoBehaviour {

	public bool EnableStressTest = false;
	public int TrackedObjectId = 0;
	public TestShape TestShape = TestShape.Helix;
	
	private GameObject _go;
	private SmallabLineChart3D _lineChart3D;

	private float _angle;
	private int _helixIterations;
	private float _radius;
	private bool _stop;
	private float _torusHeight;
	private TrackedObject[] _trackedObjects;
	
	void OnGUI()
	{
		float xPos = Screen.width * 0.5f  - 40f;
		float yPos = Screen.height * 0.75f - 10f;
		// Display a button that toggles the text, Pause/Resume
		if (_lineChart3D != null)
		{
			string text = "Pause";
			if (_lineChart3D.Paused) text = "Resume";
			
			if (GUI.Button(new Rect(xPos, yPos, 80, 20), text))
			{
				_lineChart3D.Pause(!_lineChart3D.Paused);
			}
			
			yPos += 30;
			if (GUI.Button(new Rect(xPos, yPos, 80, 20), "Reset"))
			{
				_lineChart3D.Reset();
			}
			
			yPos += 30;
			text = "Show";
			if (_lineChart3D.Visible) text = "Hide";
			
			if (GUI.Button(new Rect(xPos, yPos, 80, 20), text))
			{
				_lineChart3D.DisplayChart(!_lineChart3D.Visible);
			}
		}
	}

	// Use this for initialization
	void Start () {
	
		// Find the LineChart3D game object
		_go = GameObject.Find("LineChart3D");
		if (_go != null)
		{
			// Get our 3D chart component
			_lineChart3D = (SmallabLineChart3D)_go.GetComponent("SmallabLineChart3D");
			if (_lineChart3D != null)
			{
				if (EnableStressTest)
				{
					_angle = -1;
					_helixIterations = -1;
					_radius = 0.5f;
					_torusHeight = 0;
					_trackedObjects = new TrackedObject[1];
					// Initialize _trackedObjects. Our transform will do.
					_trackedObjects[0] = new TrackedObject();
					_trackedObjects[0].id = TrackedObjectId;
					_trackedObjects[0].position = transform.position;
					_trackedObjects[0].rotation = transform.rotation;						
				}
				Debug.Log("LineChart3D found!");
			}
		}
		// Turn off the stop flag
		_stop = false;
	}
	
	void Update() {
		
		if (EnableStressTest && !_stop && _lineChart3D != null)
		{
			if (TestShape == TestShape.Torus)
				TorusLogic();
			else
				HelixLogic();
		}
	}
	
	private void HelixLogic()
	{
		// This test will create a helix shape that rises on the y-axis every revolution
		// 
		_angle += 1.0f;
		if (_angle >= 360)
			_angle = 0;
		_torusHeight = _torusHeight + 0.005f;
		if (_torusHeight > _lineChart3D.Dimensions.y)
		{
			_torusHeight = 0;
			_helixIterations++;
			if (_helixIterations > 2)
			{
				_helixIterations = -1;
				_radius += 0.5f;
				if (_radius >= _lineChart3D.Dimensions.x)
				{
					_radius = 0.5f;
					_stop = true;
					Debug.Log("Done");
				}
			}
		}
		float rdn = _angle  * Mathf.Deg2Rad;
		// Just update the position
		_trackedObjects[0].position = new Vector3(_radius * Mathf.Cos(rdn), _torusHeight, _radius * Mathf.Sin(rdn));
		_lineChart3D.handleTrackedObjectData(_trackedObjects);
	}
	
	private void TorusLogic()
	{
		// This test will create a torus shape that rises on the y-axis every revolution
		// 
		_angle += 1.0f;
		if (_angle >= 360)
		{
			_angle = 0;
			_torusHeight += 0.5f;
			if (_torusHeight >= _lineChart3D.Dimensions.y)
			{
				_torusHeight = 0;
				_radius += 0.5f;
				if (_radius >= _lineChart3D.Dimensions.x)
				{
					_radius = 0.5f;
					_stop = true;
					Debug.Log("Done");
				}
			}
		}
		float rdn = _angle  * Mathf.Deg2Rad;
		// Just update the position
		_trackedObjects[0].position = new Vector3(_radius * Mathf.Cos(rdn), _torusHeight, _radius * Mathf.Sin(rdn));
		_lineChart3D.handleTrackedObjectData(_trackedObjects);
	}
}
