
namespace mjjames
{
    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Text; //string builder
    using System.Collections;
    using System.Drawing;

    using System.Collections.Specialized;
    using System.Configuration;
    using System.Web.Configuration;
    using System.Collections.Generic;
    using System.Configuration.Provider;

    public class adminToolbar : WebControl
    {
        private int intAccessLevel = 0; //lets default to show nothing

        public int accessLevel
        {
            get { return intAccessLevel; }
            set { intAccessLevel = value; }
        }

        protected override void CreateChildControls()
        {
            StringBuilder strToolbox;
            strToolbox = new StringBuilder();
            strToolbox.AppendLine("<ul class=\"adminToolbar\">");
            strToolbox.AppendLine("<li class=\"home\">");
            strToolbox.AppendLine("<a href=\"./\" accesskey=\"1\" title=\"Home\"> Home </a>");
            strToolbox.AppendLine("</li>");

            LiteralControl lcToolbox = new LiteralControl();

            lcToolbox.Text = strToolbox.ToString();

            Controls.Add(lcToolbox);  //add our ul and home link
            Controls.Add(buildToolbar());
            strToolbox = new StringBuilder();
            strToolbox.AppendLine("</ul>");
            LiteralControl lcToolbox2 = new LiteralControl();
            lcToolbox2.Text = strToolbox.ToString();
            Controls.Add(lcToolbox2); //close it all up
        }


        private Control buildToolbar()
        {
            Control ccConfigControls = new Control();
            String strLink = "";
            String strName = "";
            String strAccessLevel = "";
			AdminToolbox ourToolBox = AdminToolbox.GetConfig();
            if (ourToolBox == null)
            {
                // Web Config is broken need to have some handler code here
				throw new Exception("Web.Config is missing MJJames ToolBars");
            }
            else
            {
                foreach (AdminToolboxState control in ourToolBox.adminControls)
                {
                    strName = control.name.ToString();
                    strLink = control.url.ToString();
                    strAccessLevel = control.accesslevel.ToString();
                    if ((strAccessLevel == "" + intAccessLevel) || (strAccessLevel == "1"))
                    {
                        ccConfigControls.Controls.Add(makeLinkItem(strName, strLink));
                    }
                }
            }
            return ccConfigControls;
        }

        private LiteralControl makeLinkItem(string linkText, string linkURL)
        {
            StringBuilder strToolboxLink;
            strToolboxLink = new StringBuilder();
            strToolboxLink.AppendLine("<li>");
            strToolboxLink.AppendLine("<a href=\"" + linkURL + "\" title=\"" + linkText + "\">" + linkText + "</a>");
            strToolboxLink.AppendLine("</li>");
            LiteralControl lcLink = new LiteralControl();
            lcLink.Text = strToolboxLink.ToString();
            return lcLink;
        }
    }
}