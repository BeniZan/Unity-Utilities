using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SingeltonsParentGO : SingletonMono<SingeltonsParentGO>
{

	bool firstLoad = true;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	void ForceSingltons()
	{
#if UNITY_EDITOR
		if (gameObject.IsInPrefabStage())
			return;
#endif
		if(firstLoad)
		{
			var singTypes = CExtensions.GetDerivingTypes(typeof(SingletonMono<>) , true);
			foreach(var singT in singTypes)
			{ 
				var singMono = GetComponentInChildren(singT);
				if (singMono != null)
					(singMono as SingletonMono<MonoBehaviour>).ForceInstance();
			}

			firstLoad = false; 
		}
	}

}
