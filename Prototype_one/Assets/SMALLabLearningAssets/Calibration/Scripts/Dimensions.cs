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
using System.IO;
using System.Xml;

public class Dimensions : MonoBehaviour {

	public Vector3 dimensions;
	object[] allGameObjects;
	
	StreamReader streamReader;

	void Awake(){
		//dimensions = new Vector3(2.0f * 0.1f, 2.0f * 0.1f, 4.0f * 0.1f);
		//dimensions = new Vector3(3.5f, 2.5f, 3.5f);	
		dimensions = readDimensionsFromXMLFile();
	}

	// Use this for initialization
	void Start () {
		
		//GameObject go = GameObject.Find("Cube");
		//go.transform.localScale = dimensions;
		//go.transform.position = new Vector3(0f, (go.transform.localScale.y * 0.5f), 0f);
		
		transform.localScale = dimensions;
		//transform.position = new Vector3(0f, (transform.localScale.y * 0.5f), 0f);
		

		broadcastDimensions();
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	private void broadcastDimensions(){
		// get an array of all the objects
		allGameObjects = GameObject.FindSceneObjectsOfType(typeof (GameObject));
		
		// go through each object in the scene
		foreach (object o in allGameObjects){
       		GameObject g = (GameObject) o;
       		// give that object the dimensions
       		g.SendMessage("handlePhysicalDimensions", dimensions, SendMessageOptions.DontRequireReceiver);
  		}
		
	}
	
	public Vector3 getDimensionsVector(){
		return dimensions;
	}
	
	private Vector3 readDimensionsFromFile(){
	
		//Debug.Log(Application.persistentDataPath + "/" + "SMALLabLearningPreferences.txt");
		
		int lineIndex = 0;
		Vector3 inputDimensions = new Vector3(0.0f, 0.0f, 0.0f);
		
		try{
			//streamReader = new StreamReader(Application.persistentDataPath + "/" + "SMALLabLearningPreferences.txt"); 
			streamReader = new StreamReader(Application.dataPath + "/../../" + "SMALLabLearningPreferences.txt"); 
			
			string line;
			// Read and display lines from the file until the end of 
            // the file is reached.
            while ((line = streamReader.ReadLine()) != null){
            	//Debug.Log(line);
				
				
				
				
				lineIndex++;
			}
			
		}catch(System.Exception e){
		
			Debug.Log(e.ToString());
			
		}
            
       return inputDimensions;

	}
	
	
	private Vector3 readDimensionsFromXMLFile(){
		
		Vector3 inputDimensions = new Vector3(0.0f, 0.0f, 0.0f);
		
		string xstr = "-1";
		string ystr = "-1";
		string zstr = "-1";
			
		try{
			XmlDocument doc = new XmlDocument();



			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor){
				doc.Load (System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) 
			          + "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml");
			}else{
				doc.Load (System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) 
			          + "/SMALLabLearning/SMALLabLearningPreferences_v1.0.xml");
			}
            
			
			
			XmlNodeList xnList = doc.SelectNodes("smallablearning/smallab/dimensions");
			foreach (XmlNode xn in xnList){
 				xstr = xn["x"].InnerText;
  				ystr = xn["y"].InnerText;
				zstr = xn["z"].InnerText;
  				
			} 
			
			//Debug.Log("Dimensions: " + x + ", " + y + ", " + z);
			
			inputDimensions = new Vector3(float.Parse(xstr), float.Parse(ystr), float.Parse(zstr));
			
		}catch(System.Exception e){
		
			Debug.Log(e.ToString());
			
		}
            
		return inputDimensions;
	}
	
	/*
	string fileName = Application.persistentDataPath + "/" + FILE_NAME;
	fileWriter = File.CreateText(fileName);
	fileWriter.WriteLine("Hello world");
	fileWriter.Close();
	*/
}
