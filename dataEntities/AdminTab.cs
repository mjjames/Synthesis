using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mjjames.AdminSystem.dataentities
{
	public struct AdminTab
	{
		public List<AdminField> Fields { set; get; }
		public string ID { set; get; }
		public string Label { set; get; }
	}
}
