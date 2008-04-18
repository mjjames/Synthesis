/*
*Image Uploader with MJJames WaterMarking
@author mjjames mike@mjjames.co.uk
*/


// Register the related commands.
FCKCommands.RegisterCommand( 'WatermarkImage', new FCKDialogCommand( FCKLang['DlgImgTitle']	, FCKLang['DlgImgTitle'], FCKConfig.PluginsPath + 'watermarkImage/watermarkImage.html'	, 340, 400 ) ) ;

// Create the "Watermark" toolbar button.
var oWatermarkItem		= new FCKToolbarButton( 'WatermarkImage', FCKLang['DlgImgTitle'] ) ;
oWatermarkItem.IconPath	= FCKConfig.PluginsPath + 'watermarkimage/watermarkimage.gif' ;

FCKToolbarItems.RegisterItem( 'WatermarkImage', oWatermarkItem ) ;	

FCK.ContextMenu.RegisterListener( {
        AddItems : function( menu, tag, tagName )
        {
                // under what circumstances do we display this option
                if ( tagName == 'IMG')
                {
                        // when the option is displayed, show a separator  the command
                        menu.AddSeparator() ;
                        // the command needs the registered command name, the title for the context menu, and the icon path
                        menu.AddItem( 'WatermarkImage', FCKLang.DlgWatermarkTitle, oWatermarkItem.IconPath ) ;
                }
        }}
);