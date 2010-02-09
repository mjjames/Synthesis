using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using mjjames.AdminSystem.DataControls;
using mjjames.AdminSystem.dataentities;
using System.Configuration;

namespace mjjames.AdminSystem.dataControls
{
	public class LookupControl : IDataControl
	{
		public int PKey { get; set; }

		public static object GetDataValue(Control ourControl, Type ourType)
		{
			Control ourFileControl = ourControl.Parent.FindControl(ourControl.ID.Replace("control", "hidden"));
			HiddenField ourHiddenFile = (HiddenField)ourFileControl;
			return ourHiddenFile.Value == String.Empty ? null : Convert.ChangeType(ourHiddenFile.Value, ourType);
		}

		public Control GenerateControl(AdminField field, object ourPage)
		{
			ScriptManager ourSM = ScriptManager.GetCurrent((Page)HttpContext.Current.Handler);
			
			UpdatePanel lookupPanel = new UpdatePanel {ChildrenAsTriggers = true};

			GridView pageListing = new GridView {ID = "control" + field.ID};

			XmlDBBase lookupDB = new XmlDBBase
			                     	{
			                     		ConnectionString = ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString,
			                     		TableName = field.Attributes["lookuptable"]
			                     	};

			SqlDataSource datasource = lookupDB.DataSource(true, true, false, false, false);

			pageListing.DataSource = datasource;
			pageListing.DataKeyNames = new[] { lookupDB.TablePrimaryKeyField };

			ourPage.GetType().GetProperty(field.ID, typeof(string));

			CommandField cfSelect = new CommandField {ShowSelectButton = true, SelectText = "Choose"};

			pageListing.AutoGenerateColumns = false;
			pageListing.EnableViewState = false;
			pageListing.Columns.Add(cfSelect);
			pageListing.CssClass = "listingTable";
			pageListing.FooterStyle.CssClass = "pageListFooter";
			pageListing.RowStyle.CssClass = "pageListRow";
			pageListing.HeaderStyle.CssClass = "pageListHeader";
			pageListing.AlternatingRowStyle.CssClass = "pageListRowAlternate";
			pageListing.SelectedRowStyle.CssClass = "selectedRow";
			pageListing.SelectedIndexChanged += PageListingSelectedIndexChanged;

			if (lookupDB.TableDefaults != null)
			{

				foreach (AdminField listingField in lookupDB.TableDefaults.FindAll(t => t.Attributes.ContainsKey("list")))
				{
					BoundField dcf = new BoundField();
					string type;
					bool bType = listingField.Attributes.TryGetValue("type", out type);
					bool bRenderField = true;

					dcf.HeaderText = listingField.Label;
					dcf.SortExpression = listingField.ID;
					dcf.DataField = listingField.ID;

					if (!bType) continue;
					// Specific Functionality Depends on Type
					switch (type)
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

			HiddenField selectedItem = new HiddenField {ID = "hidden" + field.ID};
			PropertyInfo ourProperty = ourPage.GetType().GetProperty(field.ID);

			///TODO Fix ourLabel.CssClass = "hidden";

			lookupPanel.ContentTemplateContainer.Controls.Add(datasource);
			lookupPanel.ContentTemplateContainer.Controls.Add(pageListing);
			lookupPanel.DataBind();
			lookupPanel.ContentTemplateContainer.Controls.Add(selectedItem);

			if (PKey > 0 && ourProperty != null)
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

			if (ourSM != null) ourSM.RegisterAsyncPostBackControl(pageListing);

			return lookupPanel;
		}


		static void PageListingSelectedIndexChanged(object sender, EventArgs e)
		{
			GridView ourRow = sender as GridView;
			if (ourRow == null) return;
			HiddenField ourValue = ourRow.Parent.Controls.OfType<HiddenField>().FirstOrDefault();
			ourValue.Value = ourRow.SelectedValue.ToString();
		}
	
	}
}
