
#if UNITY_EDITOR  
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
static public class GizmosU
{
	static public void DrawTriangles(Vector3[] verts, int[] tris, Transform tf = null)
	{
		for (int i = 0; i < tris.Length; i += 3)
		{
			DrawTriangle( verts[tris[i]], verts[tris[i + 1]], verts[tris[i + 2]] , tf);
		}

	}
	static public void DrawTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Transform tf = null)
	{
		if (tf)
		{
			p1 = tf.TransformPoint(p1);
			p2 = tf.TransformPoint(p2);
			p3 = tf.TransformPoint(p3);
		}
		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p2);
		Gizmos.DrawLine(p3, p1);
	}
	internal static void DrawPolygonXZ(List<Vector2> verts)
	{
		for (int i = 0; i < verts.Count - 1; i++)
			Gizmos.DrawLine(verts[i].XZToXYZ(), verts[i + 1].XZToXYZ());
		if (verts.Count > 0)
			Gizmos.DrawLine(verts[verts.Count - 1].XZToXYZ(), verts[0].XZToXYZ());
	}
	internal static void LabelPolygonXZ(List<Vector2> verts)
	{
		for (int i = 0; i < verts.Count; i++)
			Handles.Label(verts[i].XZToXYZ(), verts[i].Vector2Hash());
	}
	internal static void DrawQuadXZ(Vector2 center, float size)
	{
		Gizmos.DrawWireCube(center.XZToXYZ(), new Vector3(size, 0, size));
	}

	internal static void DrawMesh(Vector3[] vertices, int[] triangles, Matrix4x4 transform, float drawSize)
	{
		for (int i = 0; i + 3 < triangles.Length; i += 3)
		{
			Gizmos.DrawLine(transform.MultiplyPoint(vertices[triangles[i]]), transform.MultiplyPoint(vertices[triangles[i + 1]]));
			Gizmos.DrawLine(transform.MultiplyPoint(vertices[triangles[i + 1]]), transform.MultiplyPoint(vertices[triangles[i + 2]]));
			Gizmos.DrawLine(transform.MultiplyPoint(vertices[triangles[i + 2]]), transform.MultiplyPoint(vertices[triangles[i]]));
		}
		int vertLen = vertices.Length;
		for (int i = 0; i < vertLen; i++)
		{
			Gizmos.DrawSphere(transform.MultiplyPoint(vertices[i]), drawSize);
			UnityEditor.Handles.Label(transform.MultiplyPoint(vertices[i]), i.ToString());
		}
	}

	public static void DrawBounds(Bounds bounds)
	{
		Gizmos.DrawWireCube(bounds.center, bounds.size);
	}


	public static void DrawMesh(Mesh mesh, Transform tf, bool debugVertices, bool debugTris, bool debugNormals)
	{
		var verts = mesh.vertices;
		var normals = mesh.normals;
		var tris = mesh.triangles;
		if (debugVertices)
			DrawVertices(verts, tris , tf);
		if (debugTris)
			DrawTriangles(verts, tris , tf);
		if (debugNormals)
			DrawNormals(verts, normals, tf);
	}


	public static void DrawVertices(Vector3[] vertices, int[] tris , Transform tf)
	{
		for (int i = 0; i < tris.Length; i++)
		{
			var global = tf.TransformPoint(vertices[tris[i]]);
			Gizmos.DrawWireSphere(global, 0.3f);
			Handles.Label(global + new Vector3(0,1,0), tris[i].ToString());
		}
	}
	public static void DrawNormals(Mesh mesh , Transform tf) => DrawNormals(mesh.vertices, mesh.normals , tf);

	private static void DrawNormals(Vector3[] vertices, Vector3[] normals , Transform tf)
	{
		Handles.color = Color.green;
		for (int i = 0; i < vertices.Length; i++)
			Handles.ArrowHandleCap(0, tf.TransformPoint( vertices[i]), Quaternion.Euler(tf.TransformDirection(normals[i]) ) , 0.67f, EventType.Repaint);  
	}
	 
}
#endif
