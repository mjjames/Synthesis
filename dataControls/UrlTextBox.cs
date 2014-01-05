using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using mjjames.AdminSystem.dataControls;
using System.Web.UI.WebControls;
using mjjames.core;
using System.Collections.Specialized;
using System.Configuration;
using mjjames.AdminSystem.DataEntities;
using System.Web.UI;

namespace mjjames.AdminSystem.dataControls
{
	public class UrlControl : TextControl
	{
		public override System.Web.UI.Control GenerateControl(dataentities.AdminField field, object ourPage)
		{
			//first generate the textbox as usual
			var textbox = base.GenerateControl(field, ourPage) as TextBox;
			//next mark it as readonly so people don't think they can edit it
			textbox.ReadOnly = true;
			textbox.Enabled = false;
			textbox.CssClass += " URLClipboard input-xlarge";

			//we only have a url if we aren't a new page
			if (PKey > 0)
			{
				//lookup the full url based upon our data (ourPage)
				textbox.Text = LookupFullURL(ourPage);
			}

			
			//create our zero clipboard JS include

			var page = (Page)HttpContext.Current.Handler;
			var ourSM = ScriptManager.GetCurrent(page);
			var csm = page.ClientScript;

			//we use the ClientScriptManager to ensure it all gets loaded properly

			//has it already been registered?
			if(!csm.IsClientScriptIncludeRegistered("ZeroClipboard")){
				//register it
				csm.RegisterClientScriptInclude("ZeroClipboard", "http://jsresources.mjjames.co.uk/zeroclipboard/ZeroClipboard.js");
			}

			if(!csm.IsStartupScriptRegistered("ZeroClipboard-" + field.ID)){
				var script = "ZeroClipboard.setMoviePath( 'http://jsresources.mjjames.co.uk/zeroclipboard/ZeroClipboard10.swf' );$(function(){ var clip = new ZeroClipboard.Client();clip.setText($('input.URLClipboard').val());clip.glue($('img.URLClipboard')[0], $('img.URLClipboard').parent()[0]); function clipboardComplete(client, text){alert('URL has been copied to your clipboard');}clip.addEventListener( 'onComplete', clipboardComplete );});";
				csm.RegisterStartupScript(this.GetType(), "ZeroClipboard-" + field.ID, script, true);
			}
			
			var clipImageWrapper = new WebControl(System.Web.UI.HtmlTextWriterTag.Div)
			{
				ID = "clipboardImageWrapper",
				CssClass = "URLClipboardWrapper"
			};

			var clipImage = new Image(){
				ID="clipboardLink",
				CssClass="URLClipboard",
				ImageUrl="images/clipboard.png"
			};

			clipImageWrapper.Controls.Add(clipImage);

			//we need a holding container for our controls
			var container = new WebControl(System.Web.UI.HtmlTextWriterTag.Span)
			{
				ID= "controlWrapper"
			};
			//add our text box
			container.Controls.Add(textbox);
			container.Controls.Add(clipImageWrapper);
			
			return container;
		}
		
		new public static object GetDataValue(Control ourControl, Type ourType)
		{
			//call the base type's GetDataValue method
			return TextControl.GetDataValue(ourControl, ourType);
		}
		/// <summary>
		/// Looks up the full url for the url parameter within our data
		/// </summary>
		/// <param name="ourPage">Our Data</param>
		/// <returns>Full URL</returns>
		private string LookupFullURL(object ourPage)
		{
			
			var siteKey = 0;
			//build a config with any settings that effect the url generation
			var config = GenerateConfig(ourPage, out siteKey);

			var multiTenancyEnabled = ConfigurationManager.AppSettings["EnableMultiTenancy"] != null ?
									ConfigurationManager.AppSettings["EnableMultiTenancy"].Equals("true", StringComparison.CurrentCultureIgnoreCase) : false;
			//create a provider
			var ssmp = new SqlSiteMapProvider();
			//if multitenancy enabled we need the site key
			if (multiTenancyEnabled)
			{
				//set our sitekey
				ssmp.SiteKey = siteKey;
				ssmp.SiteRootURL = LookupSitePath(siteKey);
			}
			//initialize and build the sitemap
			ssmp.Initialize("Admin URL Lookup SiteMap", config);
			
			//if our key is that of the root node return the root url
			if (PKey.ToString() == ssmp.RootNode.Key)
			{
				return ssmp.RootNode.Url;
			}
			//lookup the node that matches this page
			//HACK: weird, the second time we load this the find method doesn't work even though there are nodes
			//so stick the method and linq it up, ideally we need to fix this
			var node = ssmp.RootNode.GetAllNodes().Cast<SiteMapNode>().FirstOrDefault(n => n.Key == PKey.ToString());
			//return the url or a blank string
			return node != null ? node.Url.Replace("//", "/") : "";
		}

		/// <summary>
		/// Given a site key looks up its root path
		/// </summary>
		/// <param name="siteKey"></param>
		/// <returns></returns>
		private string LookupSitePath(int siteKey)
		{
			var cache = HttpContext.Current.Cache;
			var cacheKey = "siteRootPath-" + siteKey;
			if (cache[cacheKey] != null)
			{
				return cache[cacheKey] as String;
			}
			var context = new AdminSystem.DataContexts.AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
			var siteData = (from s in context.sites
							where s.site_key == siteKey && s.active
							select s.hostname).FirstOrDefault();
			if (siteData == null)
			{
				return "";
			}
			var url = new Uri(siteData);
			cache[cacheKey] = url.AbsolutePath;
			return url.AbsolutePath;

		}

		/// <summary>
		/// Generates a config file for use in building a sitemap
		/// </summary>
		/// <param name="dataType"></param>
		/// <returns></returns>
		private NameValueCollection GenerateConfig(object data, out int siteKey)
		{
			var dataType = data.GetType().Name;
			//create a config with an initial option of our database details
			var config = new NameValueCollection();
			config.Add("connectionStringName", "ourDatabase");
			//by default urlReWriting Is Off
			var urlReWritingEnabled = false;
			siteKey = 0;
			var urlPrefix = "";
			switch (dataType)
			{
				case "page":
					urlReWritingEnabled = true;
					var realPage = data as page;
					siteKey = realPage.site_fkey.HasValue ? realPage.site_fkey.Value : 0;
					break;
				case "project":
					urlPrefix = ConfigurationManager.AppSettings["urlprefixProject"];
					var realProject = data as project;
					siteKey = realProject.site_fkey.HasValue ? realProject.site_fkey.Value : 0;
					break;
				case "article":
					urlPrefix = ConfigurationManager.AppSettings["urlprefixArticle"];
					var realArticle = data as article;
					siteKey = realArticle.site_fkey.HasValue ? realArticle.site_fkey.Value : 0;
					break;
				case "offer":
					urlPrefix = ConfigurationManager.AppSettings["urlprefixOffer"];
					var realOffer = data as offer;
					siteKey = realOffer.site_fkey.HasValue ? realOffer.site_fkey.Value : 0;
					break;
				default:
				break; 
			}


			//indicate whether the sitemap provider should turn on url rewriting
			config.Add("urlwriting", urlReWritingEnabled.ToString());

			//if we have a url prefix apply it now
			if(!String.IsNullOrEmpty(urlPrefix)){
				config.Add("urlprefix", urlPrefix);
			}
			return config;
		}
	}
}