using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.dataControls;
using mjjames.AdminSystem.DataEntities;
using com.flajaxian;
using System.IO;

namespace mjjames.AdminSystem.tests
{
	public partial class MediaGallery : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ph.Controls.Add(new MediaControl
			{
				PKey = 1,
				SiteKey = 1
			}.GenerateControl(new dataentities.AdminField
			{
				ID = "mediaTest",
				Label = "mediaTest",
				Type = "media",
				Attributes = new Dictionary<string, string>()
			}, new page
			{
				
			}));
        }
	}
}