using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mjjames.AdminSystem.dataentities
{

	public class AdminTable
	{
		private bool _bEmailButton = false;
		/// <summary>
		/// Indicates whether to render the "email the content" button
		/// </summary>
		public bool bEmailButton
		{
			get
			{
				return _bEmailButton;
			}
			set
			{
				_bEmailButton = value;
			}
		}
		public string ID { get; set; }
		public string Label { get; set; }
		public bool bQuickEdit { get; set; }
		public List<AdminField> Defaults { set; get; }
		public List<AdminTab> Tabs { set; get; }
	}
}
