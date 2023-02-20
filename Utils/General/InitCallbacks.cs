using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
static public class RuntimeInit 
{
	static readonly ThreadSafeActionList[] actionLists;
	static readonly ThreadSafeActionQueue[] queueLists;

	static RuntimeInit()
	{  
		actionLists = new ThreadSafeActionList[5];
		queueLists = new ThreadSafeActionQueue[5];
		foreach (int index in Enumerable.Range(0, 5))
		{
			actionLists[index] = new ThreadSafeActionList();
			queueLists[index] = new ThreadSafeActionQueue();
		}
	}

	static public void SubscribeEveryTime(Action action , RuntimeInitializeLoadType loadType)
	{
		actionLists[(int)loadType].Add(action);
	}
	static public void SubscribeOneTime(Action action, RuntimeInitializeLoadType loadType)
	{
		queueLists[(int)loadType].Enqueue(action);
	}

	static void Execute(RuntimeInitializeLoadType loadType)
	{
		actionLists[(int)loadType].Execute();
		queueLists[(int)loadType].ExecuteQ();
	}



	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	static void OnAssembliesLoaded() => Execute(RuntimeInitializeLoadType.AfterAssembliesLoaded);


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void OnAfterSceneLoaded() => Execute(RuntimeInitializeLoadType.AfterSceneLoad);


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnBeforeSceneLoad() => Execute(RuntimeInitializeLoadType.BeforeSceneLoad);


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	static void OnBeforeSplashScreen() => Execute(RuntimeInitializeLoadType.BeforeSplashScreen);

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void OnSubsystemReg() => Execute(RuntimeInitializeLoadType.SubsystemRegistration);
}
