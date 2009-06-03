using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Reflection;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem.dataControls
{
	public class hiddenControl
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
			HiddenField ourHidden = (HiddenField)ourControl;
			HttpContext.Current.Trace.Write("Saving Content Value: " + ourHidden.Value);
			if (ourType.FullName.Contains("Int"))
			{
				if (ourHidden.Value == "")
				{
					return null;
				}
				HttpContext.Current.Trace.Write("Saving Content Value As Int: " + int.Parse(ourHidden.Value));
				return int.Parse("" + ourHidden.Value);
			}
			else
			{
				return Convert.ChangeType(ourHidden.Value, ourType);
			}
		}

		public Control generateControl(AdminField field, object ourPage)
		{
			PropertyInfo ourProperty;
			HiddenField ourHidden = new HiddenField();
			ourHidden.ID = "control" + field.ID;
			ourProperty = ourPage.GetType().GetProperty(field.ID);
			if (iPKey > 0 && ourProperty != null)
			{
				string ourValue = (ourProperty.GetValue(ourPage, null) + "");
				ourHidden.Value = "" + ourValue;
				HttpContext.Current.Trace.Warn("Rendering Control Value: " + ourHidden.Value);
			}
			///TODO work out how to hide the label ourLabel.CssClass = "hidden";
			return ourHidden;
		}
	}
}
