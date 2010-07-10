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
	public class DatetimeControl : KeyValuePairControl, IDataControl
	{
		//public int PKey { get; set; }

		public static object GetDataValue(Control ourControl, Type ourType)
		{
			TextBox ourDateTime = (TextBox)ourControl;

			DateTime? dtValue = null;
			if (!String.IsNullOrEmpty(ourDateTime.Text))
			{

				dtValue = DateTime.Parse(ourDateTime.Text, new CultureInfo("en-GB", false));
				// if our datatype is a string return the date as a string in the ISO 8601 format (YYYY-MM-DD), otherwise return the date
				if (ourType.Name.Equals("String"))
				{
					return dtValue.Value.ToString("yyyy-MM-dd");
				}
				else
				{
					return dtValue;
				}
			}
			else
			{
				return null;
			}

		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			WebControl container = new WebControl(HtmlTextWriterTag.Div);
			container.CssClass = "field";

			TextBox ourDateText = new TextBox { ID = "control" + field.ID };

			PropertyInfo ourProperty = ourPage.GetType().GetProperty(field.ID);
			DateTime ourValue = new DateTime();

			if (PKey > 0 && field.Attributes.ContainsKey("keyvalue"))
			{
				var dateTime = base.GetStringValue(field, ourPage);
				if(!String.IsNullOrEmpty(dateTime)){
					//as we store the date in ISO 8601 we can parse the date with an invarient culture
					ourValue = DateTime.Parse(dateTime, null, DateTimeStyles.RoundtripKind);
				}
			}
			else
			{

				if (PKey > 0 && ourProperty != null && (ourProperty.GetValue(ourPage, null) != null))
				{
					ourValue = DateTime.Parse(ourProperty.GetValue(ourPage, null).ToString());
					
				}
			}
			if(ourValue != null){
				ourDateText.Text = String.Format("{0:dd/MM/yyyy}", ourValue);
				HttpContext.Current.Trace.Write("Rendering Control Value: " + ourDateText.Text);
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
