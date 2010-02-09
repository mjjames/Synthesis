using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Reflection;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem.dataControls
{
	public class HiddenControl : IDataControl
	{
		public int PKey { get; set; }

		public static object GetDataValue(Control ourControl, Type ourType)
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
			return Convert.ChangeType(ourHidden.Value, ourType);
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			HiddenField ourHidden = new HiddenField {ID = "control" + field.ID};
			PropertyInfo ourProperty = ourPage.GetType().GetProperty(field.ID);
			if (PKey > 0 && ourProperty != null)
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
