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
// ObjectBillboard.cs
//---------------------------------------------------------------------------------------------------------------------
// This class will 'billboard' the object it is attached to to face the user provided or 'Main Camera' if not, 
// and mirror it's up vector as well.
//---------------------------------------------------------------------------------------------------------------------
// Author : Jeff Paxson
// Date : 6/22/2011
//---------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class ObjectBillboard : MonoBehaviour {

	// The camera we will use to billboard ourselves to
	public Camera billboardCamera;
	
	// Use this for initialization
	void Start () 
	{
		//  Get our billboard camera.  If the user didn't set one
		// then just get the main camera.  This may or may not return
		// a valid camera if the scene doesn't contain a 'Main Camera' object.
		if(billboardCamera == null)
		{
			SetBillboardCamera(Camera.main);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Don't do anything unless we have a valid camera to use.
		if(billboardCamera)
		{
			// Adjust the object's transform to billboard against the provided valid camera.
			transform.LookAt(transform.position + billboardCamera.transform.rotation * Vector3.forward,
				billboardCamera.transform.rotation * Vector3.up);
		}
	}
	
	// Set the camera used to billboard to the caller provided one
	public void SetBillboardCamera(Camera newCamera)
	{
		// Make sure we got a valid camera passed in.
		if(newCamera)
		{
			billboardCamera = newCamera;
		}
	}
	
	// Return to caller the currently used billboardCamera
	// This CAN return null so caller must handle that on their end.
	public Camera GetBillboardCamera()
	{
		return billboardCamera;
	}
}
