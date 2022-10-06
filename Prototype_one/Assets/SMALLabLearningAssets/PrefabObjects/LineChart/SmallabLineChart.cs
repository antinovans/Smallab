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


using UnityEngine;
using System;
using System.Collections.Generic;

public enum SmallabLineChartDirection {
	LeftToRight = 0,
	RightToLeft
}

public enum SmallabLineChartRangeAxisPosition {
	Default = 0,
	Left = 1,
	Right = 2
}

public class SmallabLineChart : MonoBehaviour {

        #region Properties
        // Private and Protected properties
        private static Camera _cam;
        private static Camera _mainCam;
        private SmallabLine _gridDomainCenterLine;
        private SmallabLine _gridDomainLine;
        private SmallabLine _gridLine;
        private SmallabLine _gridRangeCenterLine;
        private SmallabLine _gridRangeLine;
		private GUIStyle _valueLabelStyle;
		private Rect _maxValueRect;
		private string _maxValue;
		private Rect _midValueRect;
		private string _midValue;
		private Rect _minValueRect;
		private string _minValue;

		private float _domain;
		private float _range;
		private float _width;
		private float _height;
		private float _xPadding;
		private float _yPadding;
		private float _xStartPos;
		private float _xStartPosPlusPadding;
		private float _yStartPos;
		private float _yStartPosPlusPadding;
        private Vector2 _innerGridDimensions;

        // Public properties
        public static int ChartLayer = 31;
		public SmallabLineChartDirection ChartDirection = SmallabLineChartDirection.LeftToRight;
        public Vector2 GridDimensions = new Vector2(0.5f, 0.5f);
        public Material GridDomainAxisMaterial;
        public Material GridLineMaterial;
        public Material GridRangeAxisMaterial;
		public SmallabLineChartRangeAxisPosition GridRangeAxisPosition = SmallabLineChartRangeAxisPosition.Default;
		public bool GridRangeAxisValues = false;
        public float GridAxisLineWidth = 0.5f;
        public float GridLineWidth = 0.25f;
		public SmallabLine[] Lines;
        public Vector2 MaxValues = new Vector2(100, 100);
        public Vector2 MinValues = new Vector2(0, 0);
        public Vector2 Padding = new Vector2(0.05f, 0.05f);
		public GUISkin ChartSkin;
		public bool ShowCenterLines = false;
		public Vector2 TicMarks = new Vector2(10, 10);
		public int TotalPointsInChart;
        public SmallabLabel[] Labels;
        #endregion

        #region Event Handlers
        public virtual void Awake()
        {
            SmallabLineChart.InitCamera();
        }

		public virtual void OnGUI()
		{
   			if (ChartSkin != null) {
				GUI.skin = ChartSkin;
			}
			// Only show labels if the camera is enabled
			if (_cam != null && _cam.enabled)
			{
				// Position labels
				PositionLabels();
				
				// Position values
				PositionValues();
			}
		}
		
        public virtual void Start()
        {
			// Set the parent gameObject's layer
			gameObject.layer = ChartLayer;
			
            // Calculate helpful intermediate values and inner grid dimensions (in screen pixel positions)
			_domain = MaxValues.x - MinValues.x;
			_range = MaxValues.y - MinValues.y;
			_width = Screen.width * GridDimensions.x;
			_height = Screen.height * GridDimensions.y;
			_xPadding = _width * Padding.x;
			_yPadding = _height * Padding.y;
			_xStartPos = Screen.width * transform.position.x;
			_xStartPosPlusPadding = _xStartPos + _xPadding;
			_yStartPos = Screen.height * transform.position.z;
			_yStartPosPlusPadding = _yStartPos + _yPadding;
			
            _innerGridDimensions = new Vector2(_width - _xPadding * 2, _height - _yPadding * 2);

			// Initialize Range Labels
			InitRangeLabels();
			
			// Show the grid
            ShowGrid();
			
        }

		public virtual void Update()
		{
			float range = MaxValues.y - MinValues.y;
			if (range != _range)
			{
				_range = range;
				InitRangeLabels();
			}
			// Recalculate domain. It's faster than testing for changes.
			_domain = MaxValues.x - MinValues.x;
		}
		
        public virtual void LateUpdate()
        {
			SmallabLine line;

			if (_cam != null && _cam.enabled)
			{
				for(int i=0; i < Lines.Length; i++)
				{
					line = Lines[i];
					if (line.IsDirty)
					{
						if (line.Points != null && line.Points.Length > 1)
						{
							line.RebuildMesh(gameObject.transform);
						}
					}
				}
			}
        }		
        #endregion

        #region Public Methods
		// AddValue	- Converts the new value to a point in 2D space and appends it to the line.
		//
		// On Entry:
		//		line	- the line to which a new value will be added
		//		value	- a Vector2 structure that specifies the domain and range values
		//				  (these are floats)
		//
		public void AddValue(SmallabLine line, Vector2 value)
		{
			if (line != null)
			{
				// Clamp domain and range values
				if (value.x < MinValues.x) value.x = MinValues.x;
				if (value.x > MaxValues.x) value.x = MaxValues.x;
				if (value.y < MinValues.y) value.y = MinValues.y;
				if (value.y > MaxValues.y) value.y = MaxValues.y;
				
				// Convert the value from domain/range to a x/y point in 2D space within the drawable area
				Vector2 point;
				if (ChartDirection == SmallabLineChartDirection.LeftToRight)
					point = new Vector2(_xStartPosPlusPadding + _innerGridDimensions.x * (value.x - MinValues.x) / _domain, _yStartPosPlusPadding + _innerGridDimensions.y * (value.y - MinValues.y) / _range);
				else
					point = new Vector2(_xStartPosPlusPadding + (_innerGridDimensions.x - _innerGridDimensions.x * (value.x - MinValues.x) / _domain), _yStartPosPlusPadding + _innerGridDimensions.y * (value.y - MinValues.y) / _range);
				
				//Debug.Log("Value = " + value.x + "," + value.y + "  Point = " + point.x + "," + point.y);
				
				// Append it to the line
				line.AddPoint(point, TotalPointsInChart);
			}
		}
		
		// DisplayChart	- Shows or hides the chart using the camera
		//
		// On Entry:
		//		show	- if true, show the chart, else hide the chart
		//
		public void DisplayChart(bool show)
		{
			if (_cam != null) _cam.enabled = show;
		}
		
        public static void InitCamera()
        {
            InitCamera(CameraClearFlags.Depth);
        }

        public static void InitCamera(CameraClearFlags clearFlags)
        {
            if (Camera.main == null)
            {
                Debug.LogError("SmallabLineChart.InitCamera: No camera tagged as \"Main Camera\"");
                return;
            }
            InitCamera(Camera.main, clearFlags);
        }

        public static void InitCamera(Camera thisCamera, CameraClearFlags clearFlags)
        {
            if (_cam == null)
            {
                _cam = new GameObject("SMALLabChartCamera", typeof(Camera)).GetComponent<Camera>();
                DontDestroyOnLoad(_cam);
            }
            _cam.depth = thisCamera.depth + 1;
            _cam.clearFlags = clearFlags;
            _cam.orthographic = false;
            _cam.fieldOfView = 90.0f;
            _cam.farClipPlane = Screen.height / 2 + 0.0101f;
            _cam.nearClipPlane = Screen.height / 2 - 0.0001f;
            _cam.transform.position = new Vector3(Screen.width / 2 - 0.5f, Screen.height / 2 - 0.5f, 0.0f);
            _cam.transform.eulerAngles = Vector3.zero;
            _cam.cullingMask = 1 << ChartLayer;
            _cam.backgroundColor = thisCamera.backgroundColor;

            thisCamera.cullingMask = thisCamera.cullingMask & (-1 ^ (1 << ChartLayer));
            _mainCam = thisCamera;
        }
		
		public void Reset()
		{
			SmallabLine line;

			for(int i=0; i < Lines.Length; i++)
			{
				line = Lines[i];
				if (line.Points != null && line.Points.Length >= 1)
				{
					line.Reset();
				}
			}
		}
        #endregion

        #region Private Methods
		private void InitRangeLabels()
		{
			if (GridRangeAxisValues && ChartSkin != null)
			{
				// Copy the label from the chart skin
				_valueLabelStyle = new GUIStyle(ChartSkin.GetStyle("label"));
				// Set alignment
				_valueLabelStyle.alignment = TextAnchor.MiddleCenter;
				// Set the font size
				_valueLabelStyle.fontSize = Convert.ToInt32(_valueLabelStyle.fontSize * 0.75f);
				// Set the labels so we can calculate their dimensions
				_minValue = MinValues.y.ToString();
				_maxValue = MaxValues.y.ToString();
				_midValue = (MinValues.y + (MaxValues.y - MinValues.y) / 2).ToString();
			}
		}
		
		private void PositionLabels()
		{
			// Save GUI Matrix
			Matrix4x4 m = GUI.matrix;
			SmallabLabel label;
			
			for(int i = 0; i < Labels.Length; i++)
			{
				label = Labels[i];
				if (label.Enabled)
				{
					// Calculate the label's absolute screen position
					// _xStartPos & _yStartPos ( upper left corner of the chart )
					// _width & height ( rectangle size of chart )
					float xPos = _xStartPos + _width * label.Offset.x;
					float yPos = Screen.height - (_yStartPos + _height * (1 - label.Offset.y));
					float width = label.Dimensions.x * _width;
					float height = label.Dimensions.y * _height;
					
					Vector2 v = new Vector2(xPos, yPos);

					if (label.Orientation == Orientation.Vertical)
					{
						GUIUtility.RotateAroundPivot(-90.0f, v);					
					}
					GUI.Label ( new Rect( xPos, yPos, width, height), label.Text);
					if (label.Orientation == Orientation.Vertical)
					{
						// Recover GUI Matrix
						GUI.matrix = m;
					}
				}
			}
		}
		
		private void PositionValues()
		{
			try
			{
				// If the user wants range axis values and we have a chart skin
				if (GridRangeAxisValues && ChartSkin != null)
				{
					// Calculate their width and height
					_minValueRect = GUILayoutUtility.GetRect(new GUIContent(_minValue), _valueLabelStyle);
					_midValueRect = GUILayoutUtility.GetRect(new GUIContent(_midValue), _valueLabelStyle);
					_maxValueRect = GUILayoutUtility.GetRect(new GUIContent(_maxValue), _valueLabelStyle);
					// Calculate their position
					float yCenterPos = _innerGridDimensions.y / 2;				
					if ((ChartDirection == SmallabLineChartDirection.LeftToRight && GridRangeAxisPosition == SmallabLineChartRangeAxisPosition.Default) || GridRangeAxisPosition == SmallabLineChartRangeAxisPosition.Right)
					{
						// Put the min value label at the bottom of the range axis
						_minValueRect = new Rect(_xStartPosPlusPadding + _innerGridDimensions.x + 4 * GridAxisLineWidth, Screen.height - (_yStartPosPlusPadding + _minValueRect.height / 2), _minValueRect.width + 8, _minValueRect.height);
						// Put the mid value label at the middle of the range axis
						_midValueRect = new Rect(_xStartPosPlusPadding + _innerGridDimensions.x + 4 * GridAxisLineWidth, Screen.height - (_yStartPosPlusPadding + yCenterPos  + _midValueRect.height / 2), _midValueRect.width + 8, _midValueRect.height);
						// Put the max value label at the top of the range axis
						_maxValueRect = new Rect(_xStartPosPlusPadding + _innerGridDimensions.x + 4 * GridAxisLineWidth, Screen.height - (_yStartPosPlusPadding + _innerGridDimensions.y + _maxValueRect.height / 2), _maxValueRect.width + 8, _maxValueRect.height);
					}
					else
					{
						// Put the min value label at the bottom of the range axis
						_minValueRect = new Rect(_xStartPosPlusPadding - (_minValueRect.width + 8) - 4 * GridAxisLineWidth, Screen.height - (_yStartPosPlusPadding + _minValueRect.height / 2), _minValueRect.width + 8, _minValueRect.height);
						// Put the mid value label at the middle of the range axis
						_midValueRect = new Rect(_xStartPosPlusPadding - (_midValueRect.width + 8) - 4 * GridAxisLineWidth, Screen.height - (_yStartPosPlusPadding + yCenterPos  + _midValueRect.height / 2), _midValueRect.width + 8, _midValueRect.height);
						// Put the max value label at the top of the range axis
						_maxValueRect = new Rect(_xStartPosPlusPadding - (_maxValueRect.width + 8) - 4 * GridAxisLineWidth, Screen.height - (_yStartPosPlusPadding + _innerGridDimensions.y + _maxValueRect.height / 2), _maxValueRect.width + 8, _maxValueRect.height);
					}				
					// Display the labels.
					GUI.Label ( _minValueRect, _minValue, _valueLabelStyle);
					if (ShowCenterLines)
						GUI.Label ( _midValueRect, _midValue, _valueLabelStyle);
					GUI.Label ( _maxValueRect, _maxValue, _valueLabelStyle);
				}
			}
			catch(Exception ex)
			{
				Debug.Log("Exception: " + ex.Message);
			}
		}
        private void ShowGrid()
        {
            // Calculate the number of points in the grid
            Vector2[] gridPoints = new Vector2[Convert.ToInt32((TicMarks.x * 2) + (TicMarks.y * 2)) ];
			Vector2[] gridDomainPoints = new Vector2[2] { new Vector2(_xStartPosPlusPadding, _yStartPosPlusPadding), new Vector2(_xStartPosPlusPadding + _innerGridDimensions.x, _yStartPosPlusPadding) };
			Vector2[] gridRangePoints;
			if ((ChartDirection == SmallabLineChartDirection.LeftToRight && GridRangeAxisPosition == SmallabLineChartRangeAxisPosition.Default) || GridRangeAxisPosition == SmallabLineChartRangeAxisPosition.Right)
				gridRangePoints = new Vector2[2] { new Vector2(_xStartPosPlusPadding + _innerGridDimensions.x, _yStartPosPlusPadding), new Vector2(_xStartPosPlusPadding +  + _innerGridDimensions.x, _yStartPosPlusPadding + _innerGridDimensions.y - 1) };
			else
				gridRangePoints = new Vector2[2] { new Vector2(_xStartPosPlusPadding, _yStartPosPlusPadding), new Vector2(_xStartPosPlusPadding, _yStartPosPlusPadding + _innerGridDimensions.y) };
			
            // Initialize the grid points
            int index = 0;
            float gridPixels = _innerGridDimensions.x / TicMarks.x;
            float xPos = _xPadding;
			if (ChartDirection != SmallabLineChartDirection.RightToLeft)
				xPos += gridPixels;
            for (int x = 0; x < TicMarks.x; x++)
            {
                gridPoints[index++] = new Vector2(_xStartPos + xPos, _yStartPosPlusPadding);
                gridPoints[index++] = new Vector2(_xStartPos + xPos, _yStartPosPlusPadding + _innerGridDimensions.y);
                xPos = xPos + gridPixels;
            }
            gridPixels = _innerGridDimensions.y / TicMarks.y;
            float yPos = _yPadding + gridPixels;
            for (int y = 0; y < TicMarks.y; y++)
            {
                gridPoints[index++] = new Vector2(_xStartPosPlusPadding, _yStartPos + yPos);
                gridPoints[index++] = new Vector2(_xStartPosPlusPadding + _innerGridDimensions.x , _yStartPos + yPos);
                yPos = yPos + gridPixels;
            }
			
			_gridDomainLine = new SmallabLine(gameObject.transform, "GridDomain", gridDomainPoints, GridDomainAxisMaterial, GridAxisLineWidth);
			_gridRangeLine = new SmallabLine(gameObject.transform, "GridRange", gridRangePoints, GridRangeAxisMaterial, GridAxisLineWidth);
			_gridLine = new SmallabLine(gameObject.transform, "Grid", gridPoints, GridLineMaterial, GridLineWidth);			
			
			if (ShowCenterLines)
			{
				float xCenterPos = _innerGridDimensions.x / 2;
				float yCenterPos = _innerGridDimensions.y / 2;				
				Vector2[] gridDomainCenterPoints = new Vector2[2] { new Vector2(_xStartPosPlusPadding + xCenterPos, _yStartPosPlusPadding), new Vector2(_xStartPosPlusPadding + xCenterPos, _yStartPosPlusPadding + _innerGridDimensions.y - 1) };
				Vector2[] gridRangeCenterPoints = new Vector2[2] { new Vector2(_xStartPosPlusPadding, _yStartPosPlusPadding + yCenterPos), new Vector2(_xStartPosPlusPadding + _innerGridDimensions.x - 1, _yStartPosPlusPadding + yCenterPos) };
				_gridDomainCenterLine = new SmallabLine(gameObject.transform, "GridDomainCenter", gridDomainCenterPoints, GridLineMaterial, GridAxisLineWidth + GridAxisLineWidth * 0.1f);
				_gridRangeCenterLine = new SmallabLine(gameObject.transform, "GridRangeCenter", gridRangeCenterPoints, GridLineMaterial, GridAxisLineWidth + GridAxisLineWidth * 0.1f);
			}			
        }		
        #endregion
}
