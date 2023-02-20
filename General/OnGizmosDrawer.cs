using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;


[ExecuteInEditMode]
public class OnGizmosDrawer : SingletonMono<OnGizmosDrawer> 
{
#if UNITY_EDITOR 
	static List<MethodInfo> methods = new(); 
	static OnGizmosDrawer()
	{
		var mrEnum = AttributeUtils.GetMethods<OnGizmosAttribute>();
		var methods = new List<MethodInfo>();
		foreach (var mr in mrEnum)
		{
			if (mr.member.IsStatic)
				methods.Add(mr.member);
			else
				ThreadUtil.Log("OnGizmosAttribute Error: " + mr.member.Name + " must be static " , true);
		}
	}
	 
	void OnDrawGizmos()
	{
		foreach (var m in methods)
			m.Invoke(null , null);
	}
#endif 
} 
public class OnGizmosAttribute : Attribute  {  }
