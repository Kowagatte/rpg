using System;
using UnityEngine;

namespace Json.Data
{
	public struct IntRange
	{
		public int Min;
		public int Max;
		public bool Usable => Min != Max;

		public IntRange(string input)
		{
			Min = 0;
			Max = 0;
			string[] split = input.Split(':');
			if (split.Length == 2)
			{
				if (int.TryParse(split[0], out int min)) Min = min;
				else Debug.LogError("Learn to format a json, your range value for an item is wrong");
				if (int.TryParse(split[1], out int max)) Max = max;
				else Debug.LogError("Learn to format a json, your range value for an item is wrong");
			}
			else Debug.LogError("Learn to format a json, your range value for an item is wrong");
		}
		public IntRange(int a, int b)
		{
			Min = Math.Min(a, b);
			Max = Math.Max(a, b);
		}

		public int GetValue()
		{
			return UnityEngine.Random.Range(Min, Max);
		}

		public override string ToString()
		{
			return $"{Min} - {Max}";
		}

		public static implicit operator IntRange(string s) => new IntRange(s);
	}
}