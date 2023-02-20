using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnGizmos  
{
#if UNITY_EDITOR
	[ShowInInspector] bool EnableGizmos { get;}
	public void OnDrawGizmos();
	public void OnDrawGizmosSelected();
#endif
} 