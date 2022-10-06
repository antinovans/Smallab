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

public class SmallabLineChartTest : MonoBehaviour {

	private GameObject _go;
	private SmallabLineChart _lineChart;
	private Vector3 _startPos;
	
	public float TimerTime2DTest = 1.0f;
	public bool EnableTranslateTest = false;

	private bool _doReset = false;
	private int _lineNo = 0;
	private float _startTime;

	private Vector2[] _line1Values = { new Vector2(0, 0), new Vector2(10, 10), new Vector2(20, 20), new Vector2(30, 30), new Vector2(40, 40), new Vector2(50, 50), new Vector2(60, 60), new Vector2(70, 70), new Vector2(80, 80), new Vector2(90, 90), new Vector2(100, 100) };
	private Vector2[] _line2Values = { new Vector2(0, 50), new Vector2(10, 50), new Vector2(20, 50), new Vector2(30, 50), new Vector2(40, 50), new Vector2(50, 50), new Vector2(60, 50), new Vector2(70, 50), new Vector2(80, 50), new Vector2(90, 50), new Vector2(100, 50) };
	private Vector2[] _line3Values = { new Vector2(0, 20), new Vector2(10, 40), new Vector2(20, 60), new Vector2(30, 80), new Vector2(40, 100), new Vector2(50, 80), new Vector2(60, 60), new Vector2(70, 40), new Vector2(80, 20), new Vector2(90, 0), new Vector2(100, 20) };
	private int[] _lineIdx = new int[3];
		
	// Use this for initialization
	void Start () {
	
		_lineIdx[0] = _lineIdx[1] = _lineIdx[2] = 0;
		
		// Find the LineChart game object
		_go = GameObject.Find("LineChart");
		if (_go != null)
		{
			// Save our starting position
			_startPos = _go.transform.position;
			
			// Get our 2D chart component
			_lineChart = (SmallabLineChart)_go.GetComponent("SmallabLineChart");
			if (_lineChart != null)
			{
				Debug.Log("LineChart found!");
			}
		}
		_startTime = Time.time;
		
		if (_lineChart != null) _lineChart.DisplayChart(true);
	}
	
	// Update is called once per frame
	void Update () {

		SmallabLine line;
		Vector2[] values = null;
		
		// If we found a line chart, start plotting points.
		
		if (_lineChart != null)
		{
			// Delay so we see simulated real-time charting
			if (Time.time - _startTime >= TimerTime2DTest)
			{				
				if (EnableTranslateTest)
				{
					_go.transform.position = new Vector3(_go.transform.position.x - 0.05f, _go.transform.position.y, _go.transform.position.z);
					if (_go.transform.position.x < 0.10f)
						_go.transform.position = _startPos;
				}
				values = GetLineValues(_lineNo);
				if (values != null && _lineIdx[_lineNo] < values.Length)
				{
					line = _lineChart.Lines[_lineNo];
					_lineChart.AddValue(line, values[_lineIdx[_lineNo]]);
					_lineIdx[_lineNo]++;
				}
				_lineNo++;
				if (_lineNo >= _lineIdx.Length) _lineNo = 0;

				_startTime = Time.time;
			}
			
			// After all the lines are charted, reset
			int linesCharted = 0;
			for(int i=0; i < _lineIdx.Length; i++)
			{
				values = GetLineValues(i);
				if (values != null && _lineIdx[i] >= values.Length)
				{
					linesCharted++;
				}
			}
			if (linesCharted == _lineIdx.Length)
			{
				// Skip one second so we see the last point.
				if (_doReset)
				{
					for(int i=0; i < _lineIdx.Length; i++)
						_lineIdx[i] = 0;
					_lineChart.Reset();
					_doReset = false;
				}
				else
				{
					_doReset = true;
				}
			}
		}		
	}
		
	private Vector2[] GetLineValues(int lineNo)
	{
		Vector2[] values = null;
		switch (lineNo)
		{
			case 0:
				values = _line1Values;
				break;

			case 1:
				values = _line2Values;
				break;
					
			case 2:
				values = _line3Values;
				break;
					
			default:
				break;
		}
		return values;
	}
}
