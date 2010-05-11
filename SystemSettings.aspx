<%@ Page Title="" Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
	CodeBehind="SystemSettings.aspx.cs" Inherits="mjjames.AdminSystem.SystemSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:ScriptManager ID="scriptmanagerAdmin" runat="server" AllowCustomErrorsRedirect="true"
		AsyncPostBackTimeout="60"  CompositeScript-ScriptMode="Release" ScriptMode="Release">
	</asp:ScriptManager>
	<h1 class="listingTitle">
		<asp:Literal runat="server" ID="dbEditorLabel" />
		System Settings
	</h1>
	<div id="colContainer">
		<div id="leftCol" runat="server" class="leftCol">
			<h2>
				Available Settings
			</h2>
			<div id="treeView">
				<ul>
					<li>
						<asp:LinkButton runat="server" ID="_TwitterOAuth" Text="Twitter Authentication" OnClick="LoadTwitterAuthentication" />
					</li>
					<li>
						<asp:LinkButton runat="server" ID="_ConfigSettings" Text="Config Settings" OnClick="LoadConfigSettings" />
					</li>
					<li>
						<asp:LinkButton runat="server" ID="_NavBar" Text="Navigation Settings" OnClick="LoadNavigation" />
					</li>
				</ul>
			</div>
		</div>
		<div id="rightCol">
			<h2 runat="server" id="settingLabel">
				Choose a Setting To Edit</h2>
			<asp:UpdatePanel ID="upSettings" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
				<contenttemplate>
					<asp:Panel runat="server" ID="panelInfo">
						<p>
							Please choose a system setting from the menu on the left.
						</p>
					</asp:Panel>
					<asp:Panel runat="server" ID="panelTwitter" Visible="false" CssClass="listingTable">
						<div class="twitterContainer">
							<p class="twitterError" runat="server" id="_InvalidTwitterSettings" visible="false">
								In order to use the MJJames CMS Twitter Integration the CMS Config must first have
								it's API Settings configured
							</p>
							<asp:UpdatePanel runat="server" ID="upTwitter" OnLoad="CheckTwitterSettings" Visible="false">
								<ContentTemplate>
									<asp:Panel runat="server" ID="panelAuthenticate" Visible="false">
										<p class="intro">
											In order for the the MJJames CMS to publish content to your Twitter stream please
											authenticate your Twitter account.<br />
											Please click Authenticate, login and allow the application access.<br />
											After authenticating please enter the pin supplied by Twitter and click Verify.
										</p>
										<p>
											<asp:HyperLink runat="server" ID="hlAuthenticate">Autheticate with Twitter</asp:HyperLink>
										</p>
										<p class="row">
											<asp:Label runat="server" ID="lblTwitterPin" AssociatedControlID="twitterPin" CssClass="labelwide">
											Authentication Pin:
											</asp:Label>
											<asp:TextBox runat="server" ID="twitterPin" CssClass="field" />
											<asp:LinkButton runat="server" ID="lbVerify" OnClick="twitterVerify" Text="Verify" />
										</p>
										
									</asp:Panel>
									<asp:Panel runat="server" ID="panelAuthenticated" Visible="false">
										<p>
											Authenticated User Screen Name: <asp:Literal runat="server" ID="_litTwitterScreenName" />
										</p>
										<asp:LinkButton runat="server" ID="_removeTwitterAuthentication" Text="Remove Twitter Authentication" OnClick="RemoveTwitterAuthentication" />
									</asp:Panel>
									<asp:Literal runat="server" ID="litStatus" />
								</ContentTemplate>
							</asp:UpdatePanel>
						</div>
					</asp:Panel>
					<asp:Panel runat="server" ID="panelConfig" Visible="false" CssClass="listingTable">
						<AjaxControlToolkit:TabContainer runat="server" ID="settingsTabContainer">
							<AjaxControlToolkit:TabPanel ID="tabTwitter" runat="server">
								<HeaderTemplate>
									Twitter
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="Label1" Text="Twitter Consumer Key:" CssClass="labelwide"
											AssociatedControlID="_twitterConsumerKey" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_twitterConsumerKey" Text='<%# GetSetting("twitterConsumerKey") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="Label2" Text="Twitter Cosumer Secret:" CssClass="labelwide"
											AssociatedControlID="_twitterConsumerSecret" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_twitterConsumerSecret" Text='<%# GetSetting("twitterConsumerSecret") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="Label3" Text="Publish Article Notification to Twitter:" CssClass="labelwide"
											AssociatedControlID="_twitterPublishArticles" />
										<asp:CheckBox runat="server" CssClass="checkbox" ID="_twitterPublishArticles" Checked='<%# GetSettingAsBool("twitterPublishArticles") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="Label4" Text="Publish Offer Notification to Twitter:" CssClass="labelwide"
											AssociatedControlID="_twitterPublishOffers" />
										<asp:CheckBox runat="server" CssClass="checkbox" ID="_twitterPublishOffers" Checked='<%# GetSettingAsBool("twitterPublishOffers") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" ID="tabGoogle">
								<HeaderTemplate>
									Google
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblAnalytics" Text="Google Analytics ID:" CssClass="labelwide"
											AssociatedControlID="_googleAnalyticsKey" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_googleAnalyticsKey" Text='<%# GetSetting("googleAnalyticsKey") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblSiteMap" Text="Google Sitemap Verify Code:" CssClass="labelwide"
											AssociatedControlID="_googleSiteMapKey" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_googleSiteMapKey" Text='<%# GetSetting("googleSiteMapKey") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblgcalFeed" Text="Google Calendar Feed:" CssClass="labelwide"
											AssociatedControlID="_gcalFeed" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_gcalFeed" Text='<%# GetSetting("gcalFeed") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblGoogleMapsLocation" Text="Google Maps Location:"
											CssClass="labelwide" AssociatedControlID="_GoogleMapsLocation" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_GoogleMapsLocation" Text='<%# GetSetting("GoogleMapsLocation") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" ID="tabFlickr">
								<HeaderTemplate>
									Flickr
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="Label5" Text="Flickr Api Key:" CssClass="labelwide"
											AssociatedControlID="_flickrApiKey" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_flickrApiKey" Text='<%# GetSetting("flickrApiKey") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="Label6" Text="Flickr Api Secret:" CssClass="labelwide"
											AssociatedControlID="_flickrApiSecret" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_flickrApiSecret" Text='<%# GetSetting("flickrApiSecret") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="Label7" Text="Flickr User:" CssClass="labelwide"
											AssociatedControlID="_flickrUser" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_flickrUser" Text='<%# GetSetting("flickrUser") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="Label8" Text="Flickr Creative Commons License:" CssClass="labelwide"
											AssociatedControlID="_flickrCCLicense" />
										<asp:DropDownList runat="server" CssClass="dropdownlist" ID="_flickrCCLicense" OnDataBinding="SetSelectedValue">
											<asp:ListItem Text="Attribution License" Value="4" />
											<asp:ListItem Text="Attribution-NoDerivs License" Value="6" />
											<asp:ListItem Text="Attribution-NonCommercial-NoDerivs License" Value="3" />
											<asp:ListItem Text="Attribution-NonCommercial License" Value="2" />
											<asp:ListItem Text="Attribution-NonCommercial-ShareAlike License" Value="1" />
											<asp:ListItem Text="Attribution-ShareAlike License" Value="5" />
										</asp:DropDownList>
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" ID="tabEmail">
								<HeaderTemplate>
									Email
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblSiteFromEmail" Text="Site From Email:" CssClass="labelwide"
											AssociatedControlID="_SiteFromEmail" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_SiteFromEmail" Text='<%# GetSetting("SiteFromEmail") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblenquiryEmailTo" Text="Enquiry Email To:" CssClass="labelwide"
											AssociatedControlID="_enquiryEmailTo" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_enquiryEmailTo" Text='<%# GetSetting("enquiryEmailTo") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblenquiryEmailBCC" Text="Enquiry Email BCC:" CssClass="labelwide"
											AssociatedControlID="_enquiryEmailBCC" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_enquiryEmailBCC" Text='<%# GetSetting("enquiryEmailBCC") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblenquiryEmailSubject" Text="Enquiry Email Subject:"
											CssClass="labelwide" AssociatedControlID="_enquiryEmailSubject" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_enquiryEmailSubject" Text='<%# GetSetting("enquiryEmailSubject") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblcallbackEmailToAddress" Text="Callback Email To:"
											CssClass="labelwide" AssociatedControlID="_callbackEmailToAddress" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_callbackEmailToAddress" Text='<%# GetSetting("callbackEmailToAddress") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblcallbackEmailFromAddress" Text="Callback Email From Address:"
											CssClass="labelwide" AssociatedControlID="_callbackEmailFromAddress" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_callbackEmailFromAddress" Text='<%# GetSetting("callbackEmailFromAddress") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblcallbackEmailSubject" Text="Callback Email Subject:"
											CssClass="labelwide" AssociatedControlID="_callbackEmailSubject" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_callbackEmailSubject" Text='<%# GetSetting("callbackEmailSubject") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" ID="tabNewsletter">
								<HeaderTemplate>
									Newsletter
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblNewsletterFromAddress" Text="Newsletter From Address:"
											CssClass="labelwide" AssociatedControlID="_NewsletterFromAddress" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_NewsletterFromAddress" Text='<%# GetSetting("NewsletterFromAddress") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblNewsletterConfirmation" Text="Newsletter Confirmation URL:"
											CssClass="labelwide" AssociatedControlID="_NewsletterConfirmation" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_NewsletterConfirmation" Text='<%# GetSetting("NewsletterConfirmation") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblNewsletterUnSubscribe" Text="Callback Email From Address:"
											CssClass="labelwide" AssociatedControlID="_NewsletterUnSubscribe" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_NewsletterUnSubscribe" Text='<%# GetSetting("NewsletterUnSubscribe") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" ID="tabRecaptcha">
								<HeaderTemplate>
									ReCaptcha
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblenquiryCaptchaPublicKey" Text="Captcha Public Key:"
											CssClass="labelwide" AssociatedControlID="_enquiryCaptchaPublicKey" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_enquiryCaptchaPublicKey" Text='<%# GetSetting("enquiryCaptchaPublicKey") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblenquiryCaptchaPrivateKey" Text="Captcha Private Key:"
											CssClass="labelwide" AssociatedControlID="_enquiryCaptchaPrivateKey" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_enquiryCaptchaPrivateKey" Text='<%# GetSetting("enquiryCaptchaPrivateKey") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" ID="tabAPI">
								<HeaderTemplate>
									API Keys
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblLiveSearchAPIKey" Text="Bing Search API Key:" CssClass="labelwide"
											AssociatedControlID="_LiveSearchAPIKey" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_LiveSearchAPIKey" Text='<%# GetSetting("LiveSearchAPIKey") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblGoogleMapsApiKey" Text="Google Maps Api Key:" CssClass="labelwide"
											AssociatedControlID="_GoogleMapsApiKey" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_GoogleMapsApiKey" Text='<%# GetSetting("GoogleMapsApiKey") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" ID="tabAdmin">
								<HeaderTemplate>
									Admin Tool
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblFCKeditorUserFilesPath" Text="FCKeditor User Files Path"
											CssClass="labelwide" AssociatedControlID="_FCKeditor__UserFilesPath" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_FCKeditor__UserFilesPath" Text='<%# GetSetting("FCKeditor:UserFilesPath") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblFCKeditorBasePath" Text="FCKeditor Base Path:" CssClass="labelwide"
											AssociatedControlID="_FCKeditor__BasePath" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_FCKeditor__BasePath" Text='<%# GetSetting("FCKeditor:BasePath") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lbluploaddir" Text="Upload Directory:" CssClass="labelwide"
											AssociatedControlID="_uploaddir" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_uploaddir" Text='<%# GetSetting("uploaddir") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblfileTypes" Text="Valid Upload File Types:" CssClass="labelwide"
											AssociatedControlID="_fileTypes" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_fileTypes" Text='<%# GetSetting("fileTypes") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblmaxFileSize" Text="Max Upload File Size:" CssClass="labelwide"
											AssociatedControlID="_maxFileSize" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_maxFileSize" Text='<%# GetSetting("maxFileSize") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lbladminConfigXML" Text="Path to Admin Config XML:"
											CssClass="labelwide" AssociatedControlID="_adminConfigXML" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_adminConfigXML" Text='<%# GetSetting("adminConfigXML") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" ID="tabFrontend">
								<HeaderTemplate>
									Front End
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblTestimonialPaging" Text="Testimonial Paging Size:"
											CssClass="labelwide" AssociatedControlID="_TestimonialPaging" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_TestimonialPaging" Text='<%# GetSetting("TestimonialPaging") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblSiteName" Text="Site Name:" CssClass="labelwide" AssociatedControlID="_SiteName" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_SiteName" Text='<%# GetSetting("SiteName") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblDomainName" Text="Domain Name:" CssClass="labelwide"
											AssociatedControlID="_DomainName" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_DomainName" Text='<%# GetSetting("DomainName") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblcopyright" Text="CopyRight:" CssClass="labelwide" AssociatedControlID="_copyright" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_copyright" Text='<%# GetSetting("copyright") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblXmlRedirectsLocation" Text="Xml Redirects Location:"
											CssClass="labelwide" AssociatedControlID="_XmlRedirectsLocation" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_XmlRedirectsLocation" Text='<%# GetSetting("XmlRedirectsLocation") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="cacheLocation" Text="Image Cache Location:" CssClass="labelwide"
											AssociatedControlID="_cacheLocation" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_cacheLocation" Text='<%# GetSetting("cacheLocation") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblPodcastFeed" Text="Podcast Feed URL:" CssClass="labelwide"
											AssociatedControlID="_PodcastFeed" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_PodcastFeed" Text='<%# GetSetting("PodcastFeed") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblPodcastPaging" Text="Podcast Paging Size:" CssClass="labelwide"
											AssociatedControlID="_PodcastPaging" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_PodcastPaging" Text='<%# GetSetting("PodcastPaging") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblnavExceptions" Text="Nav Word Exceptions:" CssClass="labelwide"
											AssociatedControlID="_navExceptions" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_navExceptions" Text='<%# GetSetting("navExceptions") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblvalidExternalDomains" Text="Valid External Domains:"
											CssClass="labelwide" AssociatedControlID="_validExternalDomains" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_validExternalDomains" Text='<%# GetSetting("validExternalDomains") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblBibleGateWayVersion" Text="Bible GateWay Version:"
											CssClass="labelwide" AssociatedControlID="_BibleGateWayVersion" />
										<asp:DropDownList runat="server" CssClass="dropdownlist" ID="_BibleGateWayVersion" OnDataBinding="SetSelectedValue">
											<asp:ListItem Text="-- Please Select --" Value="" />
											<asp:ListItem Text="Amplified Bible" Value="45" />
											<asp:ListItem Text="American Standard Version" Value="8"  />
											<asp:ListItem Text="Contemporary English Version" Value="46"  />
											<asp:ListItem Text="Darby Translation" Value="16"  />
											<asp:ListItem Text="Douay-Rheims 1899 American Edition." Value="63" />
											<asp:ListItem Text="English Standard Version" Value="47" />
											<asp:ListItem Text="Good News Translation" Value="86" />
											<asp:ListItem Text="Holman Christian Standard Bible" Value="77" />
											<asp:ListItem Text="21st Century King James Version" Value="48"  />
											<asp:ListItem Text="King James Version" Value="9" />
											<asp:ListItem Text="The Message" Value="65"  />
											<asp:ListItem Text="New American Standard Bible" Value="49"  />
											<asp:ListItem Text="New Century Version" Value="78" />
											<asp:ListItem Text="New International Reader's Version" Value="76" />
											<asp:ListItem Text="New International Version" Value="31" />
											<asp:ListItem Text="New International Version - UK" Value="64" />
											<asp:ListItem Text="New King James Version Value="50"  />
											<asp:ListItem Text="New Living Translation" Value="51"  />
											<asp:ListItem Text="Today's New International Version" Value="72"  />
											<asp:ListItem Text="Worldwide English (New Testament" Value="73" />
											<asp:ListItem Text="Wycliffe New Testament" Value="53" />
											<asp:ListItem Text="Young's Literal Translation" Value="15"  />
										</asp:DropDownList>
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" ID="tabURLs">
								<HeaderTemplate>
									URL Settings
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lbldefault_rewritepath" Text="Path to Default Rewrite File"
											CssClass="labelwide" AssociatedControlID="_default_rewritepath" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_default_rewritepath" Text='<%# GetSetting("default_rewritepath") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="lbldefault_rewritepath_param" Text="Default Rewrite Parameter Name:"
											CssClass="labelwide" AssociatedControlID="_default_rewritepath_param" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_default_rewritepath_param" Text='<%# GetSetting("default_rewritepath_param") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="lblurlprefixPage" Text="Page Url Prefix:"
											CssClass="labelwide" AssociatedControlID="_urlprefixPage" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_urlprefixPage" Text='<%# GetSetting("urlprefixPage") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="lblurlprefixProject" Text="Project Url Prefix:"
											CssClass="labelwide" AssociatedControlID="_urlprefixProject" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_urlprefixProject" Text='<%# GetSetting("urlprefixProject") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="lblurlprefixArticle" Text="Articles Url Prefix:"
											CssClass="labelwide" AssociatedControlID="_urlprefixArticle" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_urlprefixArticle" Text='<%# GetSetting("urlprefixArticle") %>' />
									</span>
									<span class="row">
										<asp:Label runat="server" ID="lblurlprefixOffer" Text="Offers Url Prefix:"
											CssClass="labelwide" AssociatedControlID="_urlprefixOffer" />
										<asp:TextBox runat="server" CssClass="textbox" ID="_urlprefixOffer" Text='<%# GetSetting("urlprefixOffer") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
						</AjaxControlToolkit:TabContainer>
						<span class="row">
							<asp:LinkButton runat="server" ID="_btnSave" OnClick="SaveConfig" Text="Save" />
							<asp:Literal runat="server" ID="configStatus" />
						</span>
					</asp:Panel>
					<asp:Panel ID="panelNavigation" runat="server" Visible="false">
						<asp:ListView runat="server" ID="_navItems" OnInit="SetAdminToolbar">
							<LayoutTemplate>
								<div class="container">
									<div runat="server" id="itemPlaceHolder" />
								</div>
							</LayoutTemplate>
							<ItemTemplate>
								<span class="row">
									<asp:Label runat="server" ID="item" Text='<%# Eval("name") %>' CssClass="label" />
									<asp:DropDownList runat="server" CssClass="field" OnDataBinding="SetAccessLevel">
										<asp:ListItem Text="None" Value="0" />
										<asp:ListItem Text="All" Value="1" />
										<asp:ListItem Text="Article Editor" Value="2" />
										<asp:ListItem Text="Content Editor" Value="3" />
										<asp:ListItem Text="Editor" Value="4" />
										<asp:ListItem Text="Site Admin" Value="5" />
										<asp:ListItem Text="System Admin" Value="6" />
									</asp:DropDownList>
								</span>
							</ItemTemplate>
						</asp:ListView>
						<asp:LinkButton runat="server" ID="lbSaveNavgation" OnClick="SaveNavigationSettings" Text="Save" />
					</asp:Panel>
				</contenttemplate>
			</asp:UpdatePanel>
			<asp:UpdateProgress ID="_updateProgress1" runat="server" AssociatedUpdatePanelID="upSettings"
				DisplayAfter="500">
				<progresstemplate>
					<p class="loading">
						Loading ...</p>
				</progresstemplate>
			</asp:UpdateProgress>
		</div>
	</div>
</asp:Content>
