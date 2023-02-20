using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefValue 
{ 
    public readonly string name;
    public PlayerPrefValue(string name)
    {
        this.name = name;   
    }  
    public bool HasValue => PlayerPrefs.HasKey(name);
    public int IntValue => PlayerPrefs.GetInt(name);
    public void SetInt(int value) => PlayerPrefs.SetInt(name, value); 
}
