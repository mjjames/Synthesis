/*
*Image Uploader with MJJames Simple Interface
@author mjjames mike@mjjames.co.uk
*/


// Register the related commands.
FCKCommands.RegisterCommand('SimpleImage', new FCKDialogCommand(FCKLang['DlgImgTitle'], FCKLang['DlgImgTitle'], FCKConfig.PluginsPath + 'SimpleImage/SimpleImage.html', 340, 400));

// Create the "Watermark" toolbar button.
var oSimpleImageItem = new FCKToolbarButton('SimpleImage', FCKLang['DlgImgTitle']);
oSimpleImageItem.IconPath = FCKConfig.PluginsPath + 'SimpleImage/SimpleImage.gif';

FCKToolbarItems.RegisterItem('SimpleImage', oSimpleImageItem);	

FCK.ContextMenu.RegisterListener( {
        AddItems : function( menu, tag, tagName )
        {
            // under what circumstances do we display this option
            var oFakeImage = FCK.Selection.GetSelectedElement();
            if (tagName == 'IMG' && !oFakeImage.getAttribute('_fckflash')) 
            {
                // when the option is displayed, show a separator  the command
                menu.AddSeparator() ;
                // the command needs the registered command name, the title for the context menu, and the icon path
                menu.AddItem('SimpleImage', "Image Properties", oSimpleImageItem.IconPath);
            }
        }}
);