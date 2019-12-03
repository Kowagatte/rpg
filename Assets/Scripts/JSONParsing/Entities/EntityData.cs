using Newtonsoft.Json.Linq;
using Keys = JsonKeys.JsonKeys;
using Json.Data;
namespace Entity.Data
{
	public class EntityData
	{

		public string Name { get; protected set; }
		public string Description { get; protected set; }
		public string Prefab { get; protected set;}

		public IntRange HP { get; protected set; }
		public IntRange Mana { get; protected set; }
		public IntRange Strength { get; protected set; }
		public IntRange Constitution { get; protected set; }
		public IntRange Intellect { get; protected set; }
		public IntRange Dexterity { get; protected set; }

		public int HPScaling { get; protected set; }
		public int ManaScaling { get; protected set; }
		public int StrengthScaling { get; protected set; }
		public int ConstitutionScaling { get; protected set; }
		public int IntellectScaling { get; protected set; }
		public int DexterityScaling { get; protected set; }

		protected string Output = " Name : {0},\n Description : {1},\n HP :{2},\n Mana : {3},\n Strength : {4},\n Constitution : {5},\n Intellect : {6},\n Dexterity : {7}";

		public EntityData(JToken jToken)
		{
			Name = jToken[Keys.KEY_NAME].Value<string>();
			Description = jToken[Keys.KEY_DESCRIPTION].Value<string>();
			Prefab = jToken[Keys.KEY_PREFAB].Value<string>();

			//Get Base Data
			HP = jToken[Keys.KEY_HP].Value<string>();
			Mana = jToken[Keys.KEY_MANA].Value<string>();
			Strength = jToken[Keys.KEY_STRENGTH].Value<string>();
			Constitution = jToken[Keys.KEY_CONSTITUTION].Value<string>();
			Intellect = jToken[Keys.KEY_CONSTITUTION].Value<string>();
			Dexterity = jToken[Keys.KEY_DEXTERITY].Value<string>();

			//Get Scaling Data
			HPScaling = jToken[Keys.KEY_SCALING][Keys.KEY_HP].Value<int>();
			ManaScaling = jToken[Keys.KEY_SCALING][Keys.KEY_MANA].Value<int>();
			StrengthScaling = jToken[Keys.KEY_SCALING][Keys.KEY_STRENGTH].Value<int>();
			ConstitutionScaling = jToken[Keys.KEY_SCALING][Keys.KEY_CONSTITUTION].Value<int>();
			IntellectScaling = jToken[Keys.KEY_SCALING][Keys.KEY_INTELLECT].Value<int>();
			DexterityScaling = jToken[Keys.KEY_SCALING][Keys.KEY_DEXTERITY].Value<int>();
		}

		public override string ToString()
		{
			return string.Format(Output, Name, Description, HP, Mana, Strength, Constitution, Intellect, Dexterity);
		}
	}
}
