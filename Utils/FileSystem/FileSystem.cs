using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;
using System.Text; 
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
namespace Filing
{ 
	[Flags] public enum FileResult { Success = 1, Created_New = 2, OverWrite = 3, Failed = -1 , InvalidLocation = -2 , Directory_Missing = -4 }
	static public class FileSystem
	{ 
		public const float FilingVersion = 0.1f;
		static public string ProjectPath => Application.dataPath.TrimEnd("Assets".ToCharArray());
		public static readonly char DirSeperatorChar = Path.DirectorySeparatorChar;
		static public string PresistendDataPath { get; private set; }
		static public string DataPath => Application.dataPath + "/GAME_DATA/";

        static readonly Dictionary<string, object> PathLocksDic = new Dictionary<string, object>();

		static public void ValidSysSeperators(ref string path)
		{ 
			path = path.Replace('/', Path.DirectorySeparatorChar);
			path.TrimEnd(Path.DirectorySeparatorChar);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static public void CacheDataPath()
		{
			PresistendDataPath = Application.persistentDataPath; 
		}

		public static byte[] LoadBytes(string fullPath)
		{
			ValidSysSeperators(ref fullPath);
			return File.ReadAllBytes(fullPath); 
		}

		public static object GetPathLock(string path)
		{
			lock(PathLocksDic)
			{
				if (PathLocksDic.TryGetValue(path, out object objLock))
					return objLock;
				objLock = new object();
				PathLocksDic.Add(path, objLock);
				return objLock;
			} 
		}
		public static FileResult SaveFile<T>(T item, string path, BinaryFormatter formatter = null)
		{
			FileResult result; 
			ValidSysSeperators(ref path);
			FileStream streamClose = null;
			try
			{ 
				lock(GetPathLock(path))
				{ 
					result = VerifyLocation(path, out var stream);

					if (stream == null)
						stream = new FileStream(path, FileMode.Open);
					streamClose = stream;

					if (formatter == null)
						formatter = new BinaryFormatter();
				
					formatter.Serialize(stream, item);
					stream.Close();
					stream.Dispose();
				}

				return result;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				streamClose?.Close();
				return FileResult.Failed;
			}
		}

		static public FileResult VerifyLocation(string path)
		{ 
			var result = VerifyLocation(path, out var stream);
			if (stream != null)
			{
				stream.Close();
				stream.Dispose();
			}
			return result; 
		}
		static public FileResult VerifyLocation(string fullFilePath, out FileStream stream)
		{  
			ValidSysSeperators(ref fullFilePath);

			stream = null;

			if (File.Exists(fullFilePath))
				return FileResult.Success;

			var dirPath = GetDirectoryPath(fullFilePath);
			VerifyLocationDir(dirPath);

			stream = File.Create(fullFilePath);
			return FileResult.Created_New; 

		}

		static public FileResult VerifyLocationDir(string dirPath)
		{
			ValidSysSeperators(ref dirPath);

			lock ( GetPathLock(dirPath) )
			{
				if (Directory.Exists(dirPath))
					return FileResult.Success;
				else
				{
					var parent = GetDirectoryParent(dirPath);
					if (parent == null)
						return FileResult.Failed;
					if (!Directory.Exists(parent))
					{
						Directory.CreateDirectory(parent);
						return VerifyLocationDir(dirPath);
					}
				}
				Directory.CreateDirectory(dirPath);
				return FileResult.Created_New;
			}
		}

		private static string GetDirectoryParent(string dirPath)
		{ 
			var sepIndex = dirPath.LastIndexOf(DirSeperatorChar);
			if (sepIndex == -1)
				return null;
			return dirPath.Substring(0, sepIndex);
		}

		static public string GetDirectoryPath(string fullFilePath)
		{
			ValidSysSeperators(ref fullFilePath);

			int dirIndex = fullFilePath.LastIndexOf('/');

			if (dirIndex >= 0 && fullFilePath.Length > 0)
				return fullFilePath.Substring(dirIndex, fullFilePath.Length - 1);
			return "";
		}

		public static T LoadFile<T>(string path, out FileResult result) 
		{
			result = FileResult.Failed;
			try
			{
				lock( GetPathLock(path) )
				{ 
					result = VerifyLocation(path, out var fileStream);
					if (fileStream == null)
						fileStream = new FileStream(path, FileMode.Open);

					BinaryFormatter formatter = new BinaryFormatter();
					T loadedItem = (T)formatter.Deserialize(fileStream);

					fileStream.Close();
					fileStream.Dispose();
					if ( result == FileResult.Success )
						result = FileResult.OverWrite;

					return loadedItem;
				}

			}
			catch (Exception ex)
			{  
				Debug.LogException(ex); 
			}

			return default(T);
		}

		static public string[] GetFileNames(string dirPath, string searchPattern = null)
		{
            ValidSysSeperators(ref dirPath);
            VerifyLocationDir(dirPath);
            return searchPattern == null ? Directory.GetFiles(dirPath) : Directory.GetFiles(dirPath, searchPattern); 
        }

        #region JSON

        public static string ToJson<T>(T obj) => JsonConvert.SerializeObject(obj);
        public static string ToJsonReadable(object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);
        public static T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json);
        public static string LoadFileJson(string path, out FileResult result)
        {
            result = FileResult.Failed;
            try
            {
                lock (GetPathLock(path))
                {
                    result = VerifyLocation(path);
                    var json = File.ReadAllText(path);
                    if (result == FileResult.Success)
                        result = FileResult.OverWrite;

                    return json;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                result = FileResult.Failed;
                return null;
            }
        }
        public static FileResult SaveFileJson<T>(T item, string path, bool readable = true)
        {
            try
            {
                ValidSysSeperators(ref path);
                lock (GetPathLock(path))
                {
                    string json = readable ? ToJsonReadable(item) : ToJson(item);
                    var result = VerifyLocation(path, out var stream);
                    if (stream == null)
                        stream = new FileStream(path, FileMode.Open);

                    var bytes = Encoding.UTF8.GetBytes(json);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                    stream.Dispose();
                    if (result == FileResult.Success)
                        result = FileResult.OverWrite;

                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return FileResult.Failed;
            }
        } 
        internal static List<T> GetFilesJson<T>(string dirPath, string filesEnding, out FileResult result)
        {
            ValidSysSeperators(ref dirPath);
            lock (GetPathLock(dirPath))
            {
                if (Directory.Exists(dirPath))
                {
                    result = FileResult.Success;
                    var filePaths = Directory.GetFiles(dirPath);
                    var itemList = new List<T>();
                    for (int i = 0; i < filePaths.Length; i++)
                        if (filePaths[i].EndsWith(filesEnding))
                            itemList.Add(JsonConvert.DeserializeObject<T>(File.ReadAllText(filePaths[i])));
                    return itemList;
                }
            }
            result = FileResult.Directory_Missing;
            return new List<T>();
        } 
        public static string[] LoadAllFilesJson(string dirPath)
		{ 
			var files = GetFileNames(dirPath);
			var jsons = new string[files.Length];

			for (int i = 0; i < files.Length; i++)
				jsons[i] = LoadFileJson(dirPath + files[i], out _);
			return jsons;
        } 
        public static T[] LoadAllFilesJson<T>(string dirPath)
        {
            var files = GetFileNames(dirPath);
            var objs = new T[files.Length];

            for (int i = 0; i < files.Length; i++)
                objs[i] = LoadFileJson<T>(dirPath + files[i], out _); 
            return objs;
        } 
        public static T LoadFileJson<T>(string path)  => FromJson<T>( LoadFileJson( path, out _) );
        public static T LoadFileJson<T>(string path, out FileResult result)  => FromJson<T>( LoadFileJson( path, out result) );
        #endregion

        #region Load Types

        public static void SaveAsPNG(this Texture2D tex, string fullPath)
        { 
            File.WriteAllBytes(fullPath, tex.EncodeToPNG()); 
        }


		public static Texture2D LoadImage(string fullPath, bool canReadWrite)
		{
			var tex = new Texture2D(1, 1);
			var bytes = LoadBytes(fullPath);
			tex.LoadImage(bytes, canReadWrite);
			return tex;
		}

        #endregion

#if UNITY_EDITOR
        static public bool EditorRequestPath(out string path , string title , string defaultName , string dotExtension , bool fullPath )
		{
			path = UnityEditor.EditorUtility.SaveFilePanel(title, Application.dataPath, defaultName, dotExtension);
			if(fullPath)
				 path = path.Replace( ProjectPath, "");
			return path != null && path.Length > 0;
		}
#endif 
	}
}

