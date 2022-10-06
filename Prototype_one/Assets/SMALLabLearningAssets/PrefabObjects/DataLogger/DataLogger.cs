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
// DataLogger.cs
//---------------------------------------------------------------------------------------------------------------------
// The DataLogger creates  a tab delimited text file when passed in an array of strings comrising the 
// column headings to be used in the log output file.  A timestamp of the current date and time will be appended 
// to the provided column headings as the leftmost column, and subsequent additions of arrays of values corresponding to 
// the requested column headers are added with timestamp.  The output should be digestible by MS Excel for viewing.
//---------------------------------------------------------------------------------------------------------------------
// Author : Jeff Paxson
// Date : 5/24/2011
// Version: 1.2
//---------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataLogger : MonoBehaviour {
	
	public string dataFolder = "";
	public string baseDataLogFileName = "DataLog";
	private string dataLogFileName = "";
	private string dataLogFilePath = "";
	private string[] columnHeadings; 
	private bool isPaused = false;
	private double startTime = 0.0;
	
	
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	// Clears the contents of the DataLog including the columnHeadings.
	// And then rewrites existing column headings with original file creation
	// timestamps for headers.  The DataLog is then ready to be written to again.
	public void ClearDataLog ()
	{
		if (File.Exists(dataLogFilePath))
		{
			//CreateDataLog(columnHeadings);
			File.Create(dataLogFilePath).Close(); 
			
			// Write DataLog Headers
			WriteLogHeaders<string>(columnHeadings);
		}
		else
		{
			Debug.LogError(gameObject.name + "::ClearDataLog::User Attempted to Clear A DataLog that does not exist::" + dataLogFilePath);
		}
	}
	
	// Deletes the current DataLog file if it exists.  Returns true if
	// successful and false if not.
	public bool DestroyDataLog ()
	{
		// See if the DataLog exists before we attempt to delete it
		if (File.Exists(dataLogFilePath))
		{
			try
			{
				// Try to delete the file.
				File.Delete(dataLogFilePath);
				return true;
			}
			catch (IOException e)
			{
				// We couldn't delete the file.
				Debug.LogError(gameObject.name + "::DestroyDataLog::"  + e.Source);
				return false;
			}
		}
		else
		{
			Debug.LogError(gameObject.name + "::DestroyDataLog::User Attempted to Delete DataLog that does not exist::" + dataLogFilePath);
			return false;
		}
	}
	
	// Creates a DataLog tab delimited text file.
	public void CreateDataLog (string[] columnHeaders)
	{
		// See if we got passed a null or empty colum header array
		if(columnHeaders == null || columnHeaders.Length == 0)
		{
			Debug.LogError(gameObject.name + "::CreateDataLog::User Attempted to Create DataLog without providing a valid array of Column Headings. Array is NULL or Empty");
			return;
		}
		
		// Copy provided column heading strings for query later if needed.
		columnHeadings = new string[columnHeaders.Length];
		Array.Copy(columnHeaders, 0, columnHeadings, 0, columnHeaders.Length);
		
		// Build the strings comprising our DataLog file name and path
		BuildDataLogPaths();
		
		// Create or verify that a Datalogs subdirectory exists to
		// write our log files into.
		CreateDataLogDirectory();
		
		// Create the DataLog File
		File.Create(dataLogFilePath).Close(); 
		
		// Write DataLog Headers
		WriteLogHeaders<string>(columnHeadings);
		
	}
	
	private void BuildDataLogPaths ()
	{
		// Get our file name with timestamp and extension
		dataLogFileName = baseDataLogFileName + "_" + GetTimeStamp() + ".txt";
		
		// Get our start Time
		startTime = Time.time;
	}
	
	// Creates a DataLogs subfolder in current application directory if one
	// does not already exist.
	private void CreateDataLogDirectory ()
	{
		// If the user doesn't provide a valid fileSavePath then just use the Application.dataPath
		// which will save the DataLog in the root application directory
		if (String.IsNullOrEmpty(dataFolder)) 
		{
			//dataLogFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/SMALLabLearning/" + dataFolder;
			//fileSavePath = Application.dataPath;
			dataFolder = "";
			Debug.LogError(gameObject.name + "::CreateDataLogDirectory::User did not provide a valid file save path.  Saving Log to Application directory");
		}

		//string dataLogFolderPath = @fileSavePath ;
		string dataLogFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/SMALLabLearning/" + dataFolder;
		dataLogFilePath = dataLogFolderPath + "/" + dataLogFileName;
		//dataLogFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/SMALLabLearning/" + dataFolder + "/" + dataLogFileName;

		
		// See if a DataLog sub-directory exists.  If not create it.
		if (Directory.Exists(dataLogFolderPath) == false)
		{
			// Create DataLog sub-directory
			Directory.CreateDirectory(dataLogFolderPath);
		}
	}
	
	public void OnPlay ()
	{
		isPaused = false;
	}
	
	public void OnPause ()
	{
		isPaused = true;
	}
	
	// Reset destroys existing DataLog File and its Contents
	// Then rebuilds a new log with new timeStamp while
	// retaining original column headings.
	public void OnReset ()
	{
		
	}
	
	public void ResetDataLog()
	{
		DestroyDataLog();
		CreateDataLog(columnHeadings);
		isPaused = false;
	}
	
	public string[] GetColumnHeadings ()
	{
		return columnHeadings;
	}
	
	private double GetElapsedTime ()
	{
		return (Time.time - startTime);
	}
	
	public string GetTimeStamp ()
	{
		// Get the current System Date and Time.
		DateTime currentDateTime = DateTime.Now;
		string format = "yyyy_MM_dd_HH_mm_ss"; 
		string ts = currentDateTime.ToString(format); 
		return ts;
	}
	
	private bool WriteLogHeaders<T>(IList<T> values)
	{
		// Make sure that a DataLog file exists.
		if (File.Exists(dataLogFilePath))
		{
			// Create a container string
			string delimeter = "\t";
			string inputString = "Time(s)" + delimeter;
		
			foreach (T value in values)
			{
				inputString += (value.ToString() + delimeter);
			}
	
			// Remove the last tab character
			inputString = inputString.Remove(inputString.Length - 1);
		
			// Open our DataLog
			StreamWriter writer = new StreamWriter(dataLogFilePath, false);
		
			// Write input values into it.
			writer.WriteLine(inputString);
		
			// Close the stream writer
			writer.Close();
			
			return true;
		}
		else
		{
			Debug.LogError(gameObject.name + "::WriteLogHeaders::User Attempted Write Headers To DataLog File That Does Not Exist::" + dataLogFilePath);
			return false;
		}
	}
	
	public bool WriteLogValues<T>(IList<T> values)
    {
		print("logging values");
		if(isPaused)
		{
			Debug.LogError(gameObject.name + "::WriteLogValues::User Attempted Write Data To DataLog While The DataLog is Paused.");
			return false;
		}
		
		// Make sure that a DataLog file exists.
		if (File.Exists(dataLogFilePath))
		{
			// Create a container string
			string delimeter = "\t";
			string inputString = GetElapsedTime() + delimeter;
		
			foreach (T value in values)
			{
				inputString += (value.ToString() + delimeter);
			}
	
			// Remove the last tab character
			inputString = inputString.Remove(inputString.Length - 1);
		
			// Open our DataLog
			StreamWriter writer = new StreamWriter(dataLogFilePath, true);
		
			// Write input values into it.
			writer.WriteLine(inputString);
		
			// Close the stream writer
			writer.Close();
			
			return true;
		}
		else
		{
			Debug.LogError(gameObject.name + "::WriteLogValues::User Attempted Write Data To DataLog File That Does Not Exist::" + dataLogFilePath);
			return false;
		}
	}

}
