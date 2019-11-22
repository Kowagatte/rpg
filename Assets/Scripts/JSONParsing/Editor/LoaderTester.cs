using System;
using UnityEditor;
public static class LoaderTester{

	[MenuItem("JsonTester/LoadItems")]
	private static void LoadItems(){
		ItemLoader.Init();
	}
}