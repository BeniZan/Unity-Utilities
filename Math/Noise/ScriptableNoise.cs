using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewNoise", menuName = "Generation/Noise", order = 0)] 
public class ScriptableNoise : SerializedScriptableObject
{
    [SerializeField, HideLabel] public Noise noise; 

    private void OnValidate()
    {
        if (noise == null)
            noise = new();
    }
}
