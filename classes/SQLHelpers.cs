using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mjjames.AdminSystem.classes
{
	public static class SQLHelpers
	{
		public static string SQLSafe(string value){
			return value.Replace("'", "''");
		}
	}
}
