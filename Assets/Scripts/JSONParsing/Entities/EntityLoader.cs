using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Entity.Data;
using Json.Data;
using Keys = JsonKeys.JsonKeys;


public static class EntityLoader
{
	private static bool initialized = false;
	private static string path;
	private static EntityData[] data;
	public static int EntityData => data.Length;

	public static void Init()
	{
		if (string.IsNullOrEmpty(path)) path = Path.Combine(Application.dataPath, "Entities.json");
		if (initialized || !File.Exists(path)) return;
		JObject parsedJson = JObject.Parse(File.ReadAllText(path));
		JArray array = parsedJson["Entities"] as JArray;
		data = new EntityData[array.Count];


		int count = 0;
		foreach (var val in array)
		{
			data[count] = new EntityData(val);
			Debug.Log(data[count]);
			count++;
		}
		initialized = true;
	}

	public static void ClearList(){
		initialized = false;
	}


	public static EntityData GetData(int id)
	{
		if (!initialized) Init();
		if (id > data.Length || id < 0) return null;
		return data[id];
	}
}
