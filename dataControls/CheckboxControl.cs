using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem.dataControls
{
	public class checkboxControl
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

		public static object getDataValue(Control ourControl, Type ourType){
			CheckBox ourCheck = (CheckBox)ourControl;
			return ourCheck.Checked;
		}

		public Control generateControl(AdminField field, object ourPage)
		{
			PropertyInfo ourProperty;
			CheckBox ourCheckBox = new CheckBox();
			ourCheckBox.ID = "control" + field.ID;
			ourCheckBox.CssClass = "field";

			ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(bool?));
			if (ourProperty == null) //if the property isnt a nullable bool then try a non nullable
			{
				ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(bool));
			}
			if (iPKey > 0 && ourProperty != null)
			{
				bool ourValue = false;
				string ourValueAsString = ourProperty.GetValue(ourPage, null) != null ? ourProperty.GetValue(ourPage, null).ToString() :  String.Empty;
				bool.TryParse(ourValueAsString, out ourValue);
				ourCheckBox.Checked = ourValue;
			}

			return ourCheckBox;
			
		}
	}
}
