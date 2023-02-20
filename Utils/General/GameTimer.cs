using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class GameTimer 
{
    //public enum TimeType { Scaled, Unscaled, Realtime }

    [ShowInInspector] public float realtimeStarted { get; private set; } 
	public float timeScaledStarted { get; private set; }
	public float timeUnscaledStarted { get; private set; }
	[ShowInInspector] public float realtime => Time.realtimeSinceStartup - realtimeStarted;
	public float timeScaled => Time.time - timeScaledStarted;
	public float timeUnscaled => Time.unscaledTime - timeUnscaledStarted;
    [ShowInInspector] public bool ReachedRealMaxTime =>	realtime >= MaxTime;
	public bool ReachedScaledMaxTime =>	timeScaled >= MaxTime;
	public bool ReachedUnscaledMaxTime =>	timeUnscaled >= MaxTime; 
	public float MaxTime { get; private set; }
    [ShowInInspector] public float NormalizedMaxTime => Normalized(MaxTime);
	public float Normalized(float maxTime) => Mathf.Clamp01((Time.time - timeScaledStarted) / maxTime);

	public void Restart()
	{
		realtimeStarted = Time.realtimeSinceStartup;
		timeScaledStarted = Time.time;
		timeUnscaledStarted = Time.unscaledTime;
	}

	public GameTimer(float maxTime = 1f) 
	{
		realtimeStarted = Time.realtimeSinceStartup; 
		timeScaledStarted = Time.time; 
		timeUnscaledStarted = Time.unscaledTime;
		MaxTime = maxTime;
		Restart();
	}  
}
