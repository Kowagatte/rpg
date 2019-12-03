using System;
using UnityEditor;
public static class LoaderTester
{

	[MenuItem("JsonTester/LoadItems")]
	private static void LoadItems()
	{
		ItemLoader.ClearInventory();
		ItemLoader.Init();
	}
	[MenuItem("JsonTester/LoadEntities")]
	private static void Entities()
	{
		EntityLoader.ClearList();
		EntityLoader.Init();
	}
}