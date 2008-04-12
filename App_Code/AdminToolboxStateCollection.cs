using System.Configuration;

/// <summary>
/// Summary description for AdminToolboxStateCollection
/// </summary>
public class AdminToolboxStateCollection : ConfigurationElementCollection
{
    public AdminToolboxState this[int index]
    {
        get
        {
            return base.BaseGet(index) as AdminToolboxState;
        }
        set
        {
            if (base.BaseGet(index) != null)
            {
                base.BaseRemoveAt(index);
            }
            this.BaseAdd(index, value);
        }
    }


    protected override ConfigurationElement CreateNewElement()
    {
        return new AdminToolboxState();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
        return ((AdminToolboxState)element).name;
    } 
}
