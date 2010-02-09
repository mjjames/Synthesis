﻿using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem.dataControls
{
	public class CheckboxControl : IDataControl
	{
		public int PKey { get; set; }

		public static object GetDataValue(Control ourControl, Type ourType){
			CheckBox ourCheck = (CheckBox)ourControl;
			return ourCheck.Checked;
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			CheckBox ourCheckBox = new CheckBox {ID = "control" + field.ID, CssClass = "field"};

			PropertyInfo ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(bool?)) ??
			                           ourPage.GetType().GetProperty(field.ID, typeof(bool));
			if (PKey > 0 && ourProperty != null)
			{
				bool ourValue;
				string ourValueAsString = ourProperty.GetValue(ourPage, null) != null ? ourProperty.GetValue(ourPage, null).ToString() :  String.Empty;
				bool.TryParse(ourValueAsString, out ourValue);
				ourCheckBox.Checked = ourValue;
			}

			return ourCheckBox;
			
		}

	}
}
