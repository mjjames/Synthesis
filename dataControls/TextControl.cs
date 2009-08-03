using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.dataentities;
using System.Reflection;

namespace mjjames.AdminSystem.dataControls
{
	public class textControl
	{
		private int _iPKey;

		public int iPKey
		{
			get
			{
				return _iPKey;
			}
			set
			{
				_iPKey = value;
			}
		}

		public static object getDataValue(Control ourControl, Type ourType)
		{
			TextBox ourTextBox = (TextBox)ourControl;
			if (ourTextBox.Text == "" && !ourType.FullName.Contains("String"))
			{
				return null;
			}
			if (ourType.FullName.Contains("Int"))
			{

				return int.Parse("" + ourTextBox.Text);
			}
	        return Convert.ChangeType(ourTextBox.Text, ourType);
		}

		/// <summary>
		/// Builds a TextControl for render
		/// </summary>
		/// <param name="field">Field Data</param>
		/// <param name="ourPage">Page Data</param>
		/// <param name="iPKey">Primary Key</param>
		/// <returns>WebControl</returns>
		public Control generateControl(AdminField field, object ourPage)
		{
			PropertyInfo ourProperty;
			TextBox ourControl = new TextBox();
			ourControl.ID = "control" + field.ID;
			ourControl.CssClass = "field textbox";
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
				ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(int?));
				ourControl.CssClass = "field number"; //if we of number type then we must overwrie the css class
				if (iPKey > 0 && ourProperty != null)
				{
					ourValue = ourProperty.GetValue(ourPage, null) != null ? (ourProperty.GetValue(ourPage, null) as int?).ToString() : string.Empty;
				}
			}
			else{
				ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(string));
				if (iPKey > 0 && ourProperty != null)
				{
					ourValue = ourProperty.GetValue(ourPage, null) != null ? (string) ourProperty.GetValue(ourPage, null) : string.Empty;
				}
			}
			ourControl.Text = "" + ourValue;

			return ourControl;
		}
	}
}
