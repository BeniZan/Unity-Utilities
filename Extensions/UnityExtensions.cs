using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using UnityEditor;
using Cinemachine;
using UnityEngine.SceneManagement;
static public class UnityExtensions
{
	const int OPTIMIZED = (int)MethodImplOptions.AggressiveInlining;

	#region GameObject

#if UNITY_EDITOR
	static public bool IsInPrefabStage(this GameObject go)
	{
		var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
		if (stage) 
			return stage.IsPartOfPrefabContents(go); 
		return false;
	} 
#endif

	static public Component AddChild(this Component comp, string name = "")
	{
		var childtf = new GameObject(name).transform;
		childtf.SetParent(comp.transform);
		return childtf;
	}

	static public GameObject AddChild(this GameObject go, string name = "")
	{
		var childGo = new GameObject(name);
		childGo.transform.SetParent(go.transform);
		return childGo;
	}

    #endregion

    #region Components 


#if UNITY_EDITOR
    [MethodImpl(OPTIMIZED)]
    static public void ForceDirtyAndSave(this UnityEngine.Object obj)
    {
        UnityEditor.EditorUtility.SetDirty(obj);
        UnityEditor.AssetDatabase.SaveAssetIfDirty(obj);
    }
#endif
    [MethodImpl(OPTIMIZED)]
    static public T GetOrAddCompnent<T>(this GameObject go) where T : Component
    {
        if (go.TryGetComponent<T>(out var outT))
            return outT;
        return go.gameObject.AddComponent<T>();
    }
    [MethodImpl(OPTIMIZED)]
    static public T GetOrAddCompnent<T>(this Component comp) where T : Component
    {
        if (comp.TryGetComponent<T>(out var outT))
            return outT;
        return comp.gameObject.AddComponent<T>();
    }

    [MethodImpl(OPTIMIZED)]
    static public T GetOrCreateCompnent<T>(this GameObject go) where T : Component
    {
        if (go.TryGetComponent<T>(out var outT))
            return outT;
        return go.gameObject.AddComponent<T>();
    }

    [MethodImpl(OPTIMIZED)]
    static public T AddComponent<T>(this Component comp) where T : Component => comp.gameObject.AddComponent<T>();
    [MethodImpl(OPTIMIZED)]
    static public Transform AddChild(this Transform tf, string name = "New GameObject")
    {
        var child = new GameObject(name).transform;
        child.parent = tf;
        return child;
    }
    [MethodImpl(OPTIMIZED)]
    public static void SafeDelay(this UnityEngine.Object behave, Action action)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () => action();
#else
        action();
#endif
    }

    [MethodImpl(OPTIMIZED)]
    static public void SafeDestroy(this Transform tf)
    {
        if (tf && tf.gameObject)
            SafeDestroy(tf.gameObject);
    }
    [MethodImpl(OPTIMIZED)]
    static public void SafeDestroy(this UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        if(UnityEditor.PrefabUtility.IsPartOfAnyPrefab(obj))
            Debug.LogWarning("Can't destroy prefab instance");
#endif
        if (obj)
        {
            if (Application.isPlaying)
                UnityEngine.Object.Destroy(obj);
            else
                UnityEngine.Object.DestroyImmediate(obj);
        }
    }
    static public VisualElement Root(this VisualElement element)
    {
        if (element == null)
            throw new NullReferenceException();
        while (element.parent != null)
            element = element.parent;
        return element;
    }
    static public List<T> GetComponentsInFirstChildren<T>(this Component comp) where T : Component
    {
        var tf = comp.transform;
        var childCount = tf.childCount;
        var listT = new List<T>();
        for (int i = 0; i < childCount; i++)
            listT.AddRange(tf.GetChild(i).GetComponents<T>());
        return listT;
    }

    static public IEnumerable LoopChildren(this Transform tf)
    {
        var count = tf.childCount;
        for (int i = 0; i < count; i++)
            yield return tf.GetChild(i);
    }
    static public void LoopChildren(this Transform tf, Action<Transform> action)
    {
        var count = tf.childCount;
        for (int i = 0; i < count; i++)
            action.Invoke(tf.GetChild(i));
    }
    static public IEnumerator<Transform> LoopGrandChildren(this Transform tf)
    {
        var count = tf.childCount;
        for (int i = 0; i < count; i++)
        {
            var child = tf.GetChild(i);
            if (child.childCount > 0)
            {
                var grandChildren = LoopGrandChildren(child);
                while (grandChildren.MoveNext())
                    yield return grandChildren.Current;
            }
            yield return tf.GetChild(i);
        }
    }
    static public void LoopGrandChildren(this Transform tf, Action<Transform> action)
    {
        var grandChildren = LoopGrandChildren(tf);
        while (grandChildren.MoveNext())
            action.Invoke(grandChildren.Current);
    }
#endregion

#region Camera

    static public T GetOrAddCinemachineComponent<T>(this CinemachineVirtualCamera cam) where T : CinemachineComponentBase
	{  
		var comp = cam.GetCinemachineComponent<T>();
		if (comp)
			return comp;
		
		return cam.AddCinemachineComponent<T>(); 
	}

#endregion

#region Scenes 
	static public bool TryGetRootComp<T>(out T res) where T : Component
	{ 
		res = null;
        var scene = SceneManager.GetActiveScene();
        if (scene.isLoaded)
        {
            var rootGos = scene.GetRootGameObjects();
            foreach (var go in rootGos)
                if (go.TryGetComponent(out res))
                    return true;
        } 
		return false;
	}
	static public bool IsMenuScene(this Scene scene) => scene.name.StartsWith("Menu");
	static public bool IsGameLevelScene(this Scene scene) => scene.name.StartsWith("Level"); 
	static public bool IsEditorDemoLevelScene(this Scene scene) => scene.name.StartsWith("DEMO");
	static public bool IsGameplayOrDemo(this Scene scene) => IsGameLevelScene(scene) || IsEditorDemoLevelScene(scene);
#endregion

#region Events
    static public bool IsRepaint(this Event e) => e != null && e.type == EventType.Repaint;
	static public bool IsMousePrimaryClick(this Event e) => e.isMouse && e.type == EventType.MouseDown && e.button == 0;
	static public bool IsMouseRightClick(this Event e) => e.isMouse && e.type == EventType.MouseDown && e.button == 1;
	static public bool IsMouseMiddleHold(this Event e) => e.isMouse && e.button == 2;
	static public bool IsKey(this Event e, KeyCode k) =>  e.keyCode == k;
	static public bool IsKeyDown(this Event e) =>  e.type == EventType.KeyDown;
	static public bool IsKeyUp(this Event e) =>  e.type == EventType.KeyUp;
	static public bool IsMouseDown(this Event e) => e.type == EventType.MouseDown;
	static public bool MousePosDelta(this Event e) =>  e.isMouse && e.button == 2; 
	static public Vector2 MouseScrollDelta(this Event e) =>  e.type == EventType.ScrollWheel ? e.delta : new Vector2();
    static public bool GetPanInput(Event e, out Vector2 pan)
    {
        pan = new Vector2();
        if (e.IsKeyDown() && e.IsKey(KeyCode.UpArrow) || e.IsKey(KeyCode.DownArrow) || e.IsKey(KeyCode.RightArrow) || e.IsKey(KeyCode.PageDown))
        {
            var zoomAmount = 15f;
            if (e.IsKey(KeyCode.UpArrow))
                pan.y += zoomAmount;
            if (e.IsKey(KeyCode.DownArrow))
                pan.y -= zoomAmount;
            if (e.IsKey(KeyCode.RightArrow))
                pan.x += zoomAmount;
            if (e.IsKey(KeyCode.LeftArrow))
                pan.x -= zoomAmount;
            return true;
        }
        if (e.isMouse && e.IsMouseMiddleHold() && e.IsMouseDown())
        {
            pan += e.delta;
            e.Use();
            return true;
        }
        return false;
    }
#endregion


}
