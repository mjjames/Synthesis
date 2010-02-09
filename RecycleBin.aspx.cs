using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mjjames.AdminSystem.dataEntities;
using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.Repositories;

namespace mjjames.AdminSystem
{
	public partial class RecycleBin : System.Web.UI.Page
	{
		protected List<RecyledItem> items = new List<RecyledItem>();
		private RecycleRepository _repos = new RecycleRepository();
		protected void Page_Load(object sender, EventArgs e)
		{
			loadItems();
		}
		
		private void loadItems(){
			items = _repos.GetRecycledItems();
			recycledItems.DataSource=items;
			recycledItems.DataKeyNames = new string[] {"ItemKey", "ItemType"};
			recycledItems.DataBind();
		}
			
		/// <summary>
		/// Handles removing Recycled Items Permently	
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e){
			_repos.PermenantDelete((int)recycledItems.DataKeys[e.RowIndex].Values["ItemKey"], (string)recycledItems.DataKeys[e.RowIndex].Values["ItemType"]);
			loadItems(); //update listing view
		}
		
		protected void RestoreItem(object sender, EventArgs e){
			_repos.RestoreItem((int)recycledItems.SelectedDataKey.Values["ItemKey"], (string)recycledItems.SelectedDataKey.Values["ItemType"]);
			loadItems();
		}
	}
}
