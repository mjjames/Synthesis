using System.Configuration;

/// <summary>
/// Summary description for AdminToolboxState
/// </summary>
public class AdminToolboxState : ConfigurationElement
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
