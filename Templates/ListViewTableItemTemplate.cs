using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mjjames.AdminSystem.Templates
{
    public class ListViewTableItemTemplate : ITemplate
    {
        private IEnumerable<string> _dataFieldIds;
        private string _dataKeyId;

        public ListViewTableItemTemplate(IEnumerable<string> dataFieldIds, string dataKeyId)
        {
            _dataFieldIds = dataFieldIds;
            _dataKeyId = dataKeyId;
        }

        public void InstantiateIn(Control container)
        {
            var row = new WebControl(HtmlTextWriterTag.Tr);
            foreach (var field in _dataFieldIds)
            {
                row.Controls.Add(new WebControl(HtmlTextWriterTag.Td)
                {
                    Controls = {
                        new LiteralControl
                        {
                            ID= "literal-" + field
                        }
                    }
                });
            }
            row.Controls.Add(new WebControl(HtmlTextWriterTag.Td)
            {
                Controls = {
                    new LinkButton{
                        CssClass = "btn btn-danger",
                        Controls = {
                                            new WebControl(HtmlTextWriterTag.I){
                                                CssClass = "icon-white icon-remove"
                                            }
                                        },
                        CommandName="Delete",
                        ToolTip = "Remove User Role"
                    }
                }
            });
            row.DataBinding += BindRow;
            container.Controls.Add(row);
        }

        private void BindRow(object sender, EventArgs e)
        {
            var control = (WebControl)sender;
            var row = ((DataRowView)((ListViewDataItem)control.NamingContainer).DataItem).Row;
            foreach (var dataField in _dataFieldIds)
            {
                var literalControl = control.FindControl("literal-" + dataField) as LiteralControl;
                literalControl.Text = row.Field<string>(dataField);
            }
        }
    }
}