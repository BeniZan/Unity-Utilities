using Sirenix.OdinInspector;
using UnityEngine;

public class ScriptableGeneric<T> : SerializedScriptableObject
{
	[Sirenix.Serialization.OdinSerialize , ShowInInspector ] public T Value = default(T); 
	static public ScriptableGeneric<T> Create() 
	{
		return CreateInstance<ScriptableGeneric<T>>();  
	}
}
