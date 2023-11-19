using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("stats", "statsInventoryMenu", "statsCharacterMenu", "baseStatsCharacterMenu", "derrivedStatsCharacterMenu", "expBar", "statDetailsText", "statPointsDetailsText", "statDataSO", "increaseButtons", "decreaseButtons", "applyButton", "revertButton", "enabled")]
	public class ES3UserType_CharacterUIManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CharacterUIManager() : base(typeof(CharacterUIManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (CharacterUIManager)obj;
			
			writer.WritePropertyByRef("stats", instance.stats);
			writer.WritePropertyByRef("statsInventoryMenu", instance.statsInventoryMenu);
			writer.WritePropertyByRef("statsCharacterMenu", instance.statsCharacterMenu);
			writer.WritePropertyByRef("baseStatsCharacterMenu", instance.baseStatsCharacterMenu);
			writer.WritePropertyByRef("derrivedStatsCharacterMenu", instance.derrivedStatsCharacterMenu);
			writer.WritePropertyByRef("expBar", instance.expBar);
			writer.WritePropertyByRef("statDetailsText", instance.statDetailsText);
			writer.WritePropertyByRef("statPointsDetailsText", instance.statPointsDetailsText);
			writer.WritePropertyByRef("statDataSO", instance.statDataSO);
			writer.WritePrivateField("increaseButtons", instance);
			writer.WritePrivateField("decreaseButtons", instance);
			writer.WritePropertyByRef("applyButton", instance.applyButton);
			writer.WritePropertyByRef("revertButton", instance.revertButton);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (CharacterUIManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "stats":
						instance.stats = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "statsInventoryMenu":
						instance.statsInventoryMenu = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "statsCharacterMenu":
						instance.statsCharacterMenu = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "baseStatsCharacterMenu":
						instance.baseStatsCharacterMenu = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "derrivedStatsCharacterMenu":
						instance.derrivedStatsCharacterMenu = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "expBar":
						instance.expBar = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "statDetailsText":
						instance.statDetailsText = reader.Read<TMPro.TMP_Text>();
						break;
					case "statPointsDetailsText":
						instance.statPointsDetailsText = reader.Read<TMPro.TMP_Text>();
						break;
					case "statDataSO":
						instance.statDataSO = reader.Read<UIStatDataSO>();
						break;
					case "increaseButtons":
					instance = (CharacterUIManager)reader.SetPrivateField("increaseButtons", reader.Read<System.Collections.Generic.Dictionary<System.String, UnityEngine.UI.Button>>(), instance);
					break;
					case "decreaseButtons":
					instance = (CharacterUIManager)reader.SetPrivateField("decreaseButtons", reader.Read<System.Collections.Generic.Dictionary<System.String, UnityEngine.UI.Button>>(), instance);
					break;
					case "applyButton":
						instance.applyButton = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "revertButton":
						instance.revertButton = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_CharacterUIManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CharacterUIManagerArray() : base(typeof(CharacterUIManager[]), ES3UserType_CharacterUIManager.Instance)
		{
			Instance = this;
		}
	}
}