using System;
using UnityEngine;

namespace Json.Data
{
    public struct FloatRange{
	public float Min;
	public float Max;
	public bool Usable => Min != Max;

	public FloatRange(string input){
		Min = 0;
		Max = 0;
		string[] split = input.Split(':');
		if(split.Length == 2){
			if(float.TryParse(split[0],out float min)) Min = min;
			else Debug.LogError("Learn to format a json, your range value for an item is wrong");
			if(float.TryParse(split[1], out float max)) Max = max;
			else Debug.LogError("Learn to format a json, your range value for an item is wrong");
		}else Debug.LogError("Learn to format a json, your range value for an item is wrong");
	}
	public FloatRange(float a, float b){
		Min = Math.Min(a,b);
		Max = Math.Max(a,b);
	}

	public float GetValue(){
		return UnityEngine.Random.Range(Min,Max);
	}
	 public static implicit operator FloatRange(string s) => new FloatRange(s);


    }
}