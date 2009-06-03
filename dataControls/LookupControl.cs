using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using mjjames.AdminSystem.dataentities;
using mjjames.AdminSystem;
using System.Configuration;

namespace mjjames.AdminSystem.dataControls
{
	public class lookupControl
	{

		private int _iPKey;

		public int iPKey
		{
			get
			{
				return _iPKey;
			}
			set
			{
				_iPKey = value;
			}
		}

		public static object getDataValue(Control ourControl, Type ourType)
		{
			Control ourFileControl = ourControl.Parent.FindControl(ourControl.ID.Replace("control", "hidden"));
			HiddenField ourHiddenFile = (HiddenField)ourFileControl;
			if (ourHiddenFile.Value == String.Empty)//If a lookup value isnt provided then return null
			{
				return null;
			}
			return Convert.ChangeType(ourHiddenFile.Value, ourType);
		}

		public Control generateControl(AdminField field, object ourPage)
		{
			PropertyInfo ourProperty;

			ScriptManager ourSM = ScriptManager.GetCurrent((Page)HttpContext.Current.Handler);
			
			UpdatePanel lookupPanel = new UpdatePanel();
			lookupPanel.ChildrenAsTriggers = true;

			GridView pageListing = new GridView();
			pageListing.ID = "control" + field.ID;

			XmlDBBase lookupDB = new XmlDBBase();
			lookupDB.ConnectionString = ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString;
			lookupDB.TableName = field.Attributes["lookuptable"];

			SqlDataSource datasource = lookupDB.DataSource(true, true, false, false, false);

			pageListing.DataSource = datasource;
			pageListing.DataKeyNames = new[] { lookupDB.TablePrimaryKeyField };

			ourProperty = ourPage.GetType().GetProperty(field.ID, typeof(string));
			
			CommandField cfSelect = new CommandField();
			cfSelect.ShowSelectButton = true;
			cfSelect.SelectText = "Choose";

			pageListing.AutoGenerateColumns = false;
			pageListing.EnableViewState = false;
			pageListing.Columns.Add(cfSelect);
			pageListing.CssClass = "listingTable";
			pageListing.FooterStyle.CssClass = "pageListFooter";
			pageListing.RowStyle.CssClass = "pageListRow";
			pageListing.HeaderStyle.CssClass = "pageListHeader";
			pageListing.AlternatingRowStyle.CssClass = "pageListRowAlternate";
			pageListing.SelectedRowStyle.CssClass = "selectedRow";
			pageListing.SelectedIndexChanged += new EventHandler(pageListing_SelectedIndexChanged);

			if (lookupDB.TableDefaults != null)
			{

				foreach (AdminField listingField in lookupDB.TableDefaults.FindAll(t => t.Attributes.ContainsKey("list")))
				{
					BoundField dcf = new BoundField();
					string Type = String.Empty;
					bool bType = listingField.Attributes.TryGetValue("type", out Type);
					bool bRenderField = true;

					dcf.HeaderText = listingField.Label;
					dcf.SortExpression = listingField.ID;
					dcf.DataField = listingField.ID;

					if (bType)
					{
						// Specific Functionality Depends on Type
						switch (Type)
						{
							case "hidden":
								bRenderField = false;
								break;
							case "datetime":
								dcf.DataFormatString = "{0:dd/MM/yyyy}";
								break;
							default:
								break;
						}
						if (bRenderField)
						{
							pageListing.Columns.Add(dcf);
						}
					}
				}
			}

			HiddenField selectedItem = new HiddenField();
			selectedItem.ID = "hidden" + field.ID;
			ourProperty = ourPage.GetType().GetProperty(field.ID);

			///TODO Fix ourLabel.CssClass = "hidden";

			lookupPanel.ContentTemplateContainer.Controls.Add(datasource);
			lookupPanel.ContentTemplateContainer.Controls.Add(pageListing);
			lookupPanel.DataBind();
			lookupPanel.ContentTemplateContainer.Controls.Add(selectedItem);

			if (_iPKey > 0 && ourProperty != null)
			{
				string ourValue = (ourProperty.GetValue(ourPage, null) + "");
				selectedItem.Value = "" + ourValue;

				int iPosition = 0;
				foreach (DataKey key in pageListing.DataKeys)
				{
					if (key.Value.ToString() == ourValue)
					{
						pageListing.SelectedIndex = iPosition;
					}
					iPosition++;
				}

				HttpContext.Current.Trace.Warn("Rendering Control Value: " + selectedItem.Value);
			}

			ourSM.RegisterAsyncPostBackControl(pageListing);

			return lookupPanel;
		}


		void pageListing_SelectedIndexChanged(object sender, EventArgs e)
		{
			GridView ourRow = sender as GridView;
			HiddenField ourValue = ourRow.Parent.Controls.OfType<HiddenField>().FirstOrDefault();
			ourValue.Value = ourRow.SelectedValue.ToString();
		}
	
	}
}
