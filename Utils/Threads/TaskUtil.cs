using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
public static class TaskUtil 
{  
	static public void StartAndAwait(ICollection<Action> actions )
	{
		var factory = Task.Factory; 
		int i=0;
		var tasks = new Task[actions.Count];
		foreach(var action in actions)
		{
			tasks[i] = factory.StartNew(action);
			i++;
		}

		Task.WaitAll(tasks); 
	}

}
