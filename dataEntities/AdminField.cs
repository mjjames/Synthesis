using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace mjjames.AdminSystem.dataentities
{
	public class AdminField
	{

		public string ID { set; get; }
		public string Type { set; get; }
		public string Label { set; get; }

		private Dictionary<string, string> _Attributes;

		public Dictionary<string, string> Attributes
		{
			set { _Attributes = value; }
			get { return _Attributes; }
		}

		public void SetAttribute(string name, string value)
		{
			_Attributes.Add(name, value);
		}

		public void SetAttributes(IEnumerable<XAttribute> Attributes)
		{

		}



	}

}
