using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Inventory.Data;
using Keys = JsonKeys.JsonKeys;


public enum DropRate { Trash, Common, Unique, Rare, Legendary };
public static class ItemLoader
{
	private static bool initialized = false;
	private static string path;
	private static ItemData[] data;
	private static Dictionary<DropRate, List<ItemData>> dropTable = new Dictionary<DropRate, List<ItemData>>();
	public static int ItemCount => data.Length;

	public static void Init()
	{
		if (string.IsNullOrEmpty(path)) path = Path.Combine(Application.dataPath, "Items.json");
		if (initialized || !File.Exists(path)) return;
		JObject parsedJson = JObject.Parse(File.ReadAllText(path));
		JArray array = parsedJson["Items"] as JArray;
		data = new ItemData[array.Count];

		foreach (DropRate rate in System.Enum.GetValues(typeof(DropRate)))
		{
			dropTable.Add(rate, new List<ItemData>());
		}

		int count = 0;
		foreach (var val in array)
		{
			switch ((ItemType)val[Keys.KEY_ITEMTYPE].Value<int>())
			{
				case ItemType.Consumable:
					data[count] = new ConsumableData(val);
					break;
				case ItemType.Equipable:
					data[count] = new EquipmentData(val);
					break;
			}
			Debug.Log(data[count]);
			dropTable[data[count].DropRate].Add(data[count]);
			count++;
		}
		initialized = true;
	}

	public static void ClearInventory(){
		initialized = false;
		dropTable.Clear();
	}


	public static ItemData GetItem(int id)
	{
		if (!initialized) Init();
		if (id > data.Length || id < 0) return null;
		return data[id];
	}

	public static ItemData GetFromDropTable(DropRate rate)
	{
		if (!initialized || dropTable[rate].Count == 0) Init();
		return dropTable[rate][Random.Range(0, dropTable[rate].Count)];
	}
}
