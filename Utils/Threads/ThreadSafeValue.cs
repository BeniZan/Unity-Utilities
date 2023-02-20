using System;
using UnityEngine;
using Sirenix.OdinInspector;
[Serializable] public class ThreadSafeValue<T>
{
	private object _lockObj;
	[ShowInInspector,ReadOnly]  private T _value;  
	public T Value
	{
		set
		{
			lock (this)
				_value = value;
		} 
		get
		{
			lock (this)
				return _value;
		} 
	}
	public override bool Equals(object obj) => Value.Equals(obj);
	public override int GetHashCode() => Value.GetHashCode();
	public override string ToString()	=> Value.ToString();
	public ThreadSafeValue() { _lockObj = this; }
	public ThreadSafeValue(T value) { Value = value; _lockObj = this; }
	public ThreadSafeValue(T value,object lockObj) { Value = value; _lockObj = lockObj; }

}

[Serializable] public class ThreadSafeBool : ThreadSafeValue<bool>
{
	public ThreadSafeBool() { } 
	public ThreadSafeBool(bool value) : base(value) { } 
	public ThreadSafeBool(bool value, object lockObj) : base(value, lockObj) { }

	public override bool Equals(object obj) => base.Equals(obj); 

	public override int GetHashCode() => base.GetHashCode(); 

	public override string ToString() => base.ToString(); 

	public static bool operator ==(ThreadSafeBool b1, bool b2) => b1.Value == b2;
	public static bool operator !=(ThreadSafeBool b1, bool b2) => b1.Value != b2; 

	public static implicit operator bool(ThreadSafeBool tsBool) => tsBool.Value;   
}
[Serializable] public class ThreadSafeInt : ThreadSafeValue<int> 
{
	public ThreadSafeInt() { }
	public ThreadSafeInt(int value) : base(value) { }
	public ThreadSafeInt(int value, object lockObj) : base(value, lockObj) { }

	public override bool Equals(object obj) => base.Equals(obj); 

	public override int GetHashCode() => base.GetHashCode(); 

	public override string ToString()=>base.ToString(); 

	public static bool operator ==(ThreadSafeInt b1, int b2) => b1.Value == b2;
	public static bool operator !=(ThreadSafeInt b1, int b2) => b1.Value != b2; 

	public static implicit operator int(ThreadSafeInt tsBool) => tsBool.Value; 
}
