using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;
using System.Reflection;
using mjjames.AdminSystem.dao;

namespace mjjames.AdminSystem.dataControls
{
	public class TextControl : IDataControl
	{
		public int PKey { get; set; }

		/// <summary>
		/// Returns the Data Value from our control
		/// </summary>
		/// <param name="ourControl">Our Text Control</param>
		/// <param name="ourType">DateType in the control</param>
		/// <returns>Actual Data</returns>
		public static object GetDataValue(Control ourControl, Type ourType)
		{
			TextBox ourTextBox = (TextBox)ourControl;
			if (ourTextBox.Text == "" && !ourType.FullName.Contains("String"))
			{
				return null;
			}
			return ourType.FullName.Contains("Int") ? int.Parse("" + ourTextBox.Text) : Convert.ChangeType(ourTextBox.Text, ourType);
		}

		/// <summary>
		/// Builds a TextControl for render
		/// </summary>
		/// <param name="field">Field Data</param>
		/// <param name="ourPage">Page Data</param>
		/// <returns>WebControl</returns>
		public Control GenerateControl(AdminField field, object ourPage)
		{
			TextBox ourControl = new TextBox {ID = "control" + field.ID, CssClass = "field textbox"};
			if (field.Attributes.ContainsKey("maxlength"))
			{
				int iMaxLength = int.Parse(field.Attributes["maxlength"]);
				if (iMaxLength > 0)
				{
					ourControl.MaxLength = iMaxLength;
					ourControl.Columns = iMaxLength;
				}
			}
			if (field.Attributes.ContainsKey("rows"))
			{
				int iRows = int.Parse(field.Attributes["rows"]);
				if (iRows > 0)
				{
					ourControl.TextMode = TextBoxMode.MultiLine;
					ourControl.Rows = iRows;
					ourControl.Wrap = true;
					ourControl.Columns = 40;
				}
			}
			string ourValue = string.Empty;

			if(field.Attributes.ContainsKey("format") && field.Attributes["format"].Equals("number", StringComparison.InvariantCultureIgnoreCase)){
				ourValue = GetNumericValue(field, ourPage);
				ourControl.CssClass = "field number"; //if we of number type then we must overwrie the css class
			}
			else{
				ourValue = GetStringValue(field, ourPage);
			}
			
			ourControl.Text = "" + ourValue;

			return ourControl;
		}

		private string GetStringValue(AdminField field, object ourPage)
		{
			if (PKey == 0)
			{
				return null;
			}

			if(field.Attributes.ContainsKey("lookupid")){
				return GetStringValueFromKeyValuePair(field.Attributes["lookupid"], ourPage);
			}
			else{
				return GetStringValueFromProperty(field.ID, DataType.String, ourPage);
			}
		}

		private string GetStringValueFromKeyValuePair(string lookupID, object ourPage)
		{
            KeyValueRepository kvr = new KeyValueRepository();
            string sourceType = String.Empty;
            switch (ourPage.GetType().Name)
            {
                case "XmlDBpages":
                    sourceType = "pagelookup";
                    break;
                case "XmlDBprojects":
                    sourceType = "projectlookup";
                    break;
            }
            return kvr.GetKeyValue(lookupID, PKey, sourceType);
		}

		private string GetNumericValue(AdminField field, object ourPage){
			if(PKey == 0)
			{
				return null;
			}
			return GetStringValueFromProperty(field.ID, DataType.Numeric, ourPage);
		}

		private string GetStringValueFromProperty(string fieldID, DataType type, object data)
		{
			if (PKey == 0)
			{
				return null;
			}
			var property = GetProperty(fieldID, type, data);
			
			if(property == null){
				return string.Empty;
			}

			if(type == DataType.String){
				return (string)property.GetValue(data, null);
			}

			return (property.GetValue(data, null) as int?).ToString();
		}

		private PropertyInfo GetProperty(string fieldID, DataType type, object data){
			if(type == DataType.String){
				return data.GetType().GetProperty(fieldID, typeof(string));
			}
			return data.GetType().GetProperty(fieldID, typeof(int?));
		}

		private enum DataType
		{
			String,
			Numeric
		}
	}
}