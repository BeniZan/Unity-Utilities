using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class CriticalFileError 
{   
	static public void OnCriticalFileError(bool quitApp , string logError = null)
	{
		logError ??= "Critical files not found. please validate the game's files.";
		Debug.LogError(logError);

		//TODO  
		if (Application.isEditor)
		{
			var logerror = "MISSING CRITICAL FILES; " + logError;
			if (quitApp)
				logerror = "WOULD QUIT APP IN BUILD: " + logerror;
			Debug.LogError(logerror);
			return;
		}
		if (quitApp)
		{
			Application.Quit(1);
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
#endif
		}
	}

}
