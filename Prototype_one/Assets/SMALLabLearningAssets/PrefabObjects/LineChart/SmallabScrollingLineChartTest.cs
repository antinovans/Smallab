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

public class SmallabScrollingLineChartTest : MonoBehaviour {

	private GameObject _go;
	private SmallabScrollingLineChart _lineChart;
	
	public float TimerTime = 1.0f;

	private float _angle;
	private float _startTime;

	// Use this for initialization
	void Start () {
	
		// Find the LineChart game object
		_go = GameObject.Find("ScrollingLineChart");
		if (_go != null)
		{
			// Get our 2D chart component
			_lineChart = (SmallabScrollingLineChart)_go.GetComponent("SmallabScrollingLineChart");
			if (_lineChart != null)
			{
				Debug.Log("LineChart found!");
			}
		}
		_angle = -1;
		_startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		SmallabLine lineSine, lineCos;
		float yValue;
		
		// If we found a line chart, start plotting points.
		
		if (_lineChart != null)
		{
			// Delay so we see simulated real-time charting
			if (Time.time - _startTime >= TimerTime)
			{				
				if (_lineChart.Lines != null && _lineChart.Lines.Length > 0)
				{
					lineSine = _lineChart.Lines[0];
					
					_angle += 1.0f;
					if (_angle > 359) _angle = 0;
					
					// Do the sine wave
					yValue = Mathf.Sin(_angle * Mathf.Deg2Rad);
					_lineChart.AddValue(lineSine, yValue);
					
					if (_lineChart.Lines.Length > 1)
					{
						lineCos = _lineChart.Lines[1];
						// Do the cosine wave
						yValue = Mathf.Cos(_angle * Mathf.Deg2Rad);
						_lineChart.AddValue(lineCos, yValue);
					}
				}
				_startTime = Time.time;
			}
		}		
	}		
}
