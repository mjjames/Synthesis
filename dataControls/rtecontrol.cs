using System;
using System.Web;
using FredCK.FCKeditorV2;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace mjjames.AdminSystem.dataControls
{
	public class RteControl : KeyValuePairControl, IDataControl
	{

		public static object GetDataValue(Control ourControl, Type ourType)
		{
			var ourFCK = (FCKeditor)ourControl;
			return ourFCK.Value;
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			var fckEditor = new FCKeditor
			                      	{
			                      		ID = "control" + field.ID,
			                      		CustomConfigurationsPath = "/admininc/fckSettings.js?v=1",
			                      		BasePath = "~/fckeditor/",
			                      		ToolbarCanCollapse = false,
			                      		ToolbarSet = "mjjames",
			                      		EnableSourceXHTML = true,
			                      		EnableXHTML = true,
			                      		FormatOutput = true,
			                      		FormatSource = true
			                      	};
			fckEditor.EnableSourceXHTML = true;
			fckEditor.HtmlEncodeOutput = true;
			//	fckEditor.Config["HtmlEncodeOutput"] = "true";

			var unit = UnitType.Percentage;

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

			var ourValue = GetStringValue(field, ourPage);
			fckEditor.Value = HttpContext.Current.Server.HtmlDecode("" + ourValue);
			return fckEditor;
		}
	}
}
