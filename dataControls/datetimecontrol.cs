using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Reflection;
using AjaxControlToolkit;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem.dataControls
{
	public class DatetimeControl : KeyValuePairControl , IDataControl
	{
		//public int PKey { get; set; }

		public static object GetDataValue(Control ourControl, Type ourType)
		{
			TextBox ourDateTime = (TextBox)ourControl;
			
			// if our datatype is a string just return the text, otherwise parse to date
			if (ourType.Name.Equals("String"))
			{
				return ourDateTime.Text;
			}
			
			DateTime? dtValue = null;
			if(!String.IsNullOrEmpty(ourDateTime.Text)){
				
				dtValue = DateTime.Parse(ourDateTime.Text, new CultureInfo("en-GB", false));
			}

			return dtValue ;
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			WebControl container = new WebControl(HtmlTextWriterTag.Div);
			container.CssClass = "field";

			TextBox ourDateText = new TextBox {ID = "control" + field.ID};

			PropertyInfo ourProperty = ourPage.GetType().GetProperty(field.ID);
			DateTime ourValue = new DateTime();

			if(PKey > 0 && field.Attributes.ContainsKey("keyvalue")){
				ourDateText.Text = base.GetStringValue(field, ourPage);
			}
			else{

				if (PKey > 0 && ourProperty != null && (ourProperty.GetValue(ourPage, null) != null))
				{
					ourValue = DateTime.Parse(ourProperty.GetValue(ourPage, null).ToString());
					ourDateText.Text = String.Format("{0:dd/MM/yyyy}", ourValue);
					HttpContext.Current.Trace.Write("Rendering Control Value: " + ourDateText.Text);
				}
			}

			if (field.Attributes.Keys.Contains("defaultvalue"))
			{
				if (!DateTime.TryParse(field.Attributes["defaultvalue"], out ourValue)) //try to parse default value, else revert to today
				{
					ourValue = DateTime.Today;
				}
				ourDateText.Text = String.Format("{0:dd/MM/yyyy}", ourValue);
				
			}

			CalendarExtender datetimeCalendar = new CalendarExtender
			                                    	{
			                                    		Animated = true,
			                                    		ID = "calendar" + field.ID,
			                                    		PopupPosition = CalendarPosition.TopRight,
			                                    		TargetControlID = "control" + field.ID,
			                                    		Format = "dd/MM/yyyy"
			                                    	};

			container.Controls.Add(ourDateText);
			container.Controls.Add(datetimeCalendar);

			return container;
		}
	}
}
