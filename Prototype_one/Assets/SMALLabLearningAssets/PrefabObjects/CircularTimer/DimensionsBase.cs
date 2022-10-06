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

// Module:  DimensionsBase.cs
// Author:  Rob Galante
// Date:    June 15, 2011
//
// ****************************************************************************
// This module handles some common positioning and scaling in case the scene dimensions have changed.
// When scene dimensions change, the position and scale of the object to which a script that inherits
// this base class is attached, need to be changed to reflect the change in the position of the camera.
//
// IMPORTANT NOTE:
//	If you change the script, CameraPositionFromSpaceDim, you need to change this script as well.
//	This script assumes that the camera will be 4 x dimensions.x. If you change the factor from 4
//	to something else, you need to change the y value in Proportion appropriately. When this scene
//	was built, the scene dimensions were (4, 2.5, 4). So the camera was 4 x dimensions.x = 16. The
//	y value in Proportion (Proportion.y) is the original scene camera height. Everything was sized
//	and positioned according to this height.
//
//	So build your scenes with dimensions set to (4, 2.5, 4). Position and scale this object's
//	transform to fit the scene. Then change your dimensions as you wish for the live site.
//
using UnityEngine;
using System;
using System.Collections;

public class DimensionsBase : MonoBehaviour {

	#region Properties
	// Public Properties
	public Vector3 Proportion = new Vector3(4.0f, 16.0f, 4.0f);
	
	// Private Properties
	protected Vector3 _dimensions = Vector3.zero;
	protected bool _initialized = false;
	#endregion
	
	#region Event Handlers
	// handlePhysicalDimensions	- this method is broadcast by script, InteractiveSpaceDimensions
	//								- the dimensions will be used to set min and max y-values.
	// On Entry:
	//		dimensions	- the 3D space dimensions in engineering units 
	//
	public virtual void handlePhysicalDimensions(Vector3 dimensions)
	{
		// Cache our dimensions.
		_dimensions = dimensions;
	}
	
	// Update is called once per frame
	// It's virtual so it can be overridden.
	public virtual void Update () 
	{
		// Wait until we have dimensions. Then create our wand.
		if (!_initialized && _dimensions != Vector3.zero)
		{
			// We have to reposition depending on the dimensions x and z as compared to what they were when the scene was built
			//    - we adjust y by the ratio of the camera height and our original Proportion.y when the scene was built.
			float ratio = Camera.main.transform.position.y / Proportion.y;
			Vector3 newPos = new Vector3(transform.position.x * _dimensions.x / Proportion.x, transform.position.y * ratio, transform.position.z * _dimensions.z / Proportion.z);
			transform.position = newPos;

			// Scale this game object			
			// We scale by the ratio of the camera height and our original Proportion.y when the scene was built
			transform.localScale = Vector3.Scale(new Vector3(ratio, ratio, ratio), transform.localScale);
			
			// Mark the object as initialized
			_initialized = true;
		}
	
	}
	#endregion
}
