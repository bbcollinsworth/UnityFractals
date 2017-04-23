using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Fractal : MonoBehaviour
{
	public bool useDirection = false;
	//public Vector3 start;
	//public Vector3 endOrDirection;
	[Range (0, 180)]
	public float angle = 90;
	public float minSize = 0.1f;
	[Range (0, 1)]
	public float lengthRandomness = 0;
	[Range (0, 1)]
	public float angleRandomness = 0;

	[Space]
	public Mesh meshToDraw = null;
	public Material mat;

	ArrayList branches;


	void Update ()
	{
		if (branches == null)
			return;

		Matrix4x4[] allMatrices = new Matrix4x4[branches.Count];
		for (int i = 0; i < allMatrices.Length; ++i) {
			allMatrices [i] = (Matrix4x4)branches [i];
		}

		//Matrix4x4[] allMatrices = branches.ToArray () as Matrix4x4[];
		Graphics.DrawMeshInstanced (meshToDraw, 0, mat, allMatrices);
	}

	void OnDrawGizmos ()
	{
		branches = new ArrayList ();

		Branch (transform.position, 2, Quaternion.LookRotation (Vector3.forward, Vector3.up));

	}

	void Branch (Vector3 start, float length, Quaternion rot)
	{

		float lRand = (Random.value * 2 - 1) * lengthRandomness;
		length = length + length * lRand;
		var direction = rot * (Vector3.up * length);
		var end = start + direction;
		//DrawVector (start, direction, Color.white);
		var rad = length * 0.1f;
		Vector3 scale = new Vector3 (rad, length * 0.5f, rad);
		Matrix4x4 tMatrix = Matrix4x4.TRS (start + (direction * 0.5f), rot, scale);

		branches.Add (tMatrix);
		//Graphics.DrawMesh (meshToDraw, tMatrix, mat, 0);
		//Matrix4x4[]
		//m = new Matrix4x4[]{ tMatrix };
		//Graphics.DrawMeshInstanced (meshToDraw, 0, mat, m);

		if (length > minSize) {
			if (start == Vector3.zero)
				start = transform.right;
			
			Vector3 directionNormal = Vector3.Cross (direction, start).normalized;
			directionNormal = Quaternion.AngleAxis (angle, direction) * directionNormal;
			//DrawVector (end, directionNormal, Color.red);
			Branch (end, length * 0.8f, Quaternion.AngleAxis (angle * 0.5f, directionNormal) * rot);
			Branch (end, length * 0.8f, Quaternion.AngleAxis (angle * -0.5f, directionNormal) * rot);
		}
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
