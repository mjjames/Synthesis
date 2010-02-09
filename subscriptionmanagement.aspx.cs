using System;
using System.Web.UI.WebControls;
using mjjames.ControlLibrary;

namespace mjjames.AdminSystem
{
	public partial class NewslettersSubscriptionmanagement : System.Web.UI.Page
	{
		private readonly NewsletterFunctions _nf = new NewsletterFunctions();
		protected void Page_Load(object sender, EventArgs e)
		{
			
		}


		protected void ResendConfirmation(object sender, EventArgs e)
		{
			ListViewDataItem lv = ((LinkButton)sender).Parent as ListViewDataItem;

			if (lv == null) return;
			Label name = lv.FindControl("nameLabel") as Label;
			Label email = lv.FindControl("emailLabel") as Label;

			labelStatus.Text = "Sorry An Error Has Occurred";

			if (name == null || email == null) return;
			if (String.IsNullOrEmpty(name.Text) || String.IsNullOrEmpty(email.Text)) return;
			if (_nf.SendSignUpConfirmation(name.Text, email.Text))
			{
				labelStatus.Text = "Confirmation Sent";
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
}