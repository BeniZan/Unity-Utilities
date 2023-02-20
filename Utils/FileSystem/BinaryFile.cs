using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Filing
{ 
	abstract public class BinaryFile : BaseFile
	{
		protected override T LoadMethod<T>(out FileResult result)  => FileSystem.LoadFile<T>(FileFullLocation, out result);
		protected override FileResult SaveMethod<T>() => FileSystem.SaveFile(this as T, FileFullLocation); 
	}   
}

