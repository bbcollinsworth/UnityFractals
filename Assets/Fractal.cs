using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Fractal : MonoBehaviour
{
	public bool useDirection = false;
	//public Vector3 start;
	//public Vector3 endOrDirection;
	public Vector3 startDirection = Vector3.up;
	[Range (0, 180)]
	public float angle = 90;
	[Range (0.1f, 1f)]
	public float minSize = 0.1f;
	[Range (0, 1)]
	public float lengthRandomness = 0;
	[Range (0, 1)]
	public float angleRandomness = 0;
	public bool reroll = false;
	[Space]
	public Mesh meshToDraw = null;
	public Material mat = null;
	public bool wind = true;
	public Transform windSource = null;
	[Range (0, 2)]
	public float windStrength = 2f;

	private bool hasWindSource = false;
	private Vector3 windForce = Vector3.zero;

	private int seed = 0;
	[SerializeField,HideInInspector]
	private float[] seeds = new float[1000];
	private int branchIndex = 0;

	ArrayList branches;

	private struct BranchData
	{
		public Vector3 start;
		public Vector3 direction;
		public Quaternion rotation;
		public Vector3 scale;

		public BranchData (Vector3 strt, Vector3 dir, Quaternion rot, Vector3 sca)
		{
			start = strt;
			direction = dir;
			rotation = rot;
			scale = sca;
		}
		/*public float lengthRandom { get; private set; }

		public float rotRandom { get; private set; }

		public float length { get; private set; }

		public Vector3 start { get; private set; }

		public Vector3 normal { get; private set; }

		public float angle { get; private set; }

		public Quaternion rotation { get { return Quaternion.AngleAxis (angle, normal); } }

		public Vector3 scale {
			get {
				var rad = length * 0.1f;
				return new Vector3 (rad, length * 0.5f, rad);
			}
		}

		public Vector3 direction { get { return rotation * (start * (length + length * lengthRandom)); } }

		public Matrix4x4 tMatrix { get { return Matrix4x4.TRS (start + (direction * 0.5f), rotation, scale); } }

		public BranchData (Vector3 strt, float len, Vector3 norm, float lRand, float rRand)
		{
			lengthRandom = lRand;
			rotRandom = rRand;
			start = strt;
			length = len;
			normal = norm;
			//scale = sca;
		}*/
	}

	void OnEnable ()
	{
		//InitRandoms ();
		//Random.InitState (255);

		//Random.state = 
	}


	void Update ()
	{
		if (reroll) {
			reroll = false;
			InitRandoms ();
			//seed = Mathf.FloorToInt (Random.value * 1000);
		}


		if (branches == null)
			branches = new ArrayList ();
		//	return;
		//if ()
		branches.Clear ();
		branchIndex = 0;
		//Random.InitState (seed);

		windForce = windSource.forward * windStrength * Mathf.PerlinNoise (Time.time, 0);

		Branch (transform.position, 2, Quaternion.LookRotation (Vector3.forward, Vector3.up));

		//if (windSource != null) {
		//	windForce = windSource.forward * windStrength * Mathf.PerlinNoise (Time.time, 0);
		//}

		for (int i = 0; i < branches.Count; ++i) {
			BranchData tData = (BranchData)branches [i];
			Matrix4x4 m = Matrix4x4.TRS (tData.start + tData.direction * 0.5f, tData.rotation, tData.scale);
			Graphics.DrawMesh (meshToDraw, m, mat, 0);
		}

		/*Matrix4x4[] allMatrices = new Matrix4x4[branches.Count];
		for (int i = 0; i < allMatrices.Length; ++i) {
			//allMatrices [i] = (Matrix4x4)branches [i];
			BranchData tData = (BranchData)branches [i];
			//Vector3 newVector = tData.direction + windForce;
			//Quaternion windRot = Quaternion.FromToRotation (tData.direction, newVector);
			//Vector3 newDir = windRot * tData.direction;

			allMatrices [i] = Matrix4x4.TRS (tData.start, tData.rotation, tData.scale);
			//allMatrices [i] = Matrix4x4.TRS (tData.start + newDir * 0.5f, windRot * tData.rotation, tData.scale);


		}*/

		//Matrix4x4[] allMatrices = branches.ToArray () as Matrix4x4[];
		//Graphics.DrawMeshInstanced (meshToDraw, 0, mat, allMatrices);

	}

	void InitRandoms ()
	{
		for (int i = 0; i < seeds.Length; ++i) {
			seeds [i] = Random.value;
		}
	}

	void OnGui ()
	{

	}

	/*void OnDrawGizmos ()
	{
		
		branches = new ArrayList ();
		//Branch (transform.position, 2, Quaternion.LookRotation (Vector3.forward, Vector3.up));
	}*/

	/*Quaternion AddWind (Quaternion rot)
	{
		if (!wind)
			return rot;


	}*/

	void Branch (Vector3 start, float length, Quaternion rot)
	{
		branchIndex = (branchIndex + 3) % seeds.Length;
		float lRand = (seeds [branchIndex] * 2 - 1) * lengthRandomness;
		length = length + length * lRand;

		var direction = rot * (startDirection * length);

		if (windSource != null) {

			//Vector3 

			Vector3 newVector = direction + windForce;
			Quaternion windRot = Quaternion.FromToRotation (direction, newVector);
			direction = windRot * direction;
			rot = windRot * rot;

		}

		var end = start + direction;
		//DrawVector (start, direction, Color.white);
		var rad = length * 0.1f;
		Vector3 scale = new Vector3 (rad, length * 0.5f, rad);
		//Matrix4x4 tMatrix = Matrix4x4.TRS (start + (direction * 0.5f), rot, scale);

		//branches.Add (tMatrix);
		branches.Add (new BranchData (start, direction, rot, scale));

		if (length > minSize) {
			if (start == Vector3.zero)
				start = transform.right;
			
			Vector3 directionNormal = Vector3.Cross (direction, start).normalized;
			directionNormal = Quaternion.AngleAxis (GetRandomAngle (true), direction) * directionNormal;
			//DrawVector (end, directionNormal, Color.red);
			Branch (end, length * 0.8f, Quaternion.AngleAxis (GetRandomAngle () * 0.5f, directionNormal) * rot);
			Branch (end, length * 0.8f, Quaternion.AngleAxis (GetRandomAngle () * -0.5f, directionNormal) * rot);
		}
	}

	float GetRandomAngle (bool randomSign = false)
	{
		var randSign = randomSign ? seeds [(branchIndex + 1) % seeds.Length] * 2 - 1 : 1;
		return angle * (1 - seeds [(branchIndex + 2) % seeds.Length] * angleRandomness * Mathf.Sign (randSign));
	}

	void DrawVector (Vector3 start, Vector3 end, Color color)
	{
		Gizmos.color = color;

		if (!useDirection) {
			Gizmos.DrawLine (start, end);
			Gizmos.DrawWireSphere (end, 0.1f);
			return;
		}

		Gizmos.DrawLine (start, start + end);
		var radius = end.magnitude * 0.1f;
		Gizmos.DrawWireSphere (start + end, radius);
	}
		
}
