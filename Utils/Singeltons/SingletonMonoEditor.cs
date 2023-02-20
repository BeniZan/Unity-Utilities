using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
abstract public class SingletonMonoEditor<T> : SingletonMono<T> where T : MonoBehaviour
{
#if UNITY_EDITOR 
	protected abstract void EditorUpdateInEditMode();
	protected abstract void EditorUpdateInPlayMode();
#endif 
	protected override void OnInstanceEnabled()
	{ 
#if UNITY_EDITOR
		ChainEditorUpdate(); 
#endif
	}
	protected override void OnInstanceRemoved()
	{ 
#if UNITY_EDITOR
		RemoveEditorUpdate();
#endif
	}

	protected override void OnInstanceDisabled()
	{
		base.OnInstanceDisabled();
#if UNITY_EDITOR
		RemoveEditorUpdate();
#endif
	}

#if UNITY_EDITOR
	void ChainEditorUpdate()
	{
		RemoveEditorUpdate();
		EditorApplication.update += Application.isPlaying ? EditorUpdateInPlayMode : EditorUpdateInEditMode;
	}
#endif
#if UNITY_EDITOR 
	void RemoveEditorUpdate()
	{
		EditorApplication.update -= EditorUpdateInEditMode;
		EditorApplication.update -= EditorUpdateInPlayMode;
	}
#endif


}
