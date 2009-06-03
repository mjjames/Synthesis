using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FredCK.FCKeditorV2;
using mjjames.AdminSystem.dataentities;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Web.UI;

namespace mjjames.AdminSystem.dataControls
{
	public class rteControl
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
			FCKeditor ourFCK = (FCKeditor)ourControl;
			return ourFCK.Value;
		}

		public Control generateControl(AdminField field, object ourPage)
		{
			PropertyInfo ourProperty;

			FCKeditor fckEditor = new FCKeditor();
			fckEditor.ID = "control" + field.ID;
			fckEditor.CustomConfigurationsPath = "/admininc/fckSettings.js?v=1";
			fckEditor.BasePath = "~/fckeditor/";
			fckEditor.ToolbarCanCollapse = false;
			fckEditor.ToolbarSet = "mjjames";
			fckEditor.EnableSourceXHTML = true;
			fckEditor.EnableXHTML = true;
			fckEditor.FormatOutput = true;
			fckEditor.FormatSource = true;
			fckEditor.EnableSourceXHTML = true;
			fckEditor.HtmlEncodeOutput = true;
			//	fckEditor.Config["HtmlEncodeOutput"] = "true";

			UnitType unit = UnitType.Percentage;

			if (field.Attributes.ContainsKey("unit"))
			{
				unit = (UnitType)Enum.Parse(typeof(UnitType), field.Attributes["unit"]);
			}

			if (field.Attributes.ContainsKey("toolbar"))
			{
				fckEditor.ToolbarSet = field.Attributes["toolbar"];
			}


			if (field.Attributes.ContainsKey("width"))
			{
				switch (unit)
				{

					case UnitType.Percentage:
						fckEditor.Width = Unit.Percentage(double.Parse(field.Attributes["width"]));
						break;
					case UnitType.Pixel:
						fckEditor.Width = Unit.Pixel(int.Parse(field.Attributes["width"]));
						break;

					default:
						fckEditor.Width = Unit.Percentage(100);
						break;

				}

			}
			else
			{
				fckEditor.Width = Unit.Percentage(100);
			}


			if (field.Attributes.ContainsKey("height"))
			{
				switch (unit)
				{

					case UnitType.Percentage:
						fckEditor.Height = Unit.Percentage(double.Parse(field.Attributes["height"]));
						break;
					case UnitType.Pixel:
						fckEditor.Height = Unit.Pixel(int.Parse(field.Attributes["height"]));
						break;

					default:
						fckEditor.Height = Unit.Percentage(100);
						break;

				}

			}
			else
			{
				fckEditor.Height = Unit.Pixel(450);
			}

			ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(string));
			if (iPKey > 0 && ourProperty != null)
			{
				string ourValue = (string)ourProperty.GetValue(ourPage, null);
				fckEditor.Value = HttpContext.Current.Server.HtmlDecode("" + ourValue);
			}
			return (Control) fckEditor;
		}
	}
}
