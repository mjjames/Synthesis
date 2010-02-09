//FCKEditor Settings

FCKConfig.ToolbarSets["mjjames"] = [
    ['Cut','Copy','PasteText','-','SpellCheck',
    '-','Image','OnlineVideo','Table','Rule','Smiley','SpecialChar','-',
	'Undo','Redo','-','Find','Replace','-','SelectAll','RemoveFormat','-','Link','Unlink','Anchor','-','Source'],
	['Style','FontFormat','-','JustifyLeft','JustifyCenter','JustifyRight','JustifyFull',
	'-','Bold','Italic','Superscript','OrderedList','UnorderedList','-','Outdent','Indent']	
];

FCKConfig.EditorAreaCSS = FCKConfig.BasePath + 'css/fck_editorarea.css' ;

FCKConfig.SkinPath = FCKConfig.BasePath + 'skins/office2003/' ;
FCKConfig.StylesXmlPath		= '/admininc/styles.xml' ;
FCKConfig.TemplatesXmlPath	= '/admininc/templates.xml' ;
_FileBrowserLanguage	= 'aspx' ;	
_QuickUploadLanguage	= 'aspx' ;

FCKConfig.ImageUpload = true ;
FCKConfig.FirefoxSpellChecker = true;
FCKConfig.FormatSource = true;

FCKConfig.PluginsPath = '/admin/fckplugins/';


FCKConfig.Plugins.Add('OnlineVideo', 'en');