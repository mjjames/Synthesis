using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;
using System.Reflection;
using mjjames.AdminSystem.dao;

namespace mjjames.AdminSystem.dataControls
{
	public class TextControl : KeyValuePairControl, IDataControl
	{

		/// <summary>
		/// Returns the Data Value from our control
		/// </summary>
		/// <param name="ourControl">Our Text Control</param>
		/// <param name="ourType">DateType in the control</param>
		/// <returns>Actual Data</returns>
		public static object GetDataValue(Control ourControl, Type ourType)
		{
			var ourTextBox = (TextBox)ourControl;
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
		virtual public Control GenerateControl(AdminField field, object ourPage)
		{
			var ourControl = new TextBox {ID = "control" + field.ID, CssClass = "field textbox"};
			if (field.Attributes.ContainsKey("maxlength"))
			{
				var iMaxLength = int.Parse(field.Attributes["maxlength"]);
				if (iMaxLength > 0)
				{
					ourControl.MaxLength = iMaxLength;
					ourControl.Columns = iMaxLength;
				}
			}
			if (field.Attributes.ContainsKey("rows"))
			{
				var iRows = int.Parse(field.Attributes["rows"]);
				if (iRows > 0)
				{
					ourControl.TextMode = TextBoxMode.MultiLine;
					ourControl.Rows = iRows;
					ourControl.Wrap = true;
					ourControl.Columns = 40;
				}
			}
			string ourValue;

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

		
	}
}