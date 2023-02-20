﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;  
using Random = System.Random; 
using System.Linq;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object; 

static public class MathU
{
    static readonly Matrix4x4 MatOne = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
    static public readonly Vector3 ZeroVector = new();

    const int INLINED = (int) MethodImplOptions.AggressiveInlining;

    #region Random
    static Random _pureRandom = new Random();
    static int randomUsages;
    public static int PureRandom
    {
        get 
        {
            lock (_pureRandom)
            {
                randomUsages++;
                if (randomUsages >= 20)
                    _pureRandom = new();
                return _pureRandom.Next();
            }
        }
    } 
    #endregion

    #region Floats & Ints

    public static bool IsAlmostZero(this float number)
    {
        return Math.Abs(number) <= 0.00000001f;
    }

    public static bool IsAlmostEqualTo(this float numberA, float numberB, float tolerance = /* SmallNumber */ 0.00000001f)
    {
        return Math.Abs(numberA - numberB) <= tolerance;
    }

    static public float TimeLerp(float lerpAmount, float timeStarted)
    {
        return Mathf.Clamp01(Mathf.InverseLerp(timeStarted + lerpAmount, timeStarted, Time.time));
    } 

    static public void Clamp(this int[] arr, int min, int max)
    {
        for (int i = 0; i < arr.Length; i++)
            arr[i] = Mathf.Clamp(arr[i], min, max);
    }
    static public void SafeDestroyChildren(this Transform tf)
    {
        //done this way to fix infinite looping when destroying transform is invalid
        var count = tf.childCount; 
		var list = new List<Transform>(count);
        for (int i = 0; i < tf.childCount; i++) 
            list.Add(tf.GetChild(i));

        foreach (var child in list)
            child.SafeDestroy();
    }

    static public bool IsLowerAndNotApprox(float f1, float f2)
       => (!Mathf.Approximately(f1, f1)) && f1 < f2;
    static public bool IsHigherAndNotApprox(float f1, float f2)
       => (!Mathf.Approximately(f1, f1)) && f1 > f2;
    internal static List<T> GetEnumValues<T>() where T : Enum
    {
        var type = typeof(T);
        var fields = type.GetFields((System.Reflection.BindingFlags)(-1));
        var list = new List<T>();
        foreach (var field in fields)
            if (field.IsLiteral)
                list.Add((T)field.GetValue(null));
        return list;
    }

    public static Vector2 ClosestCircleEdge(Vector2 circleCenter, float radius, Vector2 point)
    {   
        Vector2 direction = point - circleCenter; // vector pointing from center of circle to point
        direction.Normalize(); // normalize to get unit vector pointing towards point
        return circleCenter + (direction * radius); // point on edge of circle closest to point
    }

    internal static List<Vector2> ResizePolygon(List<Vector2> verts, Vector2 center, float scale)
    {
        var resized = new List<Vector2>(verts.Count);
        var count = resized.Count;
        for (int i = 0; i < count; i++)
            resized[i] = Vector2.Lerp(resized[i], center, scale);

        return resized;
    }

    [MethodImpl(INLINED)]
    public static bool IsValidMinMax(Vector2Int MinMax, float value)
    {
        return value >= MinMax.x && value <= MinMax.y;
    }


    [MethodImpl(INLINED)]
    public static bool ValidMinMax(Vector2Int MinMax, int value)
    {
        return value >= MinMax.x && value <= MinMax.y;
    }

    [MethodImpl(INLINED)]
    public static bool IsValidMinMax(Vector2 MinMax, float value)
	{ 
        return value >= MinMax.x &&  value <= MinMax.y;
	}

    [MethodImpl(INLINED)]
    static public Vector3 ToV3(this Color color)
        => new Vector3()
        {
            x = color.r,
            y = color.g,
            z = color.b
        };

    [MethodImpl(INLINED)]
    static public float getVFromHSV(this Color color)
    {
        Color.RGBToHSV(color, out _, out _, out float v);
        return v;
    }
    [MethodImpl(INLINED)]
    static public float getS(this Color color)
    {
        Color.RGBToHSV(color, out _, out float s, out _);
        return s;
    }
    [MethodImpl(INLINED)]
    static public float getH(this Color color)
    {
        Color.RGBToHSV(color, out float h, out _, out _);
        return h;
    }
    [MethodImpl(INLINED)]
    static public Color OppositeColor(this Color color)
    { 
        return new Color(1-color.r, 1-color.g, 1-color.b, color.a); 
    }
    [MethodImpl(INLINED)] 
    static public Vector2 ToFloat(this Vector2Int vec2Int) => new Vector2(vec2Int.x, vec2Int.y);
    [MethodImpl(INLINED)] 
    static public Vector2Int RoundToInt(this Vector2 vec2) => new Vector2Int() { x = Mathf.RoundToInt(vec2.x), y = Mathf.RoundToInt(vec2.y) };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cross(this Vector2 v1, Vector2 v2)
    {
        return v1.x * v2.y
               - v1.y * v2.x;
    }
    [MethodImpl(INLINED)]
    static public Vector2Int Round(this Vector2 v)
        => new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    [MethodImpl(INLINED)]
    static public void AddOrSet<Key, Val>(this Dictionary<Key, Val> dic, Key key, Val value)
	{
        if (dic.ContainsKey(key))
            dic[key] = value;
        else dic.Add(key, value);
	} 
    [MethodImpl(INLINED)]
    static public Val TryGetOrAddDefault<Key, Val>(this Dictionary<Key, Val> dic, Key key, Val defaultVal)
    {
        if (dic.TryGetValue(key, out var value))
            return value;
        value = defaultVal;
        dic.Add(key, value);
        return value;
    }
    [MethodImpl(INLINED)]
    public static int Round(this float f) => Mathf.RoundToInt(f);
    [MethodImpl(INLINED)]
    static public void SetZero(this Vector3 v) { v.x = 0f; v.y = 0f; v.z = 0f; }
    [MethodImpl(INLINED)]
    static public bool IsZero(this Vector3 v) => v.x == 0f && v.y == 0f && v.z == 0f;
    [MethodImpl(INLINED)]
    static public bool IsNanOrInifinity(this Vector3 v)
        => float.IsNaN(v.x) || float.IsInfinity(v.x)
        || float.IsNaN(v.y) || float.IsInfinity(v.y)
        || float.IsNaN(v.z) || float.IsInfinity(v.z);
    [MethodImpl(INLINED)]
    static public bool IsNanOrInifinity(this Vector2 v)
        => float.IsNaN(v.x) || float.IsInfinity(v.x)
        || float.IsNaN(v.y) || float.IsInfinity(v.y);

    [MethodImpl(INLINED)]
    public static float Remap(this float val, float fromMin, float fromMax, float toMin, float toMax)
    {
        if (fromMax - fromMin == 0)
            fromMax = 0.0000001f;
        return (toMax - toMin) * (val - fromMin) / (fromMax - fromMin) + toMin;
    } 


    [MethodImpl(INLINED)]
    public static float Remap(this int val, float fromMin, float fromMax, float toMin, float toMax)
    {
        if (fromMax - fromMin == 0)
            fromMax = 0.0000001f;
        return (toMax - toMin) * (val - fromMin) / (fromMax - fromMin) + toMin;
    }
    [MethodImpl(INLINED)]
    public static float RemapTo01(this float val, float fromMin, float fromMax)
    {
        if (fromMax - fromMin == 0)
            fromMax = 0.000001f;
        return (val - fromMin) / (fromMax - fromMin);
    }


    #endregion

    #region Vectors
      
    static public Color ToColor(this Vector3 v) => new Color(v.x, v.y, v.z);
    [MethodImpl(INLINED)]
    static public bool Approximately(Vector2 v1, Vector2 v2)
   => Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y);

    [MethodImpl(INLINED)] 
    static public Vector2 ClampValues(this Vector2 toClamp, float maxX, float maxY)
    => new Vector2
        (
           Mathf.Clamp(toClamp.x, 0, maxX),
           Mathf.Clamp(toClamp.y, 0, maxY)
        ); 
    [MethodImpl(INLINED)]
     
    static public Vector2 ClampMinValues(this Vector2 toClamp, Vector2 minClamp)
       => new Vector2()
       {
           x = Mathf.Clamp(toClamp.x, minClamp.x, float.MaxValue),
           y = Mathf.Clamp(toClamp.y, minClamp.y, float.MaxValue),
       };
    static public Vector2 ClampedMinMaxValues(this Vector2 toClamp, Vector2 minClamp, Vector2 maxClamp)
        => new Vector2()
        {
            x = Mathf.Clamp(toClamp.x, minClamp.x, maxClamp.x),
            y = Mathf.Clamp(toClamp.y, minClamp.y, maxClamp.y),
        };
    static public Vector3 ClampMinValues(this  Vector3 toClamp, Vector3 minClamp)
        => ClampV3(toClamp, minClamp, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
    static public Vector3 ClampV3(this Vector3 toClamp, Vector3 minClamp, Vector3 maxClamp)
    => new Vector3()
    {
        x = Mathf.Clamp(toClamp.x, minClamp.x, maxClamp.x),
        y = Mathf.Clamp(toClamp.y, minClamp.y, maxClamp.y),
        z = Mathf.Clamp(toClamp.z, minClamp.z, maxClamp.z)
    };
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    static public Rect GetNearestRect(Vector2 posXZ, float cellSize, Vector2 cellCenter)
    {
        var rightDist = Mathf.Abs(posXZ.x - cellCenter.x);
        var topDis = Mathf.Abs(posXZ.y - cellCenter.y);
        Rect neighborRect = new() { size = new Vector2(cellSize, cellSize) , center = cellCenter };
        if (rightDist > topDis)//is right or top
        {
            if (posXZ.x > cellCenter.x) // right
                neighborRect.x += cellSize;
            else //left
                neighborRect.x -= cellSize; 
        }
        else
        {
            if (posXZ.y > cellCenter.y)//top
                neighborRect.y += cellSize;
            else//bottom
                neighborRect.y -= cellSize; 
        }
        return neighborRect;
    }
    static public Vector2 TopCenter(this Rect rect) => rect.center + new Vector2(0, +rect.size.y / 2f);
    static public Vector2 BottomCenter(this Rect rect) => rect.center + new Vector2(0, -rect.size.y / 2f);

    [MethodImpl(INLINED)]
    public static Vector2 Abs(this Vector2 v) => new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
	[MethodImpl(INLINED)]
	public static Vector2 XZ(this Vector3Int vv) => new Vector2(vv.x, vv.z);
	[MethodImpl(INLINED)]
	public static Vector2Int XZRoundToInt(this Vector3 vv) => new Vector2Int( Mathf.RoundToInt( vv.x) , Mathf.RoundToInt(vv.z));
	[MethodImpl(INLINED)]
	public static Vector2Int XZInt(this Vector3Int vv) => new Vector2Int(vv.x, vv.z); 

    [MethodImpl(INLINED)]
    public static Vector2 XZ(this Vector3 vv) => new Vector2(vv.x, vv.z);
	[MethodImpl(INLINED)]
	public static Vector3 XZToXYZ(this Vector2 v2) => new Vector3(v2.x, 0, v2.y);

	[MethodImpl(INLINED)]
    public static Vector3Int XZToXYZInt(this Vector2Int v2) => new Vector3Int(v2.x, 0, v2.y);
	[MethodImpl(INLINED)] 
	public static Vector3 XZToXYZ(this Vector2Int v2) => new Vector3(v2.x, 0, v2.y);
	[MethodImpl(INLINED)]
    public static Vector3 XZToXYZ(this Vector2 v2, float Y) => new Vector3(v2.x, Y, v2.y); 
    [MethodImpl(INLINED)]
    public static string Vector2Hash(this Vector2 hash) => $"{hash.x},{hash.y}";
    [MethodImpl(INLINED)] 
    public static Vector3 ToV3(this Vector2 v2) => new Vector3(v2.x,v2.y,0);
    [MethodImpl(INLINED)]
    static public Vector3 Abs(this Vector3 v) => new Vector3() { x = Mathf.Abs(v.x), y = Mathf.Abs(v.x), z = Mathf.Abs(v.x) };



    [MethodImpl(INLINED)]
    public static Vector2 RadianToVector2(float radian) => new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

    [MethodImpl(INLINED)]
    public static Vector2 DegreeToV2(float degree) => RadianToVector2(degree * Mathf.Deg2Rad);

   

    public static Vector2 NearestEdge(Rect rect, Vector2 point)
    {
        Vector2 nearestEdgePoint = new Vector2();

        // calculate the distance to each edge of the square
        float topDistance = Math.Abs(point.y - rect.yMax);
        float bottomDistance = Math.Abs(point.y - rect.yMin);
        float leftDistance = Math.Abs(point.x - rect.xMin);
        float rightDistance = Math.Abs(point.x - rect.xMax);

        // determine the nearest edge
        float minDistance = Math.Min(topDistance, Math.Min(bottomDistance, Math.Min(leftDistance, rightDistance)));

        if (minDistance == topDistance)
        {
            nearestEdgePoint.x = point.x;
            nearestEdgePoint.y = rect.yMax;
        }
        else if (minDistance == bottomDistance)
        {
            nearestEdgePoint.x = point.x;
            nearestEdgePoint.y = rect.yMin;
        }
        else if (minDistance == leftDistance)
        {
            nearestEdgePoint.x = rect.xMin;
            nearestEdgePoint.y = point.y;
        }
        else if (minDistance == rightDistance)
        {
            nearestEdgePoint.x = rect.xMax;
            nearestEdgePoint.y = point.y;
        }

        return nearestEdgePoint;
    }

    [MethodImpl(INLINED)]
    public static Vector2 GetCellCenter(float posX, float posZ, float cellSize)
    {
        return new Vector2()
        {
            x = GetCellCenter(posX, cellSize),
            y = GetCellCenter(posZ, cellSize)
        };
    }
    [MethodImpl(INLINED)]
    public static Vector2 GetCellCenter(Vector2 pos, float cellSize)
    {
        return new Vector2()
        {
            x = GetCellCenter(pos.x, cellSize),
            y = GetCellCenter(pos.y, cellSize)
        };
    }
    [MethodImpl(INLINED)]
    public static Vector2 GetCellCenter(Vector2 pos, int cellSize)
    {
        return new Vector2()
        {
            x = GetCellCenter(pos.x, cellSize),
            y = GetCellCenter(pos.y, cellSize)
        };
    }



    [MethodImpl(INLINED)]
    public static Vector2 GetCellMin(Vector2 pos, int cellSizeSqr)
    {
        return new Vector2()
        {
            x = GetCellMin(pos.x, cellSizeSqr),
            y = GetCellMin(pos.y, cellSizeSqr)
        };
    } 

    [MethodImpl(INLINED)]
    public static Vector2 GetCellMax(Vector2 pos, int cellSizeSqr)
    {
        return new Vector2()
        {
            x = GetCellMax(pos.x, cellSizeSqr),
            y = GetCellMax(pos.y, cellSizeSqr)
        };
    }
    [MethodImpl(INLINED)]
    public static float GetCellCenter(float pos, float cellSize)
    {
        return Mathf.Round(pos / cellSize) * cellSize;
    }

    [MethodImpl(INLINED)]
    public static float GetCellCenter(float pos, int cellSize)
    {
        return Mathf.Round(pos / cellSize) * cellSize;
    }

    [MethodImpl(INLINED)]
    public static float GetCellMax(float pos, int cellSize)
    {
        return  Mathf.Round(pos / cellSize) * cellSize + cellSize /2f;
    } 

    [MethodImpl(INLINED)]
    public static float GetCellMin(float pos, int cellSize)
    {
        return Mathf.Round(pos / cellSize) * cellSize - cellSize / 2f;
    }

    public static string XYHash(float x, float y) => $"{x},{y}";
    [MethodImpl(INLINED)]
    public static string V3Hash(this Vector3 hash) => $"{hash.x},{hash.y},{hash.z}";
    [MethodImpl(INLINED)]
    public static string XZHash(this Vector3 hash) => $"{hash.x},{hash.z}";
    public static Vector2 XY(this Vector3 v3) => new Vector2(v3.x, v3.y);
    public static List<Vector2> ToXZ(this List<Vector3> list)
    {
        var v2 = new List<Vector2>();
        for (int i = 0; i < list.Count; i++)
            v2.Add(list[i].XZ());
        return v2;
    }
    public static List<Vector2Int> ToInt(this List<Vector2> list)
    {
        var v2 = new List<Vector2Int>();
        for (int i = 0; i < list.Count; i++)
            v2.Add(list[i].Round());
        return v2;
    }
    #endregion

    #region Collections


    [MethodImpl(INLINED)]
    public static int RowColToIndex(int arrSize, int x, int z)
    {
        return x / (arrSize - 1) + z;
    }

    [MethodImpl(INLINED)]
    public static Vector2Int IndexToRowLen(int arrSize, int index)
    {
        return new()
        {
            x = index / arrSize,
            y = index % arrSize
        };
    }


    public static List<T> GetNameTagInLocalChildren<T>(this Transform transform, string nameTag) where T : Component
    {
        List<T> list = new List<T>();
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform tf = transform.GetChild(i);
            T Tcomp = tf.GetComponent<T>();
            if (Tcomp != null && tf.name.Contains(nameTag))
                list.Add(Tcomp);
        }
        return list;
    }
    public static T GetComponentInFirstChildren<T>(this Transform transform) where T : Component
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            T comp = transform.GetChild(i).GetComponent<T>();
            if (comp != null)
                return comp;
        }
        return null;
    }
    [MethodImpl(INLINED)]
    public static List<T> GetComponentsInFirstChildren<T>(this Transform transform) where T : Component
    {
        var list = new List<T>();
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            T comp = transform.GetChild(i).GetComponent<T>();
            if (comp != null)
                list.Add(comp);
        }

        return list;
    }


    [MethodImpl(INLINED)]
    public static T GetNameTagInAllChild<T>(this Transform transform, string nameTag) where T : Component
        => GetTypeInAllChild<T>(transform, nameTag, 0);
    [MethodImpl(INLINED)]
    public static T GetTypeInAllChild<T>(this Transform transform, string nameTag, int level) where T : Component
    {
        int childCount = transform.childCount;
        T localItem = transform.GetComponent<T>();
        if (localItem && transform.gameObject.name.Contains(nameTag))
            return localItem;

        for (int i = 0; i < childCount; i++)
        {
            T item = transform.GetChild(i).GetNameTagInAllChild<T>(nameTag);
            if (item && item.gameObject.name.Contains(nameTag))
                return item;
        }
        if (level > 0)
            Debug.LogWarning("Couldn't find by nameTag (" + nameTag + ")");
        return default(T);
    }
    static public Matrix4x4 MatrixOne { get => MatOne; }

    [MethodImpl(INLINED)]
    static public bool isInsideRect(Vector2 bottomLeft, Vector2 topRight, Vector2 point)
    {
        return point.x >= bottomLeft.x && point.x <= topRight.x
                    && point.y >= bottomLeft.y && point.y <= topRight.y;
    }

    [MethodImpl(INLINED)]
    public static void CopyBlendShapesFrom(this SkinnedMeshRenderer otherRend, SkinnedMeshRenderer body)
    {
        int shapeCount1 = otherRend.sharedMesh.blendShapeCount;
        int shapeCount2 = body.sharedMesh.blendShapeCount;
        for (int i = 0; i < shapeCount1 && i < shapeCount2; i++)
            otherRend.SetBlendShapeWeight(i, body.GetBlendShapeWeight(i));
    }

    static public int[] GetAllPossibleFlags<T>() where T : System.Enum
    {
        var typeOf = typeof(T);
        if (typeOf.BaseType != typeof(Enum))
            throw new ArgumentException("T must be an Enum type");

        // The return type of Enum.GetValues is Array but it is effectively int[] per docs
        // This bit converts to int[]
        var values = Enum.GetValues(typeof(T)).Cast<int>().ToArray();

        if (!typeOf.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
            return values;

        var valuesInverted = values.Select(v => ~v).ToArray();
        int max = 0;
        for (int i = 0; i < values.Length; i++)
        {
            max |= values[i];
        }

        var result = new List<int>();
        for (int i = 0; i <= max; i++)
        {
            int unaccountedBits = i;
            for (int j = 0; j < valuesInverted.Length; j++)
            {
                // This step removes each flag that is set in one of the Enums thus ensuring that an Enum with missing bits won't be passed an int that has those bits set
                unaccountedBits &= valuesInverted[j];
                if (unaccountedBits == 0)
                {
                    result.Add(i);
                    break;
                }
            }
        }

        return result.ToArray();
    }

   

    static public Dictionary<TValue, Tkey> ReverseKeyValues<TValue, Tkey>(this Dictionary<Tkey, TValue> originalDic)
    {
        var reversedDic = new Dictionary<TValue, Tkey>(originalDic.Count);
        foreach (var key in originalDic.Keys)
            reversedDic.Add(originalDic[key], key);

        return reversedDic;
    }

    static public void DistinctRefrences<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count;)
            {  
                if (i != j && ReferenceEquals( list[i] , list[j] ) )
                    list.RemoveAt(j);
                else j++;
            }
        }
    }

    static public void RemoveNull<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count;)
            if ( list[i] == null )
                list.RemoveAt(i);
            else i++;
    }

    static public void RemoveEmpty<T>(this List<T> list) where T : UnityEngine.Object
    {
        for (int i = 0; i < list.Count;)
            if (!list[i])
                list.RemoveAt(i);
            else i++;
    }

    static public T[] ForceLength<T>(T[] arr, int length)
    {
        if (arr == null)
            return new T[length];

        if (arr.Length == length)
            return arr.ToNewArray();

        var tmpList = new List<T>(arr);
        while (tmpList.Count < length)
            tmpList.Add(default(T));

        while (tmpList.Count > length)
            tmpList.RemoveAt(tmpList.Count - 1);

        arr = tmpList.ToArray();
        return arr;
    }

    static public T[] ToNewArray<T>(this T[] arr)
    {
        int count = arr.Length;
        var tmpArr = new T[count];
        arr.CopyTo(tmpArr, 0);
        return tmpArr;
    }
    static public List<T> ToNewList<T>(this ICollection<T> list) => new List<T>(list);
    static public T[] ToNewArray<T>(this ICollection<T> arr)
    {
        int count = arr.Count;
        var tmpArr = new T[count];
        arr.CopyTo(tmpArr, 0);
        return tmpArr;
    }
    
    static public Rect[] SubdivideFromTop(this Rect rect , float[] ySizes , bool byPrecentage)
    {
        var rects = new Rect[ySizes.Length];
        var currentPos = rect.position;
		for (int i = 0; i < ySizes.Length; i++)
		{
            var sub = new Rect(rect);
            sub.height = byPrecentage ? sub.height * ySizes[i] : sub.height + ySizes[i];
            sub.position = currentPos;
            currentPos.y += sub.height;
            rects[i] = sub;
		}
        return rects;
    }

    public static Rect ScaleSizeBy(this Rect rect, float scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }

    public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale;
        result.xMax *= scale;
        result.yMin *= scale;
        result.yMax *= scale;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }
    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale.x;
        result.xMax *= scale.x;
        result.yMin *= scale.y;
        result.yMax *= scale.y;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }

    public static Color RandomColor()
    {
        var r = _pureRandom.NextFloat(0, 1f);
        var g = _pureRandom.NextFloat(0, 1f);
        var b = _pureRandom.NextFloat(0, 1f);
        return  new(r, g, b);   
    }

    #endregion

}