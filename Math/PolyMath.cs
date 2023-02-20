using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System;
using Random = System.Random; 
//using ClipperLib;
static public class PolyMath
{

	public static void XZToMesh(Mesh mesh, List<Vector2> verts, float density)
	{
		density = Mathf.Clamp(density, 1f , float.MaxValue);

		var sortedList = new List<Vector2>(verts);
		sortedList.Sort((v1, v2) => v1.y > v2.y ? 1 : 0);

		foreach (var v in sortedList)
			Debug.Log(v);
	}

	//internal static void XZToMesh(Mesh mesh, List<Vector2> verts, Vector2 centroid)
	//{
	//	var meshVerts = new Vector3[verts.Count + 1];

	//	for (int i = 0; i < verts.Count; i++)
	//		meshVerts[i] = verts[i].XYZ();

	//	int centroidIndex = meshVerts.Length - 1;
	//	meshVerts[centroidIndex] = centroid;

	//	int vertsCount = verts.Count;
	//	int[] tris = new int[vertsCount * 3];
	//	for (int i = 0; i < vertsCount ; i++)
	//	{
	//		tris[i] = i;
	//		if (i + 1 < vertsCount) 
	//			tris[i + 1] = centroidIndex;
	//		if(i + 2 < vertsCount)
	//			tris[i + 2] = i + 1;
	//	}

	//	var meshNormals = Generators.MeshGen.MeshData.GenerateNormals(meshVerts, tris);

	//	mesh.SetVertices(meshVerts);
	//	mesh.SetTriangles(tris, 0);
	//	mesh.SetNormals(meshNormals);
	//}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static List<Vector2> ClipperToVerts(List<IntPoint> points)
	//{
	//	var verts = new List<Vector2>();
	//	int count = points.Count;
	//	for (int i = 0; i < count; i++)
	//		verts.Add(new Vector2(points[i].X, points[i].Y));
	//	return verts;
	//}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static List<IntPoint> VertsToClipper(List<Vector2> verts)
	//{
	//	var points = new List<IntPoint>();
	//	int count = verts.Count;
	//	for (int i = 0; i < count; i++)
	//		points.Add(new IntPoint(verts[i].x, verts[i].y));
	//	return points;
	//}


	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static List<Vector2> RandomPolyByDegrees(Vector2 center, Random rnd, Vector2 minMaxSize, Vector2 minMaxDegreesPerVert, int maxVerts = 300)
	//{
	//	if (minMaxDegreesPerVert.y < 0.5f)
	//	{
	//		Debug.LogError("Minimum degrees are too low");
	//		throw new Exception();
	//	}

	//	var verts = new List<Vector2>();
	//	var normal = rnd.NextNormalV2();
	//	var totalDegrees = 0f;

	//	var watch = new System.Diagnostics.Stopwatch();
	//	watch.Start();
	//	do
	//	{
	//		float magnitude = rnd.NextFloat(minMaxSize.x, minMaxSize.y);
	//		float degrees = rnd.NextFloat(minMaxDegreesPerVert.x, minMaxDegreesPerVert.y);
	//		normal = normal.Rotate(degrees);
	//		verts.Add(center + normal * magnitude);
	//		totalDegrees += degrees;
	//	} while (totalDegrees < 360f && verts.Count < maxVerts && watch.ElapsedMilliseconds < 20000);

	//	if(watch.ElapsedMilliseconds >= 20000)
	//	{
	//		Debug.LogError("WOWOWOWOWW");
	//	}
	//	return verts;
	//}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static List<Vector2> ResizePoly(List<Vector2> poly, Vector2 center, float scale)
	//{
	//	int count = poly.Count;
	//	var resizedPoly = new List<Vector2>(poly.Count);
	//	for (int i = 0; i < count; i++)
	//		resizedPoly.Add(Vector2.LerpUnclamped(center, poly[i], scale));
	//	return resizedPoly;
	//}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static List<Vector2> ResizePoly(List<Vector2> poly, float scale)
	//{
	//	int count = poly.Count;
	//	var resizedPoly = new List<Vector2>(poly.Count);
	//	var center = GetCentroid(poly);
	//	for (int i = 0; i < count; i++)
	//		resizedPoly.Add(Vector2.LerpUnclamped(center, poly[i], scale));
	//	return resizedPoly;
	//}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static List<Vector2> ResizePoly(Vector2[] poly, float scale)
	//{
	//	int count = poly.Length;
	//	var resizedPoly = new List<Vector2>(poly.Length);
	//	var center = GetCentroid(poly);
	//	for (int i = 0; i < count; i++)
	//		resizedPoly.Add(Vector2.LerpUnclamped(center, poly[i], scale));
	//	return resizedPoly;
	//}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static Vector2 GetCentroid(Vector2[] poly)
	//{
	//	float accumulatedArea = 0.0f;
	//	float centerX = 0.0f;
	//	float centerY = 0.0f;
	//	for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
	//	{
	//		float temp = poly[i].x * poly[j].y - poly[j].x * poly[i].y;
	//		accumulatedArea += temp;
	//		centerX += (poly[i].x + poly[j].x) * temp;
	//		centerY += (poly[i].y + poly[j].y) * temp;
	//	}

	//	if (Math.Abs(accumulatedArea) < 1E-7f)
	//		return Vector2.zero;  // Avoid division by zero

	//	accumulatedArea *= 3f;
	//	return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
	//}
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static Vector2 GetCentroid(List<Vector2> poly)
	//{
	//	float accumulatedArea = 0.0f;
	//	float centerX = 0.0f;
	//	float centerY = 0.0f;
	//	for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
	//	{
	//		float temp = poly[i].x * poly[j].y - poly[j].x * poly[i].y;
	//		accumulatedArea += temp;
	//		centerX += (poly[i].x + poly[j].x) * temp;
	//		centerY += (poly[i].y + poly[j].y) * temp;
	//	}

	//	if (Math.Abs(accumulatedArea) < 1E-7f)
	//		return Vector2.zero;  // Avoid division by zero

	//	accumulatedArea *= 3f;
	//	return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
	//}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static bool IsPointInPolygon(List<Vector2> polygon, Vector2 point)
	//{
	//	bool result = false;
	//	int j = polygon.Count - 1;
	//	for (int i = 0; i < polygon.Count; i++)
	//	{
	//		if (polygon[i].y < point.y && polygon[j].y >= point.y || polygon[j].y < point.y && polygon[i].y >= point.y)
	//			if (polygon[i].x + (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < point.x)
	//				result = !result;
	//		j = i;
	//	}
	//	return result;
	//}


}
