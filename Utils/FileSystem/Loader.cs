using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System.Reflection; 
public class Loader
{ 
    public static T Load<T>(string path) where T : UnityEngine.Object
    {
      T item = Resources.Load<T>(path);
        if ( ! item )
            Debug.LogWarning($"Couldn't find resource type of { typeof(T) } at {path}"  ); 

        return item;
    }    
    public static T[] LoadAll<T>(string path) where T : UnityEngine.Object
    {
       T[] items = Resources.LoadAll<T>(path);
       if (items.Length == 0 || items == null)
            Debug.LogWarning($"Couldn't find resources of type <({typeof(T)})>[]  at {path}"); 
         
        return items; 
    }

    public static Object LoadGeneric(string path , System.Type type)
    {
        MethodInfo methodInfo = typeof(Loader).GetMethod(
            nameof(Load) , BindingFlags.Public | BindingFlags.Static); 

        methodInfo = methodInfo.MakeGenericMethod(type); 

        return methodInfo.Invoke(null, new object[] { path } ) as Object; 
    }

    public static Object[] LoadAllGeneric(string path, System.Type type)
    {
        MethodInfo methodInfo = typeof(Loader).GetMethod(
            nameof(LoadAll), BindingFlags.Public | BindingFlags.Static);
         
        methodInfo = methodInfo.MakeGenericMethod(type.GetElementType());

        return methodInfo.Invoke(null, new object[] { path }) as Object[];
    }

}
