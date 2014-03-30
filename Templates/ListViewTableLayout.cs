using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mjjames.AdminSystem.Templates
{
    public class ListViewTableLayout : ITemplate
    {
        private IEnumerable<string> _columnNames;
        public ListViewTableLayout(IEnumerable<string> columnNames)
        {
            _columnNames = columnNames;
        }

        public void InstantiateIn(Control container)
        {
            var table = new WebControl(HtmlTextWriterTag.Table){
                CssClass = "listingTable table table-bordered table-hover table-striped",
                ID = "tableLayout"
            };

            GenerateHeader(table);
            GenerateBody(table);

            container.Controls.Add(table);
        }

        private static void GenerateBody(WebControl table)
        {
            var body = new WebControl(HtmlTextWriterTag.Tbody);
            var placeHolder = new WebControl(HtmlTextWriterTag.Tr)
            {
                ID = "itemPlaceholder"
            };
            body.Controls.Add(placeHolder);
            table.Controls.Add(body);
        }

        private void GenerateHeader(WebControl table)
        {
            var tableHead = new WebControl(HtmlTextWriterTag.Thead);
            var headRow = new WebControl(HtmlTextWriterTag.Tr);
            foreach (var column in _columnNames)
            {
                headRow.Controls.Add(new WebControl(HtmlTextWriterTag.Th)
                {
                    Controls =
                    {
                        new LiteralControl(column)
                    }
                });
            }
            tableHead.Controls.Add(headRow);
            table.Controls.Add(tableHead);
        }
    }
}