using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.ControlLibrary;

public partial class newsletters_subscriptionmanagement : System.Web.UI.Page
{
	private NewsletterFunctions nf = new NewsletterFunctions();
	protected void Page_Load(object sender, EventArgs e)
	{

	}

	protected void resendConfirmation(object sender, EventArgs e)
	{
		ListViewDataItem lv = ((LinkButton)sender).Parent as ListViewDataItem;

		Label name = lv.FindControl("nameLabel") as Label;
		Label email = lv.FindControl("emailLabel") as Label;

		labelStatus.Text = "Sorry An Error Has Occurred";

		if (name != null && email != null)
		{
			if (!String.IsNullOrEmpty(name.Text) && !String.IsNullOrEmpty(email.Text))
			{
				if (nf.SendSignUpConfirmation(name.Text, email.Text))
				{
					labelStatus.Text = "Confrmation Sent";
				}
			}
		}
	}

	protected void errorStatus(object sender, EventArgs e)
	{
		labelStatus.Text = "Sorry An Error Has Occurred, Please Try Again";
	}

	protected void editStatus(object sender, EventArgs e)
	{
		labelStatus.Text = "Editing";
	}

	protected void insertedStatus(object sender, EventArgs e)
	{
		labelStatus.Text = "Reciprient Inserted";
	}

	protected void updatedStatus(object sender, EventArgs e)
	{
		labelStatus.Text = "Reciprient Updated";
	}

	protected void removedStatus(object sender, EventArgs e)
	{
		labelStatus.Text = "Reciprient Removed";
	}

	protected void resetStatus(object sender, EventArgs e)
	{
		labelStatus.Text = "Listing Mode";
	}
}
