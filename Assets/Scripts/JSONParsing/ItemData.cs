using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Keys = JsonKeys.JsonKeys;
namespace Inventory.Data
{
	public class ItemData
	{

		public string Name { get; protected set; }
		public string Description { get; protected set; }
		public float MinDamage { get; protected set; }
		public float MaxDamage { get; protected set; }
		public DropRate DropRate = DropRate.Trash;

		protected string Output = "Name : {0}, Description : {1}, Damage : ({2}-{3})";

		public ItemData(JToken jToken)
		{
			Name = jToken[Keys.KEY_NAME].Value<string>();
			Description = jToken[Keys.KEY_DESCRIPTION].Value<string>();
			MinDamage = jToken[Keys.KEY_MINDAMAGE].Value<float>();
			MaxDamage = jToken[Keys.KEY_MAXDAMAGE].Value<float>();
			DropRate =  (DropRate)jToken[Keys.KEY_DROPRATE].Value<int>();
			Debug.Log(this);
		}

		public override string ToString()
		{
			return string.Format(Output, Name, Description, MinDamage, MaxDamage);
		}


	}
}
