using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
abstract public class SingletonMono<T> : MonoBehaviour  where T : MonoBehaviour
{
    private static T _instance = null;
    bool isAwakeCalled; 
    public static bool HasInstance => _instance; 
    public static T Instance
    { 
        private set
        {
            if (_instance && value == _instance)
                return;

            if(HasInstance)
                (_instance as SingletonMono<T>).OnInstanceRemoved(); 

            _instance = value;
             
            if (_instance != null)
            {
                var instanceSing = _instance as SingletonMono<T>; 

                instanceSing.OnInstanceSet();
                if (!instanceSing.isAwakeCalled)
                    instanceSing.Awake();

                if (instanceSing.enabled)
                    instanceSing.OnEnable();
                else
                    instanceSing.OnDisable();
            } 
        }
        get => _instance;
         
    }  

    static public T FindAndForceInstanceAtRootGOs()
	{
        if ( ! _instance ) 
        {
            if ( UnityExtensions.TryGetRootComp<T>(out var rootT) ) 
                return Instance = rootT; 
        }
        return FindAndForceInstance();
    }

    static public T FindAndForceInstance()
    {
        if ( ! _instance )
        {
            var inst = FindObjectOfType<T>();
            if (inst)
                (inst as SingletonMono<T>).ForceInstance();
            return inst;
        } 
        return _instance;
    } 
    public bool IsInstance { get => _instance && this == _instance; } 
    [PropertyOrder(100), DisableIf("@this.IsInstance"), Button("@this.IsInstance ? \"Instance Class\" : \"Force Instance\"")]  
    public void ForceInstance() => Instance = this as T;   
    virtual protected bool OverridableAwake() => false;

	void Awake()
	{

        if (OverridableAwake())
            return;

#if UNITY_EDITOR
        var prefabStatus = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(gameObject);
        if(prefabStatus == UnityEditor.PrefabInstanceStatus.NotAPrefab)
		{
#endif
            T thisT = this as T;
            if (HasInstance)
            {
                if (IsInstance)
                    OnInstanceAwake();
                else 
                    OnNoneInstanceAwake();
            }
            else 
                Instance = thisT;  
#if UNITY_EDITOR
        }
#endif
        isAwakeCalled = true;

        OnAnyAwake(); 
    }


	void OnEnable()
    { 
        T  thisT = this as T;

        if (_instance != null && _instance != thisT) 
            OnNoneInstanceEnabled(); 
        else
        {
            Instance = thisT; 
            OnInstanceEnabled(); 
        }  
    }
    public void OnDestroy()
	{

		if (_instance == this)
        {
            OnInstanceRemoved();
            OnInstanceDestroy();
            Instance = null;
        }

		OnAnyDestroy();

	} 
	void OnDisable()
	{
        if (IsInstance)
            OnInstanceDisabled();
	} 
    protected virtual void OnAnyAwake() { }
    protected virtual void OnAnyDestroy() { }
	protected virtual void OnInstanceSet() {}
    protected virtual void OnInstanceDisabled() { }
    protected virtual void OnInstanceRemoved() { }
    protected virtual void OnInstanceDestroy() { }
    protected virtual void OnInstanceEnabled() { }
    protected virtual void OnNoneInstanceEnabled() { }
    protected virtual void OnInstanceAwake() { }
    protected virtual void OnNoneInstanceAwake() { }

}
