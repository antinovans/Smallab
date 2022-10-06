/*

Copyright � 2011.  Arizona Board of Regents.  All Rights Reserved
 
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

public enum SmallabLine3DPointShape {
	Sphere = 0,
	Cube
}

[Serializable]
public class SmallabLine3D {

	#region Properties
	// Private properties
	private List<GameObject> _cylinderMesh;
	private int _cylinders;
	private Vector3 _dimensions = Vector3.zero;
	private bool _enabled = true;
	private Vector3 _lastPos;
	private int _lastPointIndex = -1;
    private Vector3 _maxValues = new Vector3(10, 10, 10);
    private Vector3 _minValues = new Vector3(0, 0, 0);
	private Transform _parent;
	private bool _pause = false;
	private List<Vector3> _points;
	private List<GameObject> _pointShapeMesh;
	private int _pointShapes;
	
	// Public properties
	public GameObject CylinderPrefab;
    public Material LineMaterial;
    public float LineWidth;				// percent of dimensions
	public int MaxPoints = 0;			// if zero, allow unlimited number of points
	public Material PointMaterial;
	public SmallabLine3DPointShape PointShape = SmallabLine3DPointShape.Sphere;
	public float PointWidth;			// percent of dimensions
	public GameObject SpherePrefab;
	public int TrackedObjectID = 0;
	
	public Vector3 Dimensions
	{
		get { return _dimensions; }
		set { 
			_dimensions = value;
			_maxValues = _dimensions / 2;
			_maxValues.y = _dimensions.y;
			_minValues = -_dimensions / 2;
			_minValues.y = 0;
		}
	}	
	#endregion

	#region Public Methods
	// DisplayLine	- Shows or hides the line
	//
	// On Entry:
	//		show	- if true, show the line, else hide the line
	//				- if not paused, we are still collecting points. So when the line is displayed again,
	//				  we may need to plot the points that have been collected.
	//
	public void DisplayLine(bool show)
	{
		bool currentDisplayState = _enabled;
		MeshRenderer rend;

		// Set the new state
		_enabled = show;		

		if (_points != null && _points.Count > 0)
		{
			lock(_points)
			{
				// First, show or hide the meshes
				if (_cylinderMesh != null && _cylinderMesh.Count > 0)
				{
					foreach(GameObject go in _cylinderMesh)
					{
						rend = go.GetComponent<MeshRenderer>();
						if (rend != null)
							rend.enabled = show;
					}
				}
				if (_pointShapeMesh != null && _pointShapeMesh.Count > 0)
				{
					foreach(GameObject go in _pointShapeMesh)
					{
						rend = go.GetComponent<MeshRenderer>();
						if (rend != null)
							rend.enabled = show;
					}
				}
				
				if (currentDisplayState != show)
				{
					// On transition from show to hide, if not paused, save the lastPos index
					if (show == false)
					{
						if (!_pause) _lastPointIndex = _points.Count;
					}
					else
					{
						// On transition from hide to show, plot the new points that haven't been plotted
						// if any exist.
						if (_lastPointIndex != -1)
						{
							// If we have been hidden for a while, make sure we haven't deleted some of those meshes.
							if (MaxPoints > 0 && (_points.Count - _lastPointIndex) > MaxPoints)
							{
								_lastPointIndex = _points.Count - MaxPoints;
							}
							if (_lastPointIndex == 0)
								_lastPos = _points[_lastPointIndex];
							for(int i=_lastPointIndex; i < _points.Count; i++)
							{
								Vector3 pos = _points[i];
								// Draw the line
								DrawLine(_parent, pos);
							}
							_lastPointIndex = -1;
						}
					}
				}
			}
		}
	}
	
	// OnPause
	//
	// On Entry:
	//		nothing
	//
	public void OnPause()
	{
		Pause(true);
	}
	
	// OnPlay
	//
	// On Entry:
	//		nothing
	//
	public void OnPlay()
	{
		Pause(false);
	}
	
	// OnReset
	//
	// On Entry:
	//		nothing
	//
	public void OnReset()
	{
		Reset();
	}
	
	// Pause	
	//
	// On Entry:
	//		pause	- if true, stop collecting points
	//				- if false, resume collecting points
	//
	public void Pause(bool pause)
	{
		_pause = pause;
	}
	
	// PlotCurrentPosition
	//
	// On Entry:
	//		parent
	//		value	- a Vector3 structure that specifies the position in 3D space
	//				  (these are floats)
	//
	public void PlotCurrentPosition(Transform parent, Vector3 newPos)
	{
		// Save our parent
		_parent = parent;
		
		// Create our point list if necessary
		if (_points == null) {
			_points = new List<Vector3>();
		}
		lock(_points)
		{
			// Don't create another cylinder if we haven't moved
			if (_points.Count > 0 && _lastPos == newPos)
				return;
			
			// Clamp values
			newPos.x = Mathf.Clamp(newPos.x, _minValues.x, _maxValues.x);
			newPos.y = Mathf.Clamp(newPos.y, _minValues.y, _maxValues.y);
			newPos.z = Mathf.Clamp(newPos.z, _minValues.z, _maxValues.z);
			
			// If not paused, append new point to the list
			if (!_pause) {
				// Check Max Points to see if we need to limit the number of points
				if (MaxPoints > 0 && _points.Count >= MaxPoints)
				{
					if (_cylinderMesh != null && _cylinderMesh.Count > 0)
					{
						// Delete the first cylinder
						GameObject.Destroy(_cylinderMesh[0]);
						_cylinderMesh.RemoveAt(0);
					}
					if (_pointShapeMesh != null && _pointShapeMesh.Count > 0)
					{
						// Delete the first pointShape
						GameObject.Destroy(_pointShapeMesh[0]);
						_pointShapeMesh.RemoveAt(0);
					}
					// Don't remove the first entry or we will not be able to determine from where to hide/show points.
					//_points.RemoveAt(0);
				}
				_points.Add(newPos);		
			}
			// If the display is not enabled, or the chart is paused, return
			if (!_enabled || _pause)
				return;
			
			// Draw the line
			DrawLine(parent, newPos);			
		}
		//Debug.Log("# of Points: " + _points.Count);
	}
	
	public void Reset()
	{
		MeshFilter mf;
		
		// Clear the lists
		if (_points != null)
		{
			lock(_points)
			{
				_points.Clear();

				if (_cylinderMesh != null && _cylinderMesh.Count > 0)
				{
					foreach(GameObject go in _cylinderMesh)
					{
						mf = go.GetComponent<MeshFilter>();
						if (mf != null)
							mf.mesh.Clear();
						
						GameObject.Destroy(go);
					}
					_cylinderMesh.Clear();
				}
				_cylinderMesh = null;
				_cylinders = 0;
				
				if (_pointShapeMesh != null && _pointShapeMesh.Count > 0)
				{
					foreach(GameObject go in _pointShapeMesh)
					{
						mf = go.GetComponent<MeshFilter>();
						if (mf != null)
							mf.mesh.Clear();
						
						GameObject.Destroy(go);
					}
					_pointShapeMesh.Clear();
				}
				_pointShapeMesh = null;
				_pointShapes = 0;
				
				// If the display is not enabled, we need to reset our last point index too
				if (!_enabled && _lastPointIndex > -1)
					_lastPointIndex = 0;
			}				
		}
	}
	#endregion
	
	#region Private Methods
	private void DrawLine(Transform parent, Vector3 newPos)
	{
		if (_cylinderMesh == null) 
		{
			// Create our cylinder mesh list
			_cylinderMesh = new List<GameObject>();
		}
		// Don't draw the line if all we have is one point.
		if (_points.Count > 1)
		{
			if (CylinderPrefab != null && LineWidth > 0)
			{
				GameObject go = GameObject.Instantiate(CylinderPrefab) as GameObject;
				go.name = "SmallabObject_"+parent.name+"_Cylinders"+(_cylinders).ToString();
				go.transform.parent = parent;
				go.transform.position = _lastPos;
				go.transform.LookAt(newPos);
				if (LineMaterial != null)
					go.GetComponent<Renderer>().material = LineMaterial;				
				// Distance between two points
				float mag = (newPos - _lastPos).magnitude;
				// Set the scale (the cylinder prefab is 1m in the z direction, so subtract 1m from _dimensions.z before applying the magnitude)
				go.transform.localScale = new Vector3(_dimensions.x * LineWidth, _dimensions.y * LineWidth, (_dimensions.z - go.transform.localScale.z) * mag);
				// Increment our cylinder count
				_cylinders++;
				// Preserve our reference
				_cylinderMesh.Add(go);
			}
			DrawPoint(parent, _lastPos);
		}
		// Store the last position
		_lastPos = newPos;
	}
	
	private void DrawPoint(Transform parent, Vector3 pos)
	{
		if (PointWidth > 0)
		{
			if (_pointShapeMesh == null) 
			{
				// Create our pointShape mesh list
				_pointShapeMesh = new List<GameObject>();
			}
			
			GameObject pointShape;
			if (PointShape == SmallabLine3DPointShape.Sphere)
			{
				if (SpherePrefab == null)
					pointShape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				else
					pointShape = GameObject.Instantiate(SpherePrefab) as GameObject;
			}
			else
				pointShape = GameObject.CreatePrimitive(PrimitiveType.Cube);
			
			pointShape.name = "SmallabObject_"+parent.name+"_PointShapes"+(_pointShapes).ToString();
			pointShape.transform.parent = parent;
			pointShape.transform.position = pos;
			//
			// If we are using the cube shape, and we have one or more points,
			//
			if (PointShape == SmallabLine3DPointShape.Cube && _pointShapes > 0)
			{
				int meshIndex = _pointShapes - 1;
				// If MaxPoints is set, make sure we don't exceed the bounds on the _pointShapeMesh list
				if (MaxPoints > 0 && meshIndex >= _pointShapeMesh.Count)
				{
					meshIndex = _pointShapeMesh.Count-1;
				}
				// Create two vectors 
				// The last point's forward vector.
				Vector3 v1 = _pointShapeMesh[meshIndex].transform.forward;
				// The vector formed by this point and the last point.
				Vector3 v2 = (pos - _pointShapeMesh[meshIndex].transform.position).normalized;
				// Get the angle between two vectors
				float angle = Mathf.Acos(Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
				// Get the axis of rotation
				Vector3 axis = Vector3.Cross(v1, v2);
				// Rotate the previous point
				_pointShapeMesh[meshIndex].transform.rotation = Quaternion.AngleAxis(angle, axis);
			}
			if (PointMaterial != null)
				pointShape.GetComponent<Renderer>().material = PointMaterial;
			// Set the scale
			pointShape.transform.localScale = new Vector3(_dimensions.x * PointWidth / 2, _dimensions.x * PointWidth / 2, _dimensions.x * PointWidth / 2);
			// Increment our pointShape count
			_pointShapes++;
			// Preserve our reference
			_pointShapeMesh.Add(pointShape);
		}
	}
	#endregion
}
