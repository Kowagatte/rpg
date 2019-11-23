using Newtonsoft.Json.Linq;
using Keys = JsonKeys.JsonKeys;
namespace Inventory.Data
{
	public class ConsumableData : ItemData
	{

		//TODO - add list of todo actions built from C# structs
		//public List<OnUse> onUse 

		public ConsumableData(JToken jToken) : base(jToken)
		{
			ItemType = ItemType.Consumable;
			Output = "Name : {0}, Is Consumable, Description : {1}, In Drop Table : {2}";
		}
		public override string ToString()
		{
			return string.Format(Output, Name, Description,DropRate);
		}
	}
}
