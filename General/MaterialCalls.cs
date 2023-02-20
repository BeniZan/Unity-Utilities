using UnityEngine; 

static public class MaterialCalls
{
	static readonly int MatVal_Intes = Shader.PropertyToID("_Intensity");
	static readonly int MatVal_MainColor = Shader.PropertyToID("_MainColor");
	static readonly int MatVal_Emmis = Shader.PropertyToID("_EmmisionColor");
	static readonly int MatVal_EnableEmmis = Shader.PropertyToID( "_EnableEmmision");
	public static float GetIntensity(this MaterialPropertyBlock block)
	=> block.GetFloat(MatVal_Intes);
	public static void SetIntensity(this MaterialPropertyBlock block , float intensity)
	=> block.SetFloat(MatVal_Intes , intensity);
	public static Color GetMainColor(this MaterialPropertyBlock block)
	=> block.GetColor(MatVal_Emmis);
	public static void setMainColor(this MaterialPropertyBlock block, Color color)
	 => block.SetColor(MatVal_MainColor, color);
	public static Color GetEmmision(this MaterialPropertyBlock block)
	=> block.GetColor(MatVal_Emmis); 
	public static void SetEmmision(this MaterialPropertyBlock block, Color emmissionColor)
	=> block.SetColor(MatVal_Emmis, emmissionColor);

 	public static void enableEmmision(this MaterialPropertyBlock block, bool enabled)
	=> block.SetInt(MatVal_EnableEmmis, enabled ? 1 : 0);
	public static bool isEmmisionEnabled(this MaterialPropertyBlock block)
	=> block.GetInt(MatVal_EnableEmmis) != 0;
}