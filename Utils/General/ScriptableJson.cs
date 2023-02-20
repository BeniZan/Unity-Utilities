using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System;
using Filing;
abstract public class ScriptableJson<TData> : SerializedScriptableObject
{  

    public static TScriptableJson Load<TScriptableJson>(string filePath) where TScriptableJson : ScriptableJson<TData>
    { 
        var scriptable = CreateInstance<TScriptableJson>();
        scriptable.LoadJson(filePath);
        return scriptable;
    }
    abstract protected void SetJsonData(TData jsonData); 
    abstract protected TData GetJsonData();
    public FileResult LoadJson(string filePath)
    { 
        SetJsonData(FileSystem.LoadFileJson<TData>(filePath, out var fileResult));
        return fileResult;  
    } 
    public FileResult SaveJson(string fullPath)
    {  
       return FileSystem.SaveFileJson(GetJsonData(), fullPath);  
    } 

}
