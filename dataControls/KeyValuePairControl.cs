using System;
using System.Reflection;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.Repositories;

namespace mjjames.AdminSystem.DataControls
{
	public class KeyValuePairControl
	{
		
		public int PKey { get; set; }

		internal string GetStringValue(AdminField field, object ourPage)
		{
			if (PKey == 0)
			{
				return null;
			}

			return field.Attributes.ContainsKey("lookupid") ?
					GetStringValueFromKeyValuePair(field.Attributes["lookupid"], ourPage) :
					GetStringValueFromProperty(field.ID, DataType.String, ourPage);
		}

		internal string GetStringValueFromKeyValuePair(string lookupID, object ourPage)
		{
			var kvr = new KeyValueRepository();
			var sourceType = String.Empty;
			switch (ourPage.GetType().Name)
			{
				case "page":
					sourceType = "pagelookup";
					break;
				case "project":
					sourceType = "projectlookup";
					break;
				case "marketingsite":
					sourceType = "marketingsitelookup";
					break;
				case "media":
					sourceType = "medialookup";
					break;
			}
			return kvr.GetKeyValue(lookupID, PKey, sourceType);
		}

		internal string GetNumericValue(AdminField field, object ourPage)
		{
			return PKey == 0 ? null : GetStringValueFromProperty(field.ID, DataType.Numeric, ourPage);
		}

		internal string GetStringValueFromProperty(string fieldID, DataType type, object data)
		{
			if (PKey == 0)
			{
				return null;
			}
			var property = GetProperty(fieldID, type, data);

			if (property == null)
			{
				return string.Empty;
			}

			if (type == DataType.String)
			{
				return (string)property.GetValue(data, null);
			}

			return (property.GetValue(data, null) as int?).ToString();
		}

		private static PropertyInfo GetProperty(string fieldID, DataType type, object data)
		{
			return data.GetType().GetProperty(fieldID, type == DataType.String ? typeof(string) : typeof(int?));
		}

		internal enum DataType
		{
			String,
			Numeric
		}
	}
}
