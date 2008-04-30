
namespace mjjames
{
    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Text; //string builder


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
			HtmlGenericControl ul = new HtmlGenericControl("ul");
			ul.Attributes.Add("class", "adminToolbar");
			ul.Controls.Add(makeLinkItem("Home", "~/"));

			ul.Controls.Add(buildToolbar());
            Controls.Add(ul);  //add our ul and home link
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

        private HtmlControl makeLinkItem(string linkText, string linkURL)
        {
			HtmlGenericControl hc = new HtmlGenericControl("li");
			
            HyperLink hl = new HyperLink();
			hl.Text = linkText;
			hl.NavigateUrl = linkURL;

			hc.Controls.Add(hl);
            
            return hc;
        }
    }
}