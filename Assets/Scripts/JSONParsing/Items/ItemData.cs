using Newtonsoft.Json.Linq;
using Keys = JsonKeys.JsonKeys;
namespace Inventory.Data
{
	public enum ItemType { Consumable, Equipable };
	public enum EquipmentType { OneHanded, TwoHanded, Shield, }
	public abstract class ItemData
	{

		public string Name { get; protected set; }
		public string Description { get; protected set; }
		public string Prefab{get;protected set;}
		public ItemType ItemType { get; protected set; }
		public DropRate DropRate = DropRate.Trash;

		protected string Output = "Name : {0}, Description : {1})";

		public ItemData(JToken jToken)
		{
			Name = jToken[Keys.KEY_NAME].Value<string>();
			Description = jToken[Keys.KEY_DESCRIPTION].Value<string>();
			Prefab = jToken[Keys.KEY_PREFAB].Value<string>();
			DropRate = (DropRate)jToken[Keys.KEY_DROPRATE].Value<int>();
		}

		public override string ToString()
		{
			return string.Format(Output, Name, Description);
		}


	}
}
