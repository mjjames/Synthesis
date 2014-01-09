using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;
using System.Configuration;
using System.Linq;

namespace mjjames.AdminSystem.dataControls
{
	public class DropdownControl : KeyValuePairControl, IDataControl
	{
		
		public static object GetDataValue(Control ourControl, Type ourType)
		{
			var ourDropDown = (DropDownList)ourControl.Parent.FindControl(ourControl.ID);

			if (ourDropDown.SelectedValue == "" && !ourType.FullName.Contains("String"))
			{
				return null;
			}

			if (ourType.FullName.Contains("Int"))
			{
				int output;
				return int.TryParse(ourDropDown.SelectedValue, out output) ? output : Convert.ChangeType(ourDropDown.SelectedValue, ourType);
			}
			else
			{
				return ourDropDown.SelectedValue;
			}
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			var ourDropDown = new DropDownList { ID = "control" + field.ID };

			var lookupDB = new XmlDBBase(false, false)
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
			//if we have a render none attribute add an option into the first position that allows the user to not select anything
			if (field.Attributes.ContainsKey("rendernone"))
			{
				var none = new ListItem() { Text = "None", Value = "", Selected = true };
				ourDropDown.Items.Insert(0, none);
				ourDropDown.SelectedIndex = 0;
			}
			// if we aren't a new record see if we havea value that we need to select
			if (PKey > 0 && (ourProperty != null || field.Attributes.ContainsKey("keyvalue")))
			{
				string ourValue;
				if (field.Attributes.ContainsKey("keyvalue"))
				{
					ourValue = GetStringValue(field, ourPage);
				}
				else
				{
					ourValue = GetNumericValue(field, ourPage);
				}
				
				var iPosition = 0;
				foreach (ListItem item in ourDropDown.Items)
				{
					if (item.Value == ourValue)
					{
						item.Selected = true;
						ourDropDown.SelectedIndex = iPosition;
						break;
					}
					iPosition++;
				}
				HttpContext.Current.Trace.Write("Rendering Control Value: " + ourDropDown.SelectedValue);
			}
			
			

			return ourDropDown;
		}
	}
}
