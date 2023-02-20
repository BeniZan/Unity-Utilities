using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

public class ThreadTask
{
	ThreadSafeBool _stopThreading = new();
	Queue<Action> actionQ = new();
	Queue<Action> onDoneActionQ = new();
	Semaphore awaitActions = new(0,int.MaxValue);
	object lockObj = new();
	int semaphoreCount = 0;
	public async void ForceStop()
	{
		actionQ.Dequeue()?.Invoke();
		onDoneActionQ.Dequeue()?.Invoke();
		_stopThreading.Value = true;
		try
		{ 
			awaitActions.Release(semaphoreCount);
		}catch(Exception ex) { Debug.LogException(ex); }
		
	}
	

	public ThreadTask()
	{
		Application.quitting += OnQuit;
		ThreadUtil.TaskAction( LoopTasks );
	}

	public void AddTask(Action action , Action onDone = null)
	{ 
		lock(lockObj)
		{
			actionQ.Enqueue(action);
			onDoneActionQ.Enqueue(onDone);
			awaitActions.Release();
			semaphoreCount++; 
		}
	}

	void LoopTasks()
	{ 
		while( ! _stopThreading )
		{
			awaitActions.WaitOne();
			var action = actionQ.Dequeue();
			var onDone = onDoneActionQ.Dequeue();
			lock ( lockObj )
			{
				semaphoreCount--; 
				action?.Invoke();
				onDone?.Invoke(); 
			} 
		} 
	}

	void OnQuit()
	{
		_stopThreading.Value = true;
	}

	~ThreadTask()
	{
		_stopThreading.Value = true;
		Application.quitting -= OnQuit; 
	}
}
