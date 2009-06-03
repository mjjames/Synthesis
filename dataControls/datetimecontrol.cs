using System;
using System.Collections.Generic;
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
	public class datetimeControl
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

			PropertyInfo ourProperty;
			TextBox ourDateText = new TextBox();
			ourDateText.ID = "control" + field.ID;

			ourProperty = ourPage.GetType().GetProperty(field.ID);
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

			CalendarExtender datetimeCalendar = new CalendarExtender();
			datetimeCalendar.Animated = true;
			datetimeCalendar.ID = "calendar" + field.ID;
			datetimeCalendar.PopupPosition = CalendarPosition.TopRight;
			datetimeCalendar.TargetControlID = "control" + field.ID;
			datetimeCalendar.Format = "dd/MM/yyyy";

			container.Controls.Add(ourDateText);
			container.Controls.Add(datetimeCalendar);

			return container;
		}
	}
}
