using UnityEngine; 
using Sirenix.OdinInspector;
[ExecuteInEditMode]
public class ParentScaleToMaterial : MonoBehaviour
{
	public float ScaleMultiplier = 1;
	public Renderer[] Renderers = new Renderer[0]; 
	static int TilingID = Shader.PropertyToID("_Scale");
	Vector3 lastScaling = Vector3.zero;
	public bool CheckForScaleChange = true;
	[Button()] void FindRenderers() => Renderers = GetComponentsInChildren<Renderer>();
	public void Awake() => SetMaterialScaling();

	void Update()
	{
		if (CheckForScaleChange && lastScaling != transform.localScale)
			SetMaterialScaling();
	}

	[Button()] void SetMaterialScaling()
	{
		var originScale = transform.lossyScale;
		var scale = originScale * ScaleMultiplier;
		lastScaling = scale;
		for (int i = 0; i < Renderers.Length; i++)
		{
			var block = new MaterialPropertyBlock();
			var mesh = Renderers[i].GetComponent<MeshFilter>();
			if (mesh)
			{
				var bounds = mesh.sharedMesh.bounds;
				var size = Vector3.Scale(bounds.size, scale);
				if (size.y < .001)
					size.y = size.z;

				if (Renderers[i].HasPropertyBlock())
					Renderers[i].GetPropertyBlock(block);
				block.SetVector(TilingID, scale);
				Renderers[i].SetPropertyBlock(block);
			}
		}
	}

	void OnValidate() => Awake();
}
