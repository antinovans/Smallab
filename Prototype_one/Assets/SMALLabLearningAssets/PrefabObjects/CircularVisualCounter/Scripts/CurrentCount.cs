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

//An example script for getting the current count from the CircularVisualCounter
//drag on to an empty and assign the counter object
//on Play, the current count will be displayed in the console
using UnityEngine;
using System.Collections;

public class CurrentCount : MonoBehaviour {

	public GameObject CtrObj;//the counter object, assign in the inspector
	
	private CircularVisualCounter myScript;//an instance of the script

	// Use this for initialization
	void Start () {
		myScript = CtrObj.GetComponent(typeof(CircularVisualCounter)) as CircularVisualCounter;//get the instance of the script
	}
	
	// Update is called once per frame
	void Update () {
	Debug.Log(myScript.GetCurrentCount());//alternately CircularVisualCounter.currentCount could be called
	}
}
