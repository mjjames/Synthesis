using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Reflection;

public partial class AdminSystem : System.Web.UI.MasterPage
{
    protected override void OnInit(EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            signout.Visible = true;
            adminToolbar.Visible = true;
            siteSelector.Visible = true;
            fileStorage.Visible = true;
        }
        base.OnInit(e);

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        double maxStorage = 0;
        if (!double.TryParse(ConfigurationManager.AppSettings["StorageLimitInGigabytes"], out maxStorage))
        {
            maxStorage = 2;
        }
        totalSpace.Text = (maxStorage * 1024).ToString();
        const int gigaByte = 1073741824;
        maxStorage = maxStorage * gigaByte; //1 gb in bytes
        var usedDiskSpace = GetUsedDiskSpace();
        usedSpace.Text = Math.Round(usedDiskSpace/(gigaByte/1024), 2).ToString();
        OutOfDiskSpace.Visible = usedDiskSpace >= maxStorage;

        Page.Title += String.Format(" | {0} | Synthesis CMS - {1}", ConfigurationManager.AppSettings["SiteName"], GetVersionNumber());

    }

    private double GetUsedDiskSpace()
    {
        if(Cache["UsedDiskSpace"] != null)
        {
            return (double) Cache["UsedDiskSpace"];
        }
        
        var usedDiskSpace = ConfigurationManager.AppSettings["uploaddir"].StartsWith("http") ? 0 : System.IO.Directory.GetFiles(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["uploaddir"]), "*", System.IO.SearchOption.AllDirectories).Sum(x => (double)(new System.IO.FileInfo(x).Length));
        Cache["UsedDiskSpace"] = usedDiskSpace;
        return usedDiskSpace;
    }

    protected string GetVersionNumber()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    protected void btnSignOut_ServerClick(object sender, System.EventArgs e)
    {
        FormsAuthentication.SignOut();
        Response.Redirect("~/", true);
    }

    protected void SetAccessLevel(object sender, EventArgs e)
    {
        //Nasty stuff this, I can only check if users are in roles so dodgy ifs to set the access level
        int accessLevel = 0; //by default block all access

        if (Page.User.IsInRole("Article Editor")) accessLevel = 2;
        if (Page.User.IsInRole("Content Editor")) accessLevel = 3;
        if (Page.User.IsInRole("Editor")) accessLevel = 4;
        if (Page.User.IsInRole("Site Admin")) accessLevel = 5;
        if (Page.User.IsInRole("System Admin")) accessLevel = 6;
        adminToolbar.AccessLevel = accessLevel;
    }
}
