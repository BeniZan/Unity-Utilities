using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TransformCurveAnimator : MonoBehaviour
{  
	//this is for camera walk noise 
	[System.Serializable] public struct CurveData
	{
		[SerializeField, BoxGroup("Position")] public readonly AnimationCurve PositionX,PositionY,PositionZ;
		[SerializeField, BoxGroup("Rotation")] public readonly AnimationCurve RotationX,RotationY,RotationZ;
	}

	CurveData _data;
	float _timeStarted = -1;
	public void StartAnimation(CurveData data)
	{
		_data = data;

	}
	public void StopAnimation() => _timeStarted = -1;


	void Update()
	{
		if (_timeStarted == -1)
			return;

		float currentTime = Time.time - _timeStarted;

		var localPosition = new Vector3();  
		if (_data.PositionX != null) localPosition.x = _data.PositionX.Evaluate(currentTime);
		if (_data.PositionY != null) localPosition.y = _data.PositionY.Evaluate(currentTime);
		if (_data.PositionZ != null) localPosition.z = _data.PositionZ.Evaluate(currentTime);

		var localRot = new Vector3();
		if (_data.RotationX != null) localRot.x = _data.RotationX.Evaluate(currentTime);
		if (_data.RotationY != null) localRot.y = _data.RotationY.Evaluate(currentTime);
		if (_data.RotationZ != null) localRot.z = _data.RotationZ.Evaluate(currentTime);

		transform.localPosition = localPosition;
		transform.localRotation = Quaternion.Euler(localRot);
	}
}
