using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

/// <summary>
/// ASPNET2 Custom Config Sections -- Adapted from 4GuysFromRolla
/// </summary>
public class ASPNET2Configuration : ConfigurationSection
{
    /// <summary>
    /// Returns an ASPNET2Configuration instance
    /// </summary>
    public static ASPNET2Configuration GetConfig()
    {
        return ConfigurationManager.GetSection("mjjames/adminToolbox") as ASPNET2Configuration;
    }

    [ConfigurationProperty("adminControls")]
    public ASPNET2ConfigurationStateCollection adminControls
    {
        get
        {
            return this["adminControls"] as ASPNET2ConfigurationStateCollection;
        }
    } 
}
