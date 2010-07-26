using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;
using System.Reflection;
using mjjames.AdminSystem.dao;
using System.Text;
using System.Web;
using System.Collections.Generic;

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
			
			if(ourType.FullName.Contains("Int")){
				return int.Parse("" + ourTextBox.Text);
			}
			if(ourType.FullName.Contains("Char")){
				return ourTextBox.Text.ToCharArray()[0];
			}
			return Convert.ChangeType(ourTextBox.Text, ourType);
		}

		/// <summary>
		/// Builds a TextControl for render
		/// </summary>
		/// <param name="field">Field Data</param>
		/// <param name="ourPage">Page Data</param>
		/// <returns>WebControl</returns>
		virtual public Control GenerateControl(AdminField field, object ourPage)
		{
			var ourControl = new TextBox {ID = "control" + field.ID, CssClass = "field textbox control" + field.ID};

			var validationScript = new StringBuilder();
			var validationRules = new List<string>();

			if (field.Attributes.ContainsKey("maxlength"))
			{
				var iMaxLength = int.Parse(field.Attributes["maxlength"]);
				if (iMaxLength > 0)
				{
					//validationScript.AppendFormat(".maxlength({0})",iMaxLength);
					validationRules.Add(String.Format("maxlength:{0}", iMaxLength));
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

			if (field.Attributes.ContainsKey("required") && field.Attributes["required"] == "1")
			{
				validationRules.Add("required:true");
			}

			if (field.Attributes.ContainsKey("validation"))
			{
				switch (field.Attributes["validation"].ToLower())
				{
					case "url":
						validationRules.Add("url: true");
						break;
					case "email":
						validationRules.Add("email:true");
						break;
					case "date":
						validationRules.Add("date:true");
						break;
					default:
						break;
				}
			}

			string ourValue;

			if(field.Attributes.ContainsKey("format") && field.Attributes["format"].Equals("number", StringComparison.InvariantCultureIgnoreCase)){
				ourValue = GetNumericValue(field, ourPage);
				ourControl.CssClass = "field number control" + field.ID; //if we of number type then we must overwrie the css class
				validationRules.Add("digits:true");
			}
			else{
				ourValue = GetStringValue(field, ourPage);
			}
			
			ourControl.Text = "" + ourValue;

			//if we have validation options then register validation script
			if (validationRules.Count > 0)
			{
				var page = (Page)HttpContext.Current.Handler;
				var ourSM = ScriptManager.GetCurrent(page);
				var csm = page.ClientScript;
				if(!csm.IsClientScriptIncludeRegistered("jQueryValidation")){
					csm.RegisterClientScriptInclude("jQueryValidation", "http://ajax.microsoft.com/ajax/jquery.validate/1.7/jquery.validate.min.js");
				}
				
				//if we haven't already on document ready setup form validation
				if (!csm.IsStartupScriptRegistered("ValidationFormScript"))
				{
					csm.RegisterStartupScript(this.GetType(), "ValidationFormScript", "$(document).ready(function(){ $('form').validate(); });", true);
				}

				//add our control specific validation
				validationScript.AppendLine("$(document).ready(function(){ $('input.control" + field.ID + "')");
				validationScript.AppendLine(".rules('add', {");
				validationScript.AppendLine(String.Join(",", validationRules.ToArray()));
				validationScript.AppendLine("}); });");
				csm.RegisterStartupScript(this.GetType(), "ValidationScript" + field.ID, validationScript.ToString(), true);


			
			}

			return ourControl;
		}


	}
}