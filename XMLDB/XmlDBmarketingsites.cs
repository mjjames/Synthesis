using System;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.DataContexts;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem.DataEntities;
using System.Web;

namespace mjjames.AdminSystem
{
	public class XmlDBmarketingsites : XmlDBBase
	{
		#region datasources

		/// <summary>
		/// Gets the data for this editor
		/// </summary>
		/// <returns>a general object that needs casting to the correct type on use</returns>
		protected override object GetData()
		{
			
			var ourSite = new marketingsite();
			if (PKey > 0)
			{
				ourSite = (from p in AdminDC.marketingsites
							  where p.marketingsite_key == PKey
							  select p).SingleOrDefault();
			}

			return ourSite;
		}

		
		#endregion

		#region button events


		/// <summary>
		/// save away our data / insert
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void SaveEdit(object sender, EventArgs e)
		{
			var ourSender = (Button)sender;
			var ourPageDataContext =new AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
			var ourData = new marketingsite();
			if (PKey > 0)
			{
				ourData = ourPageDataContext.marketingsites.Single(p => p.marketingsite_key== PKey);
			}

			foreach (var tab in Table.Tabs)
			{
				var ourTab = (TabPanel)FindControlRecursive(ourSender.Page, tab.ID);
				if (ourTab == null) continue;
				foreach (var field in tab.Fields)
				{
					var ourControl = ourTab.FindControl("control" + field.ID);

					if (ourControl == null) continue;
					var ourProperty = ourData.GetType().GetProperty(field.ID);
					if (ourProperty != null)
					{
						Logger.LogInformation("Saving Content In: " + ourControl.ID);
						ourProperty.SetValue(ourData, GetDataValue(ourControl, field.Type, ourProperty.PropertyType), null);
					}
					else
					{
						Logger.LogError("Error Saving Content: " + ourControl.ID, new Exception("Error Saving Content: " + ourControl.ID));
					}
				}
			}

			if (PKey == 0)
			{
				if (MultiTenancyEnabled)
				{
					ourData.site_fkey = SiteFKey;
				}
				ourPageDataContext.marketingsites.InsertOnSubmit(ourData);
			}

			var labelStatus = (Label)FindControlRecursive(ourSender.Page, ("labelStatus"));
			try
			{
				var ourChanges = ourPageDataContext.GetChangeSet();

				labelStatus.Text = "Nothing to Save";
				ourPageDataContext.SubmitChanges();
				
				//TODO: after we have saved our changes we need to insert / update any keyvaluepair data we may have
				//		to do this we need to look at stashing this data and passing it to a base method. 
				//		we can only do it after the insert / update as we need the primary key

				if (ourChanges.Inserts.Count > 0)
				{
					labelStatus.Text = String.Format("{0} Inserted", Table.ID);


					PKey = ourData.marketingsite_key;

					var strPKeyField = TablePrimaryKeyField;

					var ourPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "pkey");
					var ourControlPKey = (HiddenField)FindControlRecursive(labelStatus.Parent, "control" + strPKeyField);

					try
					{
						ourControlPKey.Value = PKey.ToString();
						ourPKey.Value = PKey.ToString();
					}
					catch
					{
						var ex =
							new Exception(String.Format("{0} doesn't contain a hidden control called {1}", Table.ID, TablePrimaryKeyField));
						Logger.LogError("Unknown Field",ex);
						throw ex;
					}

				}
				if (ourChanges.Updates.Count > 0)
				{
					labelStatus.Text = String.Format("{0} Updated", Table.ID);
				}



			}
			catch (Exception ex)
			{
				labelStatus.Text = String.Format("{0} Update Failed", Table.ID);
				Logger.LogError("Failed Update", ex);
			}
		}

		#endregion


	}

}