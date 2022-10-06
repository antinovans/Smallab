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

public class DataLoggerControlExample : MonoBehaviour {
	
	DataLogger dataLogger;
	
	// Use this for initialization
	void Start () {
		dataLogger = GameObject.Find("DataLogger").GetComponent<DataLogger>(); // get a reference to the data logger prefab
		string [] dataLogHeaders = {"x", "y", "z"}; // these are the headings that will be at the top of the file
		// this directory can either be set in code or in the settings of the DataLogger prefab itself
		//dataLogger.fileSavePath = "../MyScenario_DATALOGS"; // tell it which directory to write the files (../ means that it will be parallel to the application directory)
		dataLogger.CreateDataLog(dataLogHeaders);
	}
	
	void logCurrentPoint(){
		float []values = {transform.position.x, transform.position.y, transform.position.z}; // create an array based on the current position of this object
		dataLogger.WriteLogValues<float>(values); // actually write the data to the file
	}

	// Update is called once per frame
	void FixedUpdate () {
		//logCurrentPoint();
	}
	
	
	void OnPause(){
		
	}
	
	void OnPlay(){
		
	}
	
	void OnReset(){
		
	}
	
}
