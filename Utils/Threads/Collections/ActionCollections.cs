using System;
using System.Collections.Generic;
using UnityEngine;
public class ThreadSafeActionQueue
{
	readonly Queue<Action> actionsQueue = new Queue<Action>();
	bool isExecuting = false;
	public int Count => actionsQueue.Count;
	public bool isEmpty
	{ get => actionsQueue.Count == 0; }
	public int Size
	{ get => actionsQueue.Count; }
	public void Enqueue(Action action)
	{
		lock (actionsQueue)
		{
			if (isExecuting)
				ThreadUtil.TaskAction ( () =>
				{
					lock(actionsQueue)
						actionsQueue.Enqueue(action);
				});
			else
				actionsQueue.Enqueue(action);
		}
	} 
	public void ExecuteQ(float timeOut = 0)
	{
		isExecuting = true;
		lock (actionsQueue)
		{
			float timeStarted = Time.realtimeSinceStartup;
			while (actionsQueue.Count > 0)
			{
				Action action = actionsQueue.Dequeue();
				action?.Invoke();
				if (timeOut > 0 && Time.realtimeSinceStartup > timeStarted + timeOut)
					break;
			}
		}
		isExecuting = false;
	}

	public void RemoveAll()
	{
		lock(actionsQueue)
			actionsQueue.Clear();
	}
}

public class ThreadSafeActionList
{
	public int Count => actionsList.Count;
	readonly List<Action> actionsList = new List<Action>();
	bool isExecuting = false;
	public bool isEmpty
	{ get => actionsList.Count == 0; }
	public void Remove(Action action)
	{
		lock (actionsList)
			actionsList.Remove(action);
	}

	public void AddIfNotExist(Action action)
	{
		lock(actionsList)
		{
			if (isExecuting)
				ThreadUtil.TaskAction( () =>
				{
					lock (actionsList)
					{
						if ( ! actionsList.Contains(action) )
							actionsList.Add(action);
					}
				});
			else
			{
				if ( ! actionsList.Contains(action))
					actionsList.Add(action); 
			}
		}

	}

	public void Add(Action action)
	{
		lock (actionsList)
		{
			if (isExecuting)
				ThreadUtil.TaskAction ( () =>
					{
						lock(actionsList)
							actionsList.Add(action);
					});
			else
				actionsList.Add(action);
		}
	}
	public void Execute(float timeOut = 0)
	{
		isExecuting = true;
		lock (actionsList)
		{
			float timeStarted = Time.realtimeSinceStartup;
			for(int i=0; i < actionsList.Count; i++)
			{
				Action action = actionsList[i];
				action?.Invoke();
				if (timeOut > 0 && Time.realtimeSinceStartup > timeStarted + timeOut)
					break;
			}
		}
		isExecuting = false;
	}
	public void RemoveAll()
	{
		lock (actionsList) 
			actionsList.Clear();
	}
}


public class ActionsList<T>
{
	readonly List<Action<T>> actionsList = new List<Action<T>>();
	bool isExecuting = false;

	public void Remove(Action<T> action)
	{
		lock (actionsList)
			actionsList.Remove(action);
	}
	public void Add(Action<T> action)
	{
		lock (actionsList)
		{
			if (isExecuting)
				ThreadUtil.TaskAction ( () => actionsList.Add(action));
			else
				actionsList.Add(action);
		}
	}
	public void Execute(T param , float timeOut = 0)
	{
		isExecuting = true;
		lock (actionsList)
		{
			float timeStarted = Time.realtimeSinceStartup;
			for (int i = 0; i < actionsList.Count; i++)
			{
				Action<T> action = actionsList[i];
				action?.Invoke(param);
				if (timeOut > 0 && Time.realtimeSinceStartup > timeStarted + timeOut)
					break;
			}
		}
		isExecuting = false;
	}
	public void RemoveAll()
	{
		lock(actionsList)
			actionsList.Clear();
	}
}