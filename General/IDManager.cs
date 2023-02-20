using System;
using System.Collections.Generic;
using System.Collections;
 
static class IDManager
{
	static Hashtable _IdTable = new Hashtable();

	static public void InputID(object key , Object data)
	{
		if (_IdTable.ContainsKey(key))
			ThreadUtil.Log( "IdTable Error: already contains key: " + key.ToString());
		else
			_IdTable.Add(key , data); 
	}

	static public bool HasKey(object key) => _IdTable.ContainsKey(key);

	static public object GetValue(object key) => _IdTable[key];



} 
