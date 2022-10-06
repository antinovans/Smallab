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

// Module:  AnimatedTexture.cs
// Author:  Rob Galante
// Date:    June 15, 2011

// ****************************************************************************
//
using UnityEngine;
using System;
using System.Collections;

public class AnimatedTexture : DimensionsBase {

	#region Properties
	// Public Properties
	public int TileX = 4;	// number of columns
	public int TileY = 4;	// number of row
	public float FramesPerSecond = 30;
	
	private bool _isEnabled = false;
	public bool IsEnabled
	{
		get { return _isEnabled; }
		set 
		{ 
			if (_isEnabled != value && value == true)
				_startTime = Time.time;
			_isEnabled = value; 
		}
	}
	
	// Private Properties
	private float _startTime;
	#endregion
	
	#region Event Handlers
	// handlePhysicalDimensions	- this method is broadcast by script, InteractiveSpaceDimensions
	//								- the dimensions will be used to set min and max y-values.
	// On Entry:
	//		dimensions	- the 3D space dimensions in engineering units 
	//
	public override void handlePhysicalDimensions(Vector3 dimensions)
	{
		// The base class caches our _dimensions
		base.handlePhysicalDimensions(dimensions);
	}

	public override void Update() 
	{
		// The base class sets our _initialized flag
		base.Update();
		
		if (_initialized)
		{
			// Calculate index. Assume we are not enabled, and display the last tile.
			int index = 0;
			// If enabled, use the current value to determine which offset to use.
			if (_isEnabled)
			{
				index = Convert.ToInt32((Time.time - _startTime) * FramesPerSecond);
			}
			// Repeat when exhausting all frames
			index = index % (TileX * TileY);
		
			// Size of every tile
			Vector2 size = new Vector2 (1.0f / TileX, 1.0f / TileY);
		
			// Split into horizontal and vertical index
			int uIndex = index % TileX;
			int vIndex = index / TileX;

			// Build offset
			// v coordinate is the bottom of the image in opengl so we need to invert.
			Vector2 offset = new Vector2 (uIndex * size.x, 1.0f - size.y - vIndex * size.y);
		
			GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", offset);
			GetComponent<Renderer>().material.SetTextureScale ("_MainTex", size);
		}
	}
	#endregion
}
