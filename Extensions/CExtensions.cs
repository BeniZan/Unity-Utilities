using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;

static public class CExtensions
{
	const int INLINE = (int)MethodImplOptions.AggressiveInlining;
	[MethodImpl(INLINE)] static public string TrimEndUntil(this string input, char until) => input.Substring(input.LastIndexOf(until) + 1); 
	[MethodImpl(INLINE)] static public string TrimEndUntil(this string input, string until) => input.Substring(input.LastIndexOf(until) + 1);
	[MethodImpl(INLINE)] static public string TrimStartUntil(this string input , char until) => input.Substring(input.IndexOf(until) + 1); 
	[MethodImpl(INLINE)] static public string TrimStartUntil(this string input , string until) => input.Substring(input.IndexOf(until) + 1); 

	[MethodImpl(INLINE)] 	static public string ToStringEnum(this IEnumerable enumerable)
	{
		var str = "";
		var i = 0;
		foreach (var e in enumerable)
			str += $"[{i++}]: {e.ToString()}\n";
		return str;
	}
	[MethodImpl(INLINE)]
	static public void SafeSub<T>(this Action subTo , Action subscribedAction )
	{
		subTo -= subscribedAction;
		subTo += subscribedAction;
	}

	[MethodImpl(INLINE)]
	static public string ToStringForEach<T>(this ICollection<T> coll ,string preString = "[{i++}]: ", string postString = "\n")
	{
		if (coll == null)
			return "null collection";
		var str = "";
		var i = 0;
		var count = coll.Count;
        foreach (var item in coll)
		{
			i++; 
			str += $"{preString}{item}{(i == count ? "" : postString)}"; 
        }
        return str;
	} 

    [MethodImpl(INLINE)]
	static public bool ValidIndex<T>(this ICollection<T> collection , int index)
	{
		return index >= 0 && index < collection.Count;
	}
	[MethodImpl(INLINE)]
	static public bool TryCast<T, TCast>(this T obj , out TCast castedObj) where TCast : class
	{
		castedObj = obj as TCast; 
		return castedObj != null;
	}
	[MethodImpl(INLINE)]
	static public bool IsEmpty<T>(this ICollection<T> col) => col.Count <= 0;
	 
	public static bool IsSubclassOf(Type type, Type baseType)
	{
		if (type == null || baseType == null || type == baseType)
			return false;

		if (baseType.IsGenericType == false)
		{
			if (type.IsGenericType == false)
				return type.IsSubclassOf(baseType);
		}
		else
		{
			baseType = baseType.GetGenericTypeDefinition();
		}

		type = type.BaseType;
		Type objectType = typeof(object);
		while (type != objectType && type != null)
		{
			Type curentType = type.IsGenericType ?
				type.GetGenericTypeDefinition() : type;
			if (curentType == baseType)
				return true;

			type = type.BaseType;
		}

		return false;
	}
	[MethodImpl(INLINE)] public static List<T> ToList<T>(this IEnumerable<T> enumerator) => new List<T>(enumerator); 
	[MethodImpl(INLINE)] static public IEnumerable<Type> GetDerivingTypes(this Type baseType , bool exludeAbstract = true)
	{ 
		foreach( var assem in AppDomain.CurrentDomain.GetAssemblies() )
		{
			foreach ( var t in assem.GetTypes() )
			{
				if ( IsSubclassOf(t,baseType) )
				{
					if (exludeAbstract && t.IsAbstract)
						continue;
					yield return t;
				}
			}
		} 
	}
	[MethodImpl(INLINE)]
	static public string TrimEnd(this string source, string trimValue)
	{
		if (!source.EndsWith(trimValue))
			return source;

		return source.Remove(source.LastIndexOf(trimValue)); 
	}
}
