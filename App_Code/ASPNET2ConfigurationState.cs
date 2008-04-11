using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for ASPNET2ConfigurationState
/// </summary>
public class ASPNET2ConfigurationState : ConfigurationElement
{
    [ConfigurationProperty("name", IsRequired = true)]
    public string name
    {
        get
        {
            return this["name"] as string;
        }
    }

    [ConfigurationProperty("accesslevel", DefaultValue = "1", IsRequired = true)]
    public int accesslevel
    {
        get
        {
            return (int)this["accesslevel"];
        }
    }

    [ConfigurationProperty("url", DefaultValue = "/", IsRequired = true)]
    public string url
    {
        get
        {
            return this["url"] as string;
        }
    }
}
