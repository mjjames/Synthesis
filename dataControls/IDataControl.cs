using System;
using System.Web.UI;
using mjjames.AdminSystem.dataentities;

namespace mjjames.AdminSystem.DataControls
{
	public interface IDataControl
	{
		int PKey { get; set;}
		//object GetDataValue(Control ourControl, Type ourType);
		Control GenerateControl(AdminField field, object ourPage);
	}
}
