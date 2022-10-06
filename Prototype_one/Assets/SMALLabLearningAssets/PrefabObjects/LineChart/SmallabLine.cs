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

[Serializable]
public class SmallabLine {

        #region Properties
		public bool Continuous = false;
		public bool IsVisible = true;
        public int LineDepth;
        public float LineWidth;
        public Material LineMaterial;
		public Material PointMaterial;
		
		public Material NewestPointMaterial;
		
		public float PointWidth = 1.5f;
		public float NewestPointWidth = 6.0f;
		public int TrackedObjectId = 0;

		private bool _isDirty = false;
		private static int _lineNo = 0;
        private static Material _defaultMaterial = null;
		private int[] _lineTriangles;
        private Vector2[] _lineUVs;
        private Vector3[] _lineVertices;
        private Mesh _mesh;
        private MeshFilter _meshFilter;
        private Mesh _pointMesh;
        private MeshFilter _pointMeshFilter;
		
		private Mesh _newestPointMesh;
        private MeshFilter _newestPointMeshFilter;
	
        private List<Vector2> _points;
		private int[] _pointTriangles;
        private Vector2[] _pointUVs;
        private Vector3[] _pointVertices;
	
		private int[] _newestPointTriangles;
        private Vector2[] _newestPointUVs;
        private Vector3[] _newestPointVertices;
	
        private GameObject _smallabLineObject;
        private GameObject _smallabPointsObject;
		private GameObject _smallabNewestPointObject;

		public bool IsDirty
		{
			get { return _isDirty; }
			set { _isDirty = value; }
		}
		
        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public MeshFilter MeshFilter
        {
            get { return _meshFilter; }
            set { _meshFilter = value; }
        }

		public Vector2[] Points
		{
            get { return _points.ToArray(); }
            set 
			{
				if (value != null && value.Length > 0)
				{
					_points = new List<Vector2>(value.Length);
					_points.AddRange(value); 
				}
				else
					_points = new List<Vector2>();
			}
        }

        public GameObject SmallabLineObject
        {
            get { return _smallabLineObject; }
            set { _smallabLineObject = value; }
        }

        public GameObject SmallabPointsObject
        {
            get { return _smallabPointsObject; }
            set { _smallabPointsObject = value; }
        }
		
        #endregion

        #region Constructors
	    // Constructors
        public SmallabLine()
        {
			_points = new List<Vector2>();
        }
    	public SmallabLine (Transform parent, string name, Vector2[] points, Material material, float width) {
	    	Points = points;
            InitMesh(parent, name, material, width, 0);
    	}
        public SmallabLine(Transform parent, string name, Vector2[] points, Material material, float width, int depth)
        {
	    	Points = points;
            InitMesh(parent, name, material, width, depth);
        }
    	public SmallabLine (Transform parent, string name, Vector2[] points, Material lineMaterial, float lineWidth, Material pointMaterial, Material newestPointMaterial, float newestPointWidth, float pointWidth, bool continuous) {
	    	Points = points;
			Continuous = continuous;
            InitMesh(parent, name, lineMaterial, lineWidth, 0);
			if (PointMaterial != null && PointWidth > 0)
			{
				// If the mesh has not been initialized, do so now.
				InitPoints(parent, name, pointMaterial, pointWidth, LineDepth+1);
			}
			if (newestPointMaterial != null && NewestPointWidth > 0)
			{
				// If the mesh has not been initialized, do so now.
				InitNewestPoint(parent, name, newestPointMaterial, NewestPointWidth, LineDepth+1);
			}
    	}
        public SmallabLine(Transform parent, string name, Vector2[] points, Material lineMaterial, float lineWidth, int depth, Material pointMaterial, float pointWidth, Material newestPointMaterial, float newestPointWidth, bool continuous)
        {
	    	Points = points;
			Continuous = continuous;
            InitMesh(parent, name, lineMaterial, lineWidth, depth);
			if (PointMaterial != null && PointWidth > 0)
			{
				// If the mesh has not been initialized, do so now.
				InitPoints(parent, name, pointMaterial, pointWidth, LineDepth+1);
			}
			if (newestPointMaterial != null && NewestPointWidth > 0)
			{
				// If the mesh has not been initialized, do so now.
				InitNewestPoint(parent, name, newestPointMaterial, NewestPointWidth, LineDepth+1);
			}
        }
        #endregion

        #region Public Methods
		public void AddPoint(Vector2 point)
		{
			AddPoint(point, 0);
		}

		public void AddPoint(Vector2 point, int maxPointsInLine)
		{
			lock(_points)
			{
				// If we have a limit on our chart length
				if (maxPointsInLine > 0 && _points.Count >= maxPointsInLine)
				{
					// Shift all the range values down
					for(int i=0; i < _points.Count-1; i++)
					{
						Vector2 v = _points[i];
						v.y = _points[i+1].y;
						_points[i] = v;
					}
					// Set the last point value
					_points[_points.Count-1] = new Vector2() { x=point.x, y=point.y };
				} else {
					_points.Add(new Vector2() { x=point.x, y=point.y });
				}
				_isDirty = true;
			}
		}

		public void RebuildMesh(Transform parent)
		{
			RebuildMesh(parent, false);
		}
		
		public void RebuildMesh(Transform parent, bool doTransform)
		{
			lock(_points)
			{
				// If a mesh exists, clear it so we can rebuild
				if (_mesh != null)
				{
					_mesh.Clear();
					if (IsVisible) BuildMesh();
				}
				else
				{
					// If the mesh has not been initialized, do so now.
					InitMesh(parent, "Line"+(++_lineNo).ToString(), LineMaterial, LineWidth, LineDepth);
				}
				
				if (PointMaterial != null && PointWidth > 0)
				{
					if (_pointMesh != null)
					{
						_pointMesh.Clear();
						if (IsVisible) BuildPoints();
					}
					else
					{
						// If the mesh has not been initialized, do so now.
						InitPoints(parent, "Points"+(++_lineNo).ToString(), PointMaterial, PointWidth, LineDepth+1);
					}
				}
				if (NewestPointMaterial != null && NewestPointWidth > 0)
					if (_newestPointMesh != null)
					{
						_newestPointMesh.Clear();
						if (IsVisible) BuildNewestPoint();
					}
					else
					{
						// If the mesh has not been initialized, do so now.
						InitNewestPoint(parent, "NewestPoint"+(++_lineNo).ToString(), NewestPointMaterial, NewestPointWidth, LineDepth+1);
					}
					
				
			}
		}
		
		public void Reset()
		{
			lock(_points)
			{
				// If a mesh exists, clear it so we can rebuild
				if (_mesh != null)
				{
					_mesh.Clear();
				}
				// If a mesh exists, clear it so we can rebuild
				if (_pointMesh != null)
				{
					_pointMesh.Clear();
				}
				_points.Clear();
			
				// If a mesh exists, clear it so we can rebuild
				if (_newestPointMesh != null)
				{
					_newestPointMesh.Clear();
				}
				_newestPointMesh.Clear();
			}
		}
        #endregion
		
		#region Private Methods
        private void BuildMesh()
        {
			if (_points == null) return;
			lock(_points)
			{
				if (_points.Count <= 1) return;
				
				int points = _points.Count;
				int vertices = points * 2;
				int triangles = points / 2 * 6;
				if (Continuous)
				{
					vertices = (points-1)*4;
					triangles = (points-1)*6;
				}
				if (vertices >= 65535)
				{
					Debug.LogError(String.Format("SmallabLine.BuildMesh: Maximum vertex count exceeded for \"{0}\"", SmallabLineObject.name));
					return;
				}

				_lineVertices = new Vector3[vertices];
				_lineTriangles = new int[triangles];
				_lineUVs = new Vector2[vertices];

				DrawLine();
				
				int idx = 0;
				int end = points / 2;
				if (Continuous)
					end = points - 1;

				for (int i = 0; i < end; i++)
				{
					_lineUVs[idx] = new Vector2(0.0f, 1.0f);
					_lineUVs[idx + 1] = new Vector2(0.0f, 0.0f);
					_lineUVs[idx + 2] = new Vector2(1.0f, 1.0f);
					_lineUVs[idx + 3] = new Vector2(1.0f, 0.0f);					
					idx += 4;
				}

				idx = 0;
				end = points * 2;
				if (Continuous)
					end = (points-1)*4;
				
				for (int i = 0; i < end; i += 4)
				{
					_lineTriangles[idx] = i;
					_lineTriangles[idx + 1] = i + 2;
					_lineTriangles[idx + 2] = i + 1;
					_lineTriangles[idx + 3] = i + 2;
					_lineTriangles[idx + 4] = i + 3;
					_lineTriangles[idx + 5] = i + 1;
					idx += 6;
				}
				Mesh.vertices = _lineVertices;
				Mesh.uv = _lineUVs;
				Mesh.triangles = _lineTriangles;
			}
			// Clear the dirty flag
			_isDirty = false;
        }

        private void BuildPoints()
        {
			if (_points == null) return;
			lock(_points)
			{			
				int points = _points.Count;
				int vertices = points * 4;
				int triangles = points * 6;
				if (vertices >= 65535)
				{
					Debug.LogError(String.Format("SmallabLine.BuildPoints: Maximum vertex count exceeded for \"{0}\"", SmallabPointsObject.name));
					return;
				}

				_pointVertices = new Vector3[vertices];
				_pointTriangles = new int[triangles];
				_pointUVs = new Vector2[vertices];

				DrawPoints();
				
				int idx = 0;
				int end = points;
				
				for (int i = 0; i < end; i++)
				{
					_pointUVs[idx] = new Vector2(0.0f, 1.0f);
					_pointUVs[idx + 1] = new Vector2(0.0f, 0.0f);
					_pointUVs[idx + 2] = new Vector2(1.0f, 1.0f);
					_pointUVs[idx + 3] = new Vector2(1.0f, 0.0f);					
					idx += 4;
				}

				idx = 0;
				end = points * 4;
				
				for (int i = 0; i < end; i += 4)
				{
					_pointTriangles[idx] = i;
					_pointTriangles[idx + 1] = i + 2;
					_pointTriangles[idx + 2] = i + 1;
					_pointTriangles[idx + 3] = i + 2;
					_pointTriangles[idx + 4] = i + 3;
					_pointTriangles[idx + 5] = i + 1;
					idx += 6;
				}
				_pointMesh.vertices = _pointVertices;
				_pointMesh.uv = _pointUVs;
				_pointMesh.triangles = _pointTriangles;
			}
			// Clear the dirty flag
			_isDirty = false;
        }
	
		private void BuildNewestPoint(){
			if (_points == null) return;
			lock(_points)
			{			
				int points = _points.Count;
				int vertices = points * 4;
				int triangles = points * 6;
				if (vertices >= 65535)
				{
					Debug.LogError(String.Format("SmallabLine.BuildPoints: Maximum vertex count exceeded for \"{0}\"", SmallabPointsObject.name));
					return;
				}

				_newestPointVertices = new Vector3[vertices];
				_newestPointTriangles = new int[triangles];
				_newestPointUVs = new Vector2[vertices];

				DrawNewestPoint();
				
				int idx = 0;
				int end = points;
				
				for (int i = 0; i < end; i++)
				{
					_newestPointUVs[idx] = new Vector2(0.0f, 1.0f);
					_newestPointUVs[idx + 1] = new Vector2(0.0f, 0.0f);
					_newestPointUVs[idx + 2] = new Vector2(1.0f, 1.0f);
					_newestPointUVs[idx + 3] = new Vector2(1.0f, 0.0f);					
					idx += 4;
				}

				idx = 0;
				end = points * 4;
				
				for (int i = 0; i < end; i += 4)
				{
				
					_newestPointTriangles[idx] = i;
					_newestPointTriangles[idx + 1] = i + 2;
					_newestPointTriangles[idx + 2] = i + 1;
					_newestPointTriangles[idx + 3] = i + 2;
					_newestPointTriangles[idx + 4] = i + 3;
					_newestPointTriangles[idx + 5] = i + 1;
					idx += 6;
				}
				_newestPointMesh.vertices = _newestPointVertices;
				_newestPointMesh.uv = _newestPointUVs;
				_newestPointMesh.triangles = _newestPointTriangles;
			}
			// Clear the dirty flag
			_isDirty = false;
        }

		private void DrawLine()
		{
			float zPos = Screen.height / 2 + ((100.0f - LineDepth) * 0.0001f);
			int idx = 0;
			int end = _points.Count;
			
			Vector3 p1 = new Vector3(0.0f, 0.0f, 0.0f);
			Vector3 p2 = new Vector3(0.0f, 0.0f, 0.0f);
			Vector3 v1 = new Vector3(0.0f, 0.0f, 0.0f);
			Vector3 v2 = new Vector3(0.0f, 0.0f, 0.0f);
			
			int add = 2;
			if (Continuous)
			{
				add = 1;
				end--;
			}
			for (int i = 0; i < end; i += add) {
				
				p1 = _points[i];
				p2 = _points[i+1];					
				p1.z = zPos;
				if (p1.x == p2.x && p1.y == p2.y) {
					_lineVertices[idx] = _lineVertices[idx+1] = _lineVertices[idx+2] = _lineVertices[idx+3] = p1;
					idx += 4;
					continue;
				}
				p2.z = zPos;
				
				Vector3 nDist = new Vector3();
				if (!Continuous)
				{
					v2.x = p2.y; v2.y = p2.x;
					v1.x = p1.y; v1.y = p1.x;
					Vector3 dist = v2 - v1;
					nDist = dist.normalized * LineWidth;
				}
				else
				{
					// Take the cross product of (k = vector for the xy plane) x pxi + pyj + pzk
					v1 = new Vector3(0, 0, 1);
					v2 = p2 - p1;
					nDist = Vector3.Cross(v2, v1).normalized * LineWidth;
				}
				//Debug.Log("p1="+p1+" p2="+p2+" nDist="+nDist+" v1*nDist="+Vector3.Dot(v1, nDist));
				
				_lineVertices[idx]   = p1 - nDist;
				_lineVertices[idx+1] = p1 + nDist;
				_lineVertices[idx+2] = p2 - nDist;
				_lineVertices[idx+3] = p2 + nDist;

				idx += 4;
			}
			_mesh.vertices = _lineVertices;
			_mesh.RecalculateBounds();
		}
		
		private void DrawPoints()
		{
			float zPos = Screen.height / 2 + ((100.0f - (LineDepth + 1)) * 0.0001f);
			int idx = 0;
			int end = _points.Count;
			
			Vector3 p1 = new Vector3(0.0f, 0.0f, 0.0f);
			Vector3 v1 = new Vector3(0.0f, 0.0f, 0.0f);
			Vector3 v2 = new Vector3(0.0f, 0.0f, 0.0f);
			
			for (int i = 0; i < end; i++) {
				
				p1 = _points[i];
				p1.z = zPos;
				
				v1.x = v1.y = v2.y = PointWidth;
				v2.x = -PointWidth;
				
				_pointVertices[idx]   = p1 + v2;
				_pointVertices[idx+1] = p1 - v1;
				_pointVertices[idx+2] = p1 + v1;
				_pointVertices[idx+3] = p1 - v2;

				idx += 4;
			}
			_pointMesh.vertices = _pointVertices;
			_pointMesh.RecalculateBounds();
		}
	
		private void DrawNewestPoint()
		{
			float zPos = Screen.height / 2 + ((100.0f - (LineDepth + 1)) * 0.0001f);
			int idx = 0;
			int end = _points.Count;
			
			Vector3 p1 = new Vector3(0.0f, 0.0f, 0.0f);
			Vector3 v1 = new Vector3(0.0f, 0.0f, 0.0f);
			Vector3 v2 = new Vector3(0.0f, 0.0f, 0.0f);
			
			for (int i = end - 1; i < end; i++) {
				
				p1 = _points[i];
				p1.z = zPos;
				
				v1.x = v1.y = v2.y = NewestPointWidth;
				v2.x = -NewestPointWidth;
				
				_newestPointVertices[idx]   = p1 + v2;
				_newestPointVertices[idx+1] = p1 - v1;
				_newestPointVertices[idx+2] = p1 + v1;
				_newestPointVertices[idx+3] = p1 - v2;

				idx += 4;
			}
			_newestPointMesh.vertices = _newestPointVertices;
			_newestPointMesh.RecalculateBounds();
		}
		
        private void InitMesh(Transform parent, string name, Material material, float width, int depth)
        {
			LineMaterial = material;
		    LineWidth = width;
		    LineDepth = Mathf.Clamp(depth, 0, 100);
		
		    if (material == null) {
                if (_defaultMaterial == null)
                    _defaultMaterial = new Material("Shader \"Vertex Colors/Alpha\" {SubShader {Cull Off ZWrite On Blend SrcAlpha OneMinusSrcAlpha Pass {BindChannels {Bind \"Color\", color Bind \"Vertex\", vertex}}}}");

                material = _defaultMaterial;
		    }
	
		    Mesh = new Mesh();
		    Mesh.name = name;
		    _smallabLineObject = new GameObject("SmallabObject_"+parent.name+"_"+name, typeof(MeshRenderer));
            _smallabLineObject.layer = SmallabLineChart.ChartLayer;
			_smallabLineObject.transform.parent = parent;
            MeshFilter = (MeshFilter)_smallabLineObject.AddComponent(typeof(MeshFilter));
            _smallabLineObject.GetComponent<Renderer>().material = material;
		    MeshFilter.mesh = Mesh;

            BuildMesh();
	    }
		
        private void InitPoints(Transform parent, string name, Material material, float width, int depth)
        {
			PointMaterial = material;
			PointWidth = width;
		
		    if (material == null) {
                if (_defaultMaterial == null)
                    _defaultMaterial = new Material("Shader \"Vertex Colors/Alpha\" {SubShader {Cull Off ZWrite On Blend SrcAlpha OneMinusSrcAlpha Pass {BindChannels {Bind \"Color\", color Bind \"Vertex\", vertex}}}}");

                material = _defaultMaterial;
		    }
	
		    _pointMesh = new Mesh();
		    _pointMesh.name = name;
			_smallabPointsObject = new GameObject("SmallabObject_"+parent.name+"_"+name, typeof(MeshRenderer));
			_smallabPointsObject.layer = SmallabLineChart.ChartLayer;
			_smallabPointsObject.transform.parent = parent;
            _pointMeshFilter = (MeshFilter)_smallabPointsObject.AddComponent(typeof(MeshFilter));
            _smallabPointsObject.GetComponent<Renderer>().material = material;
		    _pointMeshFilter.mesh = _pointMesh;

            BuildPoints();
	    }
	
		private void InitNewestPoint(Transform parent, string name, Material material, float width, int depth)
        {
			NewestPointMaterial = material;
			NewestPointWidth = width;
		
		    if (material == null) {
                if (_defaultMaterial == null)
                    _defaultMaterial = new Material("Shader \"Vertex Colors/Alpha\" {SubShader {Cull Off ZWrite On Blend SrcAlpha OneMinusSrcAlpha Pass {BindChannels {Bind \"Color\", color Bind \"Vertex\", vertex}}}}");

                material = _defaultMaterial;
		    }
	
		    _newestPointMesh = new Mesh();
		    _newestPointMesh.name = name;
			_smallabNewestPointObject = new GameObject("SmallabObject_"+parent.name+"_"+name, typeof(MeshRenderer));
			_smallabNewestPointObject.layer = SmallabLineChart.ChartLayer;
			_smallabNewestPointObject.transform.parent = parent;
            _newestPointMeshFilter = (MeshFilter)_smallabNewestPointObject.AddComponent(typeof(MeshFilter));
            _smallabNewestPointObject.GetComponent<Renderer>().material = material;
		    _newestPointMeshFilter.mesh = _newestPointMesh;

            BuildNewestPoint();
	    }	
		#endregion
}
