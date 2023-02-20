using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StaticRenderer : SingletonMono<StaticRenderer>
{  
	HashSet<Matrix4x4> matrixesSet = new();
	Matrix4x4[] matrixes = new Matrix4x4[0];
	[ShowInInspector, HideInEditorMode, HideIf("@this.IsInstance"), Required] public Mesh mesh;
	[ShowInInspector, HideInEditorMode, HideIf("@this.IsInstance"), Required] public Material material;
	[ShowInInspector, HideInEditorMode, HideIf("@this.IsInstance"), Required] public MaterialPropertyBlock materialPropertyBlock;
	[HideInEditorMode, HideIf("@this.IsInstance")] public int Layer { get => gameObject.layer; set => gameObject.layer = value; }
	[HideInEditorMode, HideIf("@this.IsInstance")] public ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
	[HideInEditorMode, HideIf("@this.IsInstance")] public bool RecieveShadows = true;
	static public StaticRenderer Register(int layer, Mesh mesh, Material material)
	{
		if (!HasInstance)
		{
			FindAndForceInstance();
			if (!HasInstance)
			{  
				Debug.LogError("StaticRendere Instance not found");
				return null;
			}
		} 
		var child = new GameObject(mesh.name).transform;
		child.parent = Instance.transform;
		var childRenderer = child.AddComponent<StaticRenderer>();
		childRenderer.material = material;
		childRenderer.mesh = mesh;
		childRenderer.materialPropertyBlock = new();
		childRenderer.Layer = layer;
		return childRenderer;
	}

	private void LateUpdate()
	{ 
		Render(); 
	}

	public void Render()
	{
		lock(matrixes)
		{ 
			if (mesh && material)
			{ 
				Graphics.DrawMeshInstanced
					(
						mesh
						, 0
						, material
						, matrixes
						, matrixesSet.Count
						, materialPropertyBlock
						, shadowCastingMode
						, RecieveShadows
						, Layer
					);
			}
		} 
	} 

	public void AddPositions(Matrix4x4[] positions)
	{
		lock (matrixes)
		{
			matrixesSet.UnionWith(positions);

			if(matrixes.Length < matrixesSet.Count)
				matrixes = new Matrix4x4[matrixesSet.Count];

			matrixesSet.CopyTo(matrixes);
		}
	}
	public void Remove(Matrix4x4[] positions)
	{
		lock (matrixes)
		{
			matrixesSet.ExceptWith(positions);
		}
	}

	public void DestroyRenderer()
	{
		gameObject?.SafeDestroy();
	}
}

 