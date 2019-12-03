using Newtonsoft.Json.Linq;
using Keys = JsonKeys.JsonKeys;
using Json.Data;
namespace Inventory.Data
{
	public class EquipmentData : ItemData
	{

		//TODO - add list of todo actions built from C# structs
		//public List<Status> statusEffects 
		public EquipmentType EquipmentSlot {get; protected set;}
		public FloatRange Damage {get; protected set;}

		public EquipmentData(JToken jToken) : base(jToken)
		{
			ItemType = ItemType.Equipable;
			Damage = jToken[Keys.KEY_DAMAGE].Value<string>();
			Output = "Name : {0}, Is Equipable, Damage ({1} to {2}), In {3} Drop Table, Description : {4}";
		}

		public override string ToString()
		{
			return string.Format(Output, Name, Damage.Min,Damage.Max,DropRate,Description);
		}
	}
}