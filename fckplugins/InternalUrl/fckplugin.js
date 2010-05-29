/*
*Internal Page URL
@author mjjames mike@mjjames.co.uk
*/


// Register the related commands.
FCKCommands.RegisterCommand('InternalUrl', new FCKDialogCommand('Internal Page URL', 'Internal Page URL', FCKConfig.PluginsPath + 'InternalUrl/InternalUrl.aspx', 340, 400));

// Create the "Watermark" toolbar button.
var oInternalURLItem = new FCKToolbarButton('InternalUrl', 'Internal Page URL');
oInternalURLItem.IconPath = FCKConfig.PluginsPath + 'internalurl/internalurl.gif';

FCKToolbarItems.RegisterItem('InternalUrl', oInternalURLItem);	

FCK.ContextMenu.RegisterListener( {
        AddItems : function( menu, tag, tagName )
        {
                // under what circumstances do we display this option
                if ( tagName == 'A')
                {
                        // when the option is displayed, show a separator  the command
                        menu.AddSeparator() ;
                        // the command needs the registered command name, the title for the context menu, and the icon path
                        menu.AddItem('InternalUrl', FCKLang.DlgInternalURLTitle, oInternalURLItem.IconPath);
                }
        }}
);