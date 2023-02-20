using System.Reflection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
public static class AttributeUtils 
{ 
    const BindingFlags defualtFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static public bool TryGetAttribute<T>(this MemberInfo member , out T attribute) where T : Attribute
    {
        attribute = member.GetCustomAttribute<T>();
        return attribute != null;
    }

    static public IEnumerable<Type> EnumrateTypes
	{
        get
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            int len = assemblies.Length;
            for (int ai = 0; ai < len; ai++)
            {
                Type[] types = assemblies[ai].GetTypes();
                for (int ti = 0; ti < types.Length; ti++) 
                    yield return types[ti]; 
            }
        }
	} 
    static public IEnumerable<MemberAttribute<MethodInfo, T>> GetMethods<T>(BindingFlags flags = defualtFlags ) where T : Attribute
    { 
        foreach(var type in EnumrateTypes)
        {       
            var methods = type.GetMethods(flags);
            for (int mi = 0; mi < methods.Length; mi++)
            {
                var attr = methods[mi].GetCustomAttribute<T>(true);
                if (attr != null)
                    yield return new MemberAttribute<MethodInfo,T>(methods[mi], attr);
            } 
        } 
    }

    static public IEnumerable<MemberAttribute<MemberInfo,T>> GetMembers<T>(BindingFlags flags = defualtFlags) where T : Attribute
    {
        foreach (var type in EnumrateTypes)
        {
            foreach(var member in type.GetMembers(flags))
			{
                if( member.TryGetAttribute<T>(out T attr) )
				{
                    yield return new(member, attr);
				}
			}
        }
    }
    static public IEnumerable<MemberAttribute<FieldInfo,T>> GetFields<T>(BindingFlags flags = defualtFlags) where T : Attribute 
    {
        foreach (var type in EnumrateTypes)
        {
            var fields = type.GetFields(flags);
            for (int mi = 0; mi < fields.Length; mi++)
            {
                var attr = fields[mi].GetCustomAttribute<T>(true);
                if ( attr != null)
                    yield return new MemberAttribute<FieldInfo,T>(fields[mi] , attr);
            }
        } 
    }  
    static public IEnumerable<Type> GetClasses<T>(BindingFlags flags = defualtFlags) where T : Attribute 
    {
        foreach (var type in EnumrateTypes)
        {
            var attr = type.GetCustomAttribute<T>();
            if (attr != null)
                yield return type;
        } 
    }
    static public void OnClasses<T>(Action<T, Type> action , BindingFlags flags = defualtFlags) where T : Attribute 
    {
        foreach (var clas in GetClasses<T>())
            action.Invoke(clas.GetCustomAttribute<T>(), clas);
    }

    static public void OnFields<T>(Action< MemberAttribute<FieldInfo,T> > action , BindingFlags flags = defualtFlags) where T : Attribute
    { 
        foreach (var fieldAndAttr in GetFields<T>(flags))
            action.Invoke(fieldAndAttr);
    }

    static public void OnMethods<T>(Action< MemberAttribute<MethodInfo,T> > action , BindingFlags flags = defualtFlags) where T : Attribute
    { 
        foreach (var method in GetMethods<T>(flags))
            action.Invoke(method);
    }

}

public struct MemberAttribute<MemberT,AttributeT> where MemberT : MemberInfo where AttributeT : Attribute
{
    public readonly MemberT member;
    public readonly AttributeT attribute;
    public MemberAttribute(MemberT member, AttributeT attr) { this.member = member; attribute = attr; }
} 
 