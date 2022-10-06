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
using System.Collections.Generic;

public class SmallabScrollingLineChart : SmallabLineChart {

    #region Properties
	// Public properties
	public float TimerTime = 1.0f;
	
	// Private properties
	private Dictionary<SmallabLine, int> _currentPointIdx;
	private float _startTime;
	#endregion	

	#region Event Handlers
	public override void Start()
	{
		// Set the point indices
		_currentPointIdx = new Dictionary<SmallabLine, int>();		
		foreach(SmallabLine line in Lines)
		{
			_currentPointIdx.Add(line, 0);
		}
		// If we have a TotalPointsInChart value, use it to set the max domain value
		if (TotalPointsInChart > 0)
			MaxValues.x = TotalPointsInChart;			

		base.Start();
	}
	
	public override void LateUpdate()
	{
		// Delay so we see simulated real-time charting
		if (Time.time - _startTime >= TimerTime)
		{
			// Just mark all the lines as dirty so we plot something
			for(int i=0; i < Lines.Length; i++)
				Lines[i].IsDirty = true;

			// Call the base class' LateUpdate method
			base.LateUpdate();
			
			// Reset the timer
			_startTime = Time.time;			
		}
	}		
	#endregion
	
	#region Public Methods
	// AddValue	- Creates the a Vector2 value that corresponds to a time/value pair and appends it to the line.
	//
	// On Entry:
	//		line	- the line to which a new value will be added
	//		yValue	- range value
	//
	public void AddValue(SmallabLine line, float yValue)
	{
		if (line != null && _currentPointIdx.ContainsKey(line))
		{
			// Calculate the domain (time) value based on the current point index
			float xValue = _currentPointIdx[line];
			if (_currentPointIdx[line] >= TotalPointsInChart)
				xValue = TotalPointsInChart;
			
			// Create a Vector2 value that has a time value for the domain.
			Vector2 vValue = new Vector2(xValue, yValue);
			
			// Call the base class' AddValue method
			AddValue(line, vValue);
			
			// Increment the time unit
			_currentPointIdx[line]++;
		}
	}	
	#endregion
}
