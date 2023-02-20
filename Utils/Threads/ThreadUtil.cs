using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections;
using System.Threading.Tasks;
#pragma warning disable 414

[ExecuteInEditMode]
public class ThreadUtil : SingletonMono<ThreadUtil>
{ 
	[SerializeField] private bool _enableLogging = true;
	static public Thread MainThread { get; private set; }
	static public bool LoggingEnabled => Instance ? Instance._enableLogging : true; 

	private static readonly Queue<string> allLogs = new Queue<string>();
	private static readonly Queue<string> allLogErrors = new Queue<string>();
	private static readonly Queue<Exception> allLogExceptions = new Queue<Exception>();

	private static readonly ThreadSafeActionQueue AwakeActions = new ThreadSafeActionQueue();
	private static readonly ThreadSafeActionQueue MainActionQ = new ThreadSafeActionQueue();
	private static readonly ThreadSafeActionQueue MainCriticalQ = new ThreadSafeActionQueue();
	private static readonly ThreadSafeActionList GizmosActions = new ThreadSafeActionList();
	private static readonly ThreadSafeActionList UpdateLoops = new ThreadSafeActionList(); 
	public static bool IsMainThread => MainThread == null ? throw new Exception("Main thread wasn't set") : MainThread == Thread.CurrentThread;
	static public int MaxThreads
	{
		get
		{ 
			ThreadPool.GetMaxThreads(out int workerThreads, out int completionportThreads);
			return workerThreads;
		}
		set
		{
			//todo
		}
	}
	 

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void RegisterMainThread() => MainThread = Thread.CurrentThread; 

	public static void Log(Exception ex)
	{
		if (LoggingEnabled)
		{ 
			lock (allLogExceptions)
			{
				allLogExceptions.Enqueue(ex);
			}
		}
	} 
	private static object _multiThreadTimeLock = new();
	private static float _multiThreadTime = 0f;
	public static float MultiThreadedTime
	{
		get => _multiThreadTime;
		private set
		{
			lock (_multiThreadTimeLock) 
			{
				_multiThreadTime = value; 
			}
		}
	}
	public static void Log(object log, bool isError = false, int skipFrames = 2, int maxFrameCount = 10) => Log(log == null ? "Null" : log.ToString() , isError , skipFrames , maxFrameCount);
	public static void Log(string log, bool isError = false, int skipFrames = 2, int maxFrameCount = 10)
	{ 
		if (LoggingEnabled)   
		{ 
			log = log + "\n"
				 + LogUtil.GetStackLog(skipFrames, maxFrameCount) 
				 + LogUtil.Color($"Threaded Log at Real Time:[{MultiThreadedTime}]\n", new Color(0f, 0.9f, 0f));

			if(isError)
				lock (allLogErrors)
					allLogErrors.Enqueue(log); 
			else
				lock (allLogs)
					allLogs.Enqueue(log);
		}
	} 
	public static void RemoveGizmos(Action onGizmos)
	=> GizmosActions.Remove(onGizmos); 
	 
	static public Task TaskAction(Action action)
	{
		return Task.Run(action);
	}

    static public Task TaskAction<T>(Action<T> action, T data)
    {
        return Task.Run( () => action.Invoke(data) );  
    }

    static public Task<T> TaskAction<T>(Func<T> action)
    {
		return Task.Run(action);
    }


    static public readonly TaskFactory DefaultTaskFactory = new TaskFactory();	
	   
	void testThread(int x)
	{
		for (int i = 0; i < 100; i++)
		{ 
			Log("ey" + i / x);
		}
	}

    static public Action MainAction
	{ set  =>MainActionQ.Enqueue(value);   }

	static public Action MainCriticalAction {   set => MainCriticalQ.Enqueue(value);   }

	public static Action AwakeAction
	{ 
		set => AwakeActions.Enqueue(value);  
	}
	public static Action LoopUpdate
	{ set => UpdateLoops.Add(value);   }

#if UNITY_EDITOR 
	public static void AddGizmos(Action action) => GizmosActions.AddIfNotExist(action); 
#endif

	static public Coroutine TU_StartCoroutine( IEnumerator coroutine)
	{ 
		return Instance?.StartCoroutine(coroutine ); 
	}  
	static public void TU_StopCoroutine(IEnumerator coroutine)
	{ 
		Instance?.StopCoroutine(coroutine); 
	} 
	static public Thread ThreadedMainCallback<T>(Func<T> dataAction , Action<T> mainCallback)
	{ 
		var thread = new Thread(new ThreadStart(() =>
		{
			var result = dataAction.Invoke();
			MainAction = () => mainCallback(result);
		}));
		thread.Start();
		return thread;
	}
	static public Thread ThreadedMainCallback(Action dataAction , Action mainCallback)
	{
		if (dataAction == mainCallback)
		{
			Debug.LogError("Callback can't be the same action (looped)");
			return null;
		}
		var thread = new Thread(new ThreadStart(() =>
		{
			dataAction();
			MainAction = () => mainCallback();
		}));
		thread.Start();
		return thread;
	}  
	 
	protected override void OnInstanceAwake()
	{ 
		MainThread = Thread.CurrentThread;
		 
        MultiThreadedTime = Time.time;

        int totalCalls = AwakeActions.Count + MainActionQ.Count + MainCriticalQ.Count;
		if(totalCalls > 0)
			Debug.Log("Thread Util instance has awaken with " + totalCalls); 

		if (Application.isPlaying) 
			DontDestroyOnLoad(transform.root.gameObject);  

		AwakeActions.ExecuteQ(); 
	}
	 
	protected override void OnInstanceRemoved()
	{
		base.OnInstanceRemoved();
		RemoveAllActions();  
	} 
	void RemoveAllActions()
	{
		AwakeActions.RemoveAll();
		UpdateLoops.RemoveAll();
		MainCriticalQ.RemoveAll();
		MainActionQ.RemoveAll();
		allLogs.Clear(); 
		StopAllCoroutines();
	} 
	private void Start()
	{
		MultiThreadedTime = Time.time; 
	}
	void Update()
	{ 
        MultiThreadedTime = Time.time;

        AwakeActions.ExecuteQ();
		UpdateLoops.Execute();
		MainCriticalQ.ExecuteQ();
		MainActionQ.ExecuteQ(0.02f);
		 
        if (LoggingEnabled)
		{
            lock (allLogs) 
                while (allLogs.Count > 0)
					Debug.Log(allLogs.Dequeue());

            lock (allLogErrors) 
                while (allLogErrors.Count > 0)
					Debug.LogError(allLogErrors.Dequeue());


            lock (allLogExceptions)
                while (allLogExceptions.Count > 0)
					Debug.LogException(allLogExceptions.Dequeue());
		}


    }

    private void LateUpdate()
	{
		MultiThreadedTime = Time.time;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{ 
		GizmosActions.Execute();
	}

	void OnValidate()
	{
		MainThread = Thread.CurrentThread; 
	}
#endif
}
