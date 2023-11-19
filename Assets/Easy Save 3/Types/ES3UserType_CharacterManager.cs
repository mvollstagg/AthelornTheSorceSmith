using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("characterDataSO")]
	public class ES3UserType_CharacterManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CharacterManager() : base(typeof(CharacterManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (CharacterManager)obj;
			
			writer.WritePropertyByRef("characterDataSO", instance.characterDataSO);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (CharacterManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "characterDataSO":
						instance.characterDataSO = reader.Read<CharacterDataSO>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_CharacterManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CharacterManagerArray() : base(typeof(CharacterManager[]), ES3UserType_CharacterManager.Instance)
		{
			Instance = this;
		}
	}
}