using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshLib;
using NoiseLib;

// Controls basically everything
public class TerrainCreator : MonoBehaviour
{

	// basic options

	[Range (128, 1024)]
	public int resolution = 256;

	public int scale = 10;

	// noise opts / ratios
	public float value = 0.25f;
	public float perlin = 0.25f;
	public float exp = 0.25f;
	public float voro = 0.25f;

	[Range (1, 8)]
	public int octaves = 1;

	[Range (2, 10)]
	public int frequency = 3;

	// island opts

	public NoiseLib.MappableTypes mappableType;

	public NoiseLib.OtherTypes otherType; 

	[Range (0f, 1f)]
	public float islandDensity = 0.2f;

	public float avgHeight = 2;


	// texture opts
	public Gradient coloring;

	// -------------------------------------
	// Private variables
	private int curRes = -1;
	private int curScale;
	private Mesh m = null;


	private Vector3[] verts;
	private Vector3[] norms;
	private Color[] colors;

	private void init () {
		if (m == null) {
			m = new Mesh (); 
			m.name = "Test mesh"; 
			GetComponent<MeshFilter> ().mesh = m;
			Debug.Log ("Assigned mesh");
		} 

	}

	private void updateParams() {
		if (resolution != curRes) {
			createNewMesh (resolution);
			curRes = resolution; 
		} 

		if (scale != curScale) {
			GetComponent<Transform> ().localScale = new Vector3 (scale, scale, scale);
			curScale = scale;
		}
	}
		
	void onEnable ()
	{
		Debug.Log ("In enable.");
		RenderTerrain ();
	}

	public void RenderTerrain() {
		init ();
		updateParams(); 

		// get constants 
		Vector2[] vecs = MeshUtil.Constants.UnitVectors2D;

		// extract parameters 
		IHeightMappable<Vector2> cl = NoiseLib.Constants.MappableClasses [(int) mappableType];

		// call the methods. 
		int v = 0; 
		float dx = 1 / resolution;

		for (int i = 0; i <= resolution; i++) {
			Vector2 p0 = Vector2.Lerp (vecs [0], vecs [2], i * dx); 
			Vector2 p1 = Vector2.Lerp (vecs [1], vecs [3], i * dx); 

			for (int j = 0; j <= resolution; j++) {
				Vector2 p = Vector2.Lerp (p0, p1, j * dx);
				float height = cl.noise(p);

				Debug.Log (height);
				verts[v].y = height;
				colors[v] = coloring.Evaluate(height);
			}
		}

		setMesh(verts, colors); 

	}

	// creates new mesh (w triangles, etc) given the new resolution 
	private void createNewMesh(int resolution) {
		m.Clear (); 
		float u = 1f / resolution;

		int numVerts = (resolution + 1) * (resolution + 1);

		verts = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		norms = new Vector3[numVerts]; 
		colors = new Color[numVerts];

		int v = 0; // vertex enumerator 
		for (int i = 0; i <= resolution; i++) {
			for (int j = 0; j <= resolution; j++) {
				verts [v] = new Vector3 (j * u - 0.5f, i * u - 0.5f); 
				norms [v] = Vector3.up;
				colors [v] = Color.blue; 

				v++;
			}
		}

		Debug.Log ("Created mesh.");
			
		setMesh (verts, colors, norms, uv);
		m.triangles = MeshLib.MeshUtil.GetTriangles(resolution);

	}

	// all-in one operation 
	private void setMesh(Vector3[] vertices, Color[] colors, Vector3[] normals = null, Vector2[] uv = null ) {
		m.vertices = vertices;
		m.colors = colors;

		if (normals != null) {
			m.normals = normals; 
		} else {
			m.RecalculateNormals (); 
		};

		if (uv != null) {
			m.uv = uv;
		}
	}
}
