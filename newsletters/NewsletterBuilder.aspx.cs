using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class newsletters_NewsletterBuilder : System.Web.UI.Page
{
	private mjjames.admin.xmlDB page = new mjjames.admin.xmlDB();
	protected override void OnInit(EventArgs e)
	{
		page.TableName = "newsletters";
		placeholderTabs.Controls.Add(page.GeneratePage());
		
		
		base.OnInit(e);
	}
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Page.IsPostBack && !String.IsNullOrEmpty(newsletter_key.Value))
		{
			page.PrimaryKey = int.Parse(newsletter_key.Value);
		}
	}
}
