using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Filing
{ 
	abstract public class JsonFile : BaseFile
	{
		protected override T LoadMethod<T>(out FileResult result) => FileSystem.LoadFileJson<T>(FileFullLocation, out result);
		protected override FileResult SaveMethod<T>() => FileSystem.SaveFileJson(this as T, FileFullLocation); 
	}   
}

