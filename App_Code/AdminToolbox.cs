using System.Configuration;
using System.Web;

/// <summary>
/// AdminToolbox Custom Config Sections 
/// </summary>
public class AdminToolbox : ConfigurationSection
{
    /// <summary>
    /// Returns an AdminToolbox instance
    /// </summary>
    public static AdminToolbox GetConfig()
    {
		return ConfigurationManager.GetSection("mjjames/adminToolbox") as AdminToolbox;
    }

    [ConfigurationProperty("adminControls")]
    public AdminToolboxStateCollection adminControls
    {
        get
        {
            return this["adminControls"] as AdminToolboxStateCollection;
        }
    } 
}
