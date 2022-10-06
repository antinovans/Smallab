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

public class SlopeInterceptorScript : MonoBehaviour {
	public float m;
	public float b;
	public GUIStyle backgroundStyle;
	
	void Awake () {
		//print("slopeInterceptorScript::Awake()");
	}
	
	void OnGUI () {
		if( null==backgroundStyle ) {
			//print("slopeInterceptorScript::OnGUI - WARNING: backgroundStyle is NULL.  Returning without rendering.\n");	
			return;
		}
		
		GUILayout.BeginArea(new Rect(130,95,200,150));
		GUILayout.BeginVertical();
			
			GUILayout.Label("y = " + m.ToString("F2") + "x + " + b.ToString("F2"), backgroundStyle, GUILayout.ExpandWidth(false));
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
