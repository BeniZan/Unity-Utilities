using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public static class LODExtensions
{  

    //returns the currently visible LOD level of a specific LODGroup, from a specific camera. If no camera is define, uses the Camera.current.
    public static int GetVisibleLOD(this LODGroup lodGroup, Camera camera)
    { 
        var lods = lodGroup.GetLODs();
        var relativeHeight = GetRelativeHeight(lodGroup, camera); 

        int lodIndex = GetMaxLOD(lodGroup);
        for (var i = 0; i < lods.Length; i++)
        {
            var lod = lods[i];

            if (relativeHeight >= lod.screenRelativeTransitionHeight)
            {
                lodIndex = i;
                break;
            }
        } 
        return lodIndex;
    }

#if UNITY_EDITOR
    //returns the currently visible LOD level of a specific LODGroup, from a the SceneView Camera.
    public static int GetVisibleLODSceneView(this LODGroup lodGroup)
    {
        Camera camera = SceneView.lastActiveSceneView.camera;
        return GetVisibleLOD(lodGroup, camera);
    }
#endif

    static float GetRelativeHeight(LODGroup lodGroup, Camera camera)
    {
        var distance = (lodGroup.transform.TransformPoint(lodGroup.localReferencePoint) - camera.transform.position).magnitude;
        return DistanceToRelativeHeight(camera, (distance / QualitySettings.lodBias), GetWorldSpaceSize(lodGroup));
    }

    static float DistanceToRelativeHeight(Camera camera, float distance, float size)
    {
        if (camera.orthographic)
            return size * 0.5F / camera.orthographicSize;

        var halfAngle = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
        var relativeHeight = size * 0.5F / (distance * halfAngle);
        return relativeHeight;
    }
    public static int GetMaxLOD(LODGroup lodGroup)
    {
        return lodGroup.lodCount - 1;
    }
    public static float GetWorldSpaceSize(LODGroup lodGroup)
    {
        return GetWorldSpaceScale(lodGroup.transform) * lodGroup.size;
    }
    static float GetWorldSpaceScale(Transform t)
    {
        var scale = t.lossyScale;
        float largestAxis = Mathf.Abs(scale.x);
        largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.y));
        largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.z));
        return largestAxis;
    }
}