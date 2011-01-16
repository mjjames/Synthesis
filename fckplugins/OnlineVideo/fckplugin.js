/*
* FCKEditor OnlineVideo Plugin - Plugin
* Publisher Host (c) Creative Commons 2008
* http://creativecommons.org/licenses/by-sa/3.0/
* Author: Michael James | http://www.mjjames.co.uk
*/

// Register the related commands.
FCKCommands.RegisterCommand('OnlineVideo', new FCKDialogCommand("Online Video", "Online Video", FCKConfig.PluginsPath + 'OnlineVideo/OnlineVideo.html', 450, 350));

// Create the "OnlineVideo" toolbar button.
var oFindItem		= new FCKToolbarButton( 'OnlineVideo', "Online Video" ) ;
oFindItem.IconPath	= FCKConfig.PluginsPath + 'OnlineVideo/OnlineVideo.gif' ;

FCKToolbarItems.RegisterItem( 'OnlineVideo', oFindItem ) ;			// 'OnlineVideo' is the name used in the Toolbar config.
FCK.ContextMenu.RegisterListener({
    AddItems: function (menu, tag, tagName) {
        var oFakeImage = FCK.Selection.GetSelectedElement();
        // under what circumstances do we display this option
        if (tagName == 'IMG' && oFakeImage.getAttribute('_fckflash')) {
            // when the option is displayed, show a separator  the command
            menu.AddSeparator();
            // the command needs the registered command name, the title for the context menu, and the icon path
            menu.AddItem('OnlineVideo', "Online Video Properties", oFindItem.IconPath);
        }
    } 
}
);