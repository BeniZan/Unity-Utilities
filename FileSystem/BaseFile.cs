using System;
using UnityEngine;
using UnityEditor; 
using System.Collections;
using System.IO;
using Newtonsoft.Json;
namespace Filing
{ 
	[Serializable] abstract public class BaseFile  
	{ 
		[JsonIgnore] static public string PresistentDataPath;
		[JsonIgnore] static public string DataPath;
		[JsonIgnore] static public readonly string TempFileEnding = ".temp";
		[JsonIgnore] static public readonly string BackupFileEnding = ".backup"; 
		public readonly object SaveLock = new object();
		string fullPath; 
		public BaseFile(string fullPath = null)
		{
			PresistentDataPath = Application.persistentDataPath;
			DataPath = Application.dataPath;
			this.fullPath = fullPath;
		}
		protected abstract void OnValidate();
		/// <summary>  Do Not Change while saving or loading. use lock(formatterLock)  </summary>
		[JsonIgnore] public virtual string FileName { get; protected set; }
		/// <summary>  End with "/". Do Not Change while saving or loading. use lock(formatterLock)  </summary> 
		[JsonIgnore] public virtual string FileFolder { get; protected set; }
		[JsonIgnore] public string FileInProjectPath => FileFolder + FileName;
		/// <summary> data path = '-Path to project-\Assets' | presistent data = '%userprofile%\AppData\Local\Packages\<productname>\LocalState' </summary>  
		[JsonIgnore] public bool UsePresistentDataPath => true;
		[JsonIgnore] public string FileFullLocation => (UsePresistentDataPath ? Application.persistentDataPath : Application.dataPath) + FileInProjectPath;
		[JsonIgnore] public string FileFullBackupLoaction => FileFullLocation + BackupFileEnding;
		[JsonIgnore] public string FileFullTempLoaction => FileFullLocation + BackupFileEnding;
		 
		abstract protected FileResult SaveMethod<T>() where T : BaseFile;
		abstract protected T LoadMethod<T>(out FileResult result);
		public virtual FileResult Save<T>() where T : BaseFile
		{
			if (fullPath == null)
				return FileResult.Failed;

			lock (SaveLock)
			{
				OnValidate();
				return SaveMethod<T>();
			}
		}
		public virtual T Load<T>(out FileResult result) where T : BaseFile
		{
			result = FileResult.Failed;
			if (fullPath == null)
				return default(T);

			lock (SaveLock)
			{
				OnValidate();
				return LoadMethod<T>(out result);
			}
		} 

		public FileResult SafeSave<T>() where T : BaseFile
		{
			FileResult result;
			lock(SaveLock)
			{ 
				result = Save<T>(); 
				File.Replace( FileFullTempLoaction , FileFullLocation, FileFullBackupLoaction  );
			}
			return result & FileResult.Success;
		} 

		public T SafeLoad<T>(out FileResult result , T defaultT = default(T) ) where T : BaseFile
		{
			result = FileResult.Failed;
			var loadedT = defaultT;
			try
			{
				loadedT = FileSystem.LoadFile<T>(FileFullLocation, out result);
				if (result == FileResult.Success)
					return loadedT;

				loadedT = FileSystem.LoadFile<T>(FileFullBackupLoaction, out var backupRes);
				result = result & backupRes;
			}
			catch (Exception ex) { 
				
				ThreadUtil.Log("Exception: " + ex.ToString(),  true ); 

			}

			return loadedT;
		}

	}

}
