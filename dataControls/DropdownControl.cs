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
			DropDownList ourDropDown = (DropDownList)ourControl.Parent.FindControl(ourControl.ID);
			int output;
			return int.TryParse(ourDropDown.SelectedValue, out output) ? output : Convert.ChangeType(ourDropDown.SelectedValue, ourType);

		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			DropDownList ourDropDown = new DropDownList { ID = "control" + field.ID };

			XmlDBBase lookupDB = new XmlDBBase
									{
										ConnectionString = ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString,
										TableName = field.Attributes["lookuptable"]
									};

			SqlDataSource datasource = lookupDB.DataSource(true, true, false, false, false);

			datasource.FilterExpression = String.Format("{0} = '{1}'", field.Attributes["lookupfilter"], field.Attributes["lookupfiltervalue"]);

			ourDropDown.DataSource = datasource;
			ourDropDown.DataValueField = lookupDB.TablePrimaryKeyField;
			ourDropDown.DataTextField = field.Attributes["lookuptextfield"];
			ourDropDown.DataBind();
			PropertyInfo ourProperty = ourPage.GetType().GetProperty(field.ID);

			if (PKey > 0 && ourProperty != null)
			{
				int ourValue = (int)ourProperty.GetValue(ourPage, null);

				int iPosition = 0;
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
