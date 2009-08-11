using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Reflection;
using AjaxControlToolkit;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem.dataControls
{
	public class DatetimeControl
	{
		public int iPKey { get; set; }

		public static object getDataValue(Control ourControl, Type ourType)
		{
			TextBox ourDateTime = (TextBox)ourControl;
			DateTime? dtValue = null;
			if(!String.IsNullOrEmpty(ourDateTime.Text)){
				dtValue = DateTime.Parse(ourDateTime.Text, new CultureInfo("en-GB", false));
			}

			return dtValue ;
		}

		public Control generateControl(AdminField field, object ourPage)
		{
			WebControl container = new WebControl(HtmlTextWriterTag.Div);

			TextBox ourDateText = new TextBox {ID = "control" + field.ID};

			PropertyInfo ourProperty = ourPage.GetType().GetProperty(field.ID);
			DateTime ourValue = new DateTime();
			if (iPKey > 0 && ourProperty != null && (ourProperty.GetValue(ourPage, null) != null))
			{
				ourValue = DateTime.Parse(ourProperty.GetValue(ourPage, null).ToString());
				ourDateText.Text = String.Format("{0:dd/MM/yyyy}", ourValue);
				HttpContext.Current.Trace.Warn("Rendering Control Value: " + ourDateText.Text);
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
