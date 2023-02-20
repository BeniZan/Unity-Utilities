using System.Runtime.CompilerServices; 
using UnityEngine; 

static public class AnimatorVals  
{
	static readonly public int MovDirectionX = Animator.StringToHash("MovDirectionX");
	static readonly public int MovDirectionY = Animator.StringToHash("MovDirectionY");
	static readonly public int MovDirectionZ = Animator.StringToHash("MovDirectionZ");
	static readonly public int IsIdle = Animator.StringToHash("IsIdle");
	static readonly public int IsRunning = Animator.StringToHash("IsRun");
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static public void LerpFloat(this Animator anim, int id , float value, float lerp)
	{
		anim.SetFloat(id, Mathf.Lerp( anim.GetFloat(id),value,lerp)  );

	}
}
