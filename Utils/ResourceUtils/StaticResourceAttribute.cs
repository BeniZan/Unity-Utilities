using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Sirenix.Utilities;

public class StaticResourceAttribute : Attribute
{
    string _path;
    public StaticResourceAttribute(string path)
    {
        _path = path;   
    }

    string Load(MemberInfo member)
    { 
        if (_path == null) 
            return LogUtil.ColorRed($"StaticResourceError ({member.DeclaringType}.{member.Name}): Invalid NULL Path");  

        var uObj = Resources.Load(_path);
        if (uObj == null)
            return LogUtil.ColorRed($"StaticResourceError ({member.DeclaringType}.{member.Name}): Incorrect path ({_path})");
        member.SetMemberValue(null, uObj);
        return LogUtil.ColorGreen($"StaticResourceLoaded: ({member.DeclaringType}.{member.Name}): {uObj.name})");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static public void LoadAll()
    {
        foreach(var attInfo in AttributeUtils.GetMembers<StaticResourceAttribute>() )
            attInfo.attribute.Load(attInfo.member); 
    }
}
