using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem;
using System.Configuration;

namespace mjjames.AdminSystem.dataControls
{
	public class dropdownControl
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
			DropDownList ourDropDown = (DropDownList) ourControl.Parent.FindControl(ourControl.ID); ;
			return Convert.ChangeType(ourDropDown.SelectedValue, ourType);
		}

		public Control generateControl(AdminField field, object ourPage)
		{
			PropertyInfo ourProperty;

			DropDownList ourDropDown = new DropDownList();
			ourDropDown.ID = "control" + field.ID;

			XmlDBBase lookupDB = new XmlDBBase();
			lookupDB.ConnectionString = ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString;
			lookupDB.TableName = field.Attributes["lookuptable"];

			SqlDataSource datasource = lookupDB.DataSource(true, true, false, false, false);

			datasource.FilterExpression =String.Format("{0} = '{1}'", field.Attributes["lookupfilter"], field.Attributes["lookupfiltervalue"]);

			ourDropDown.DataSource = datasource;
			ourDropDown.DataValueField = lookupDB.TablePrimaryKeyField;
			ourDropDown.DataTextField = field.Attributes["lookuptextfield"];
			ourDropDown.DataBind();
			ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(int));
		
			if (_iPKey > 0 && ourProperty != null)
			{
				int ourValue = (int) ourProperty.GetValue(ourPage, null);

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
