using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class UniqueGuidGenerator  
{
	class CompareGuids : IComparer<Guid>
	{   public int Compare(Guid x, Guid y) => x.CompareTo(y);  }

	SortedSet<Guid> set = new SortedSet<Guid>(new CompareGuids()); 
	public Guid Generate()
	{
		var id = Guid.NewGuid();

		while ( set.Contains( id ) )
			id = Guid.NewGuid();

		set.Add(id);
		return id;
	} 

}
