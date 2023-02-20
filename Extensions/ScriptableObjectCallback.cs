namespace UnityEngine
{ 
	public abstract class ScriptableObjectCallback : Sirenix.OdinInspector.SerializedScriptableObject
	{
		public delegate void OnChangedDelegate();
		public OnChangedDelegate OnChanged;

		void OnValidate()
		{
			OnValidated();
			OnChanged?.Invoke();
			OnChangedCallbacks();
		}
		protected abstract void OnChangedCallbacks();
		protected abstract void OnValidated(); 
	}
}
