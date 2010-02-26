using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;
using System.Configuration;

namespace mjjames.AdminSystem.dataControls
{
	public class DropdownControl : IDataControl
	{
		public int PKey { get; set; }

		public static object GetDataValue(Control ourControl, Type ourType)
		{
			var ourDropDown = (DropDownList)ourControl.Parent.FindControl(ourControl.ID);
			int output;
			return int.TryParse(ourDropDown.SelectedValue, out output) ? output : Convert.ChangeType(ourDropDown.SelectedValue, ourType);

		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			var ourDropDown = new DropDownList { ID = "control" + field.ID };

			var lookupDB = new XmlDBBase(false)
									{
										ConnectionString = ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString,
										TableName = field.Attributes["lookuptable"]
									};

			var datasource = lookupDB.DataSource(true, true, false, false, false);

			datasource.FilterExpression = String.Format("{0} = '{1}'", field.Attributes["lookupfilter"], field.Attributes["lookupfiltervalue"]);

			ourDropDown.DataSource = datasource;
			ourDropDown.DataValueField = lookupDB.TablePrimaryKeyField;
			ourDropDown.DataTextField = field.Attributes["lookuptextfield"];
			ourDropDown.DataBind();
			var ourProperty = ourPage.GetType().GetProperty(field.ID);

			if (PKey > 0 && ourProperty != null)
			{
				var ourValue = (int)ourProperty.GetValue(ourPage, null);

				var iPosition = 0;
				foreach (ListItem item in ourDropDown.Items)
				{
					if (item.Value == ourValue.ToString())
					{
						item.Selected = true;
						ourDropDown.SelectedIndex = iPosition;
						break;
					}
					iPosition++;
				}

				HttpContext.Current.Trace.Warn("Rendering Control Value: " + ourDropDown.SelectedValue);
			}

			return ourDropDown;
		}
	}
}
