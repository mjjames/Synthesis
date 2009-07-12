<%@ Page Title="" Language="C#" MasterPageFile="~/AdminSystem.master" AutoEventWireup="true"
	CodeBehind="SystemSettings.aspx.cs" Inherits="SystemSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:ScriptManager ID="scriptmanagerAdmin" runat="server" AllowCustomErrorsRedirect="true"
		AsyncPostBackTimeout="60" CompositeScript-ScriptMode="Auto" ScriptMode="Auto">
	</asp:ScriptManager>
	<h1 class="listingTitle">
		<asp:Literal runat="server" ID="dbEditorLabel" />
		System Settings
	</h1>
	<div id="colContainer">
		<div id="leftCol" runat="server" class="leftCol">
			<h2>
				Available Settings</h2>
			<div id="treeView">
				<ul>
					<li>
						<asp:LinkButton runat="server" ID="_TwitterAccount" Text="Twitter Account" OnClick="loadTwitterSettings" />
					</li>
					<li>
						<asp:LinkButton runat="server" ID="_GoogleSettings" Text="Config Settings" OnClick="LoadConfigSettings" />
					</li>
				</ul>
			</div>
		</div>
		<div id="rightCol">
			<h2 runat="server" id="settingLabel">
				Choose a Setting To Edit</h2>
			<asp:UpdatePanel ID="upSettings" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
				<ContentTemplate>
					<asp:Panel runat="server" ID="info">
						<p>
							Please choose a system setting from the menu on the left.</p>
					</asp:Panel>
					<asp:Panel runat="server" ID="panelConfig" Visible="false">
						<AjaxControlToolkit:TabContainer runat="server" ID="settingsTabContainer">
							<AjaxControlToolkit:TabPanel runat="server">
								<HeaderTemplate>
									Google Account Settings
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblAnalytics" Text="Google Analytics ID:" CssClass="label"
											AssociatedControlID="_googleAnalyticsKey" />
										<asp:TextBox runat="server" ID="_googleAnalyticsKey" Text='<%# GetSetting("googleAnalyticsKey") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblSiteMap" Text="Google Sitemap Verify Code:" CssClass="label"
											AssociatedControlID="_googleSiteMapKey" />
										<asp:TextBox runat="server" ID="_googleSiteMapKey" Text='<%# GetSetting("googleSiteMapKey") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblgcalFeed" Text="Google Calendar Feed:" CssClass="label"
											AssociatedControlID="_gcalFeed" />
										<asp:TextBox runat="server" ID="_gcalFeed" Text='<%# GetSetting("gcalFeed") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblGoogleMapsLocation" Text="Google Maps Location:"
											CssClass="label" AssociatedControlID="_GoogleMapsLocation" />
										<asp:TextBox runat="server" ID="_GoogleMapsLocation" Text='<%# GetSetting("GoogleMapsLocation") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" >
								<HeaderTemplate>
									Email Settings
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblSiteFromEmail" Text="Site From Email:" CssClass="label"
											AssociatedControlID="_SiteFromEmail" />
										<asp:TextBox runat="server" ID="_SiteFromEmail" Text='<%# GetSetting("SiteFromEmail") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblenquiryEmailTo" Text="Enquiry Email To:" CssClass="label"
											AssociatedControlID="_enquiryEmailTo" />
										<asp:TextBox runat="server" ID="_enquiryEmailTo" Text='<%# GetSetting("enquiryEmailTo") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblenquiryEmailBCC" Text="Enquiry Email BCC:" CssClass="label"
											AssociatedControlID="_enquiryEmailBCC" />
										<asp:TextBox runat="server" ID="_enquiryEmailBCC" Text='<%# GetSetting("enquiryEmailBCC") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblenquiryEmailSubject" Text="Enquiry Email Subject:"
											CssClass="label" AssociatedControlID="_enquiryEmailSubject" />
										<asp:TextBox runat="server" ID="_enquiryEmailSubject" Text='<%# GetSetting("enquiryEmailSubject") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblcallbackEmailToAddress" Text="Callback Email To:"
											CssClass="label" AssociatedControlID="_callbackEmailToAddress" />
										<asp:TextBox runat="server" ID="_callbackEmailToAddress" Text='<%# GetSetting("callbackEmailToAddress") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblcallbackEmailFromAddress" Text="Callback Email From Address:"
											CssClass="label" AssociatedControlID="_callbackEmailFromAddress" />
										<asp:TextBox runat="server" ID="_callbackEmailFromAddress" Text='<%# GetSetting("callbackEmailFromAddress") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblcallbackEmailSubject" Text="Callback Email Subject:"
											CssClass="label" AssociatedControlID="_callbackEmailSubject" />
										<asp:TextBox runat="server" ID="_callbackEmailSubject" Text='<%# GetSetting("callbackEmailSubject") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" >
								<HeaderTemplate>
									Newsletter Settings
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblNewsletterFromAddress" Text="Newsletter From Address:"
											CssClass="label" AssociatedControlID="_NewsletterFromAddress" />
										<asp:TextBox runat="server" ID="_NewsletterFromAddress" Text='<%# GetSetting("NewsletterFromAddress") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblNewsletterConfirmation" Text="Newsletter Confirmation URL:"
											CssClass="label" AssociatedControlID="_NewsletterConfirmation" />
										<asp:TextBox runat="server" ID="_NewsletterConfirmation" Text='<%# GetSetting("NewsletterConfirmation") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblNewsletterUnSubscribe" Text="Callback Email From Address:"
											CssClass="label" AssociatedControlID="_NewsletterUnSubscribe" />
										<asp:TextBox runat="server" ID="_NewsletterUnSubscribe" Text='<%# GetSetting("NewsletterUnSubscribe") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" >
								<HeaderTemplate>
									ReCaptcha Settings
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblenquiryCaptchaPublicKey" Text="Captcha Public Key:"
											CssClass="label" AssociatedControlID="_enquiryCaptchaPublicKey" />
										<asp:TextBox runat="server" ID="_enquiryCaptchaPublicKey" Text='<%# GetSetting("enquiryCaptchaPublicKey") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblenquiryCaptchaPrivateKey" Text="Captcha Private Key:"
											CssClass="label" AssociatedControlID="_enquiryCaptchaPrivateKey" />
										<asp:TextBox runat="server" ID="_enquiryCaptchaPrivateKey" Text='<%# GetSetting("enquiryCaptchaPrivateKey") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" >
								<HeaderTemplate>
									API Keys
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblLiveSearchAPIKey" Text="Bing Search API Key:" CssClass="label"
											AssociatedControlID="_LiveSearchAPIKey" />
										<asp:TextBox runat="server" ID="_LiveSearchAPIKey" Text='<%# GetSetting("LiveSearchAPIKey") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblGoogleMapsApiKey" Text="Google Maps Api Key:" CssClass="label"
											AssociatedControlID="_GoogleMapsApiKey" />
										<asp:TextBox runat="server" ID="_GoogleMapsApiKey" Text='<%# GetSetting("GoogleMapsApiKey") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" >
								<HeaderTemplate>
									Admin Tool</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lblFCKeditorUserFilesPath" Text="FCKeditor User Files Path"
											CssClass="label" AssociatedControlID="_FCKeditor__UserFilesPath" />
										<asp:TextBox runat="server" ID="_FCKeditor__UserFilesPath" Text='<%# GetSetting("FCKeditor:UserFilesPath") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblFCKeditorBasePath" Text="FCKeditor Base Path:" CssClass="label"
											AssociatedControlID="_FCKeditor__BasePath" />
										<asp:TextBox runat="server" ID="_FCKeditor__BasePath" Text='<%# GetSetting("FCKeditor:BasePath") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lbluploaddir" Text="Upload Directory:" CssClass="label"
											AssociatedControlID="_uploaddir" />
										<asp:TextBox runat="server" ID="_uploaddir" Text='<%# GetSetting("uploaddir") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblfileTypes" Text="Valid Upload File Types:" CssClass="label"
											AssociatedControlID="_fileTypes" />
										<asp:TextBox runat="server" ID="_fileTypes" Text='<%# GetSetting("fileTypes") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblmaxFileSize" Text="Max Upload File Size:" CssClass="label"
											AssociatedControlID="_maxFileSize" />
										<asp:TextBox runat="server" ID="_maxFileSize" Text='<%# GetSetting("maxFileSize") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lbladminConfigXML" Text="Path to Admin Config XML:"
											CssClass="label" AssociatedControlID="_adminConfigXML" />
										<asp:TextBox runat="server" ID="_adminConfigXML" Text='<%# GetSetting("adminConfigXML") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
							<AjaxControlToolkit:TabPanel runat="server" >
								<HeaderTemplate>
									Front End
								</HeaderTemplate>
								<ContentTemplate>
									<span class="row">
										<asp:Label runat="server" ID="lbldefault_rewritepath" Text="Path to Default Rewrite File"
											CssClass="label" AssociatedControlID="_default_rewritepath" />
										<asp:TextBox runat="server" ID="_default_rewritepath" Text='<%# GetSetting("default_rewritepath") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lbldefault_rewritepath_param" Text="Default Rewrite Parameter Name:"
											CssClass="label" AssociatedControlID="_default_rewritepath_param" />
										<asp:TextBox runat="server" ID="_default_rewritepath_param" Text='<%# GetSetting("default_rewritepath_param") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblTestimonialPaging" Text="Testimonial Paging Size:"
											CssClass="label" AssociatedControlID="_TestimonialPaging" />
										<asp:TextBox runat="server" ID="_TestimonialPaging" Text='<%# GetSetting("TestimonialPaging") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblSiteName" Text="Site Name:" CssClass="label" AssociatedControlID="_SiteName" />
										<asp:TextBox runat="server" ID="_SiteName" Text='<%# GetSetting("SiteName") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblDomainName" Text="Domain Name:" CssClass="label"
											AssociatedControlID="_DomainName" />
										<asp:TextBox runat="server" ID="_DomainName" Text='<%# GetSetting("DomainName") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblcopyright" Text="CopyRight:" CssClass="label" AssociatedControlID="_copyright" />
										<asp:TextBox runat="server" ID="_copyright" Text='<%# GetSetting("copyright") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblXmlRedirectsLocation" Text="Xml Redirects Location:"
											CssClass="label" AssociatedControlID="_XmlRedirectsLocation" />
										<asp:TextBox runat="server" ID="_XmlRedirectsLocation" Text='<%# GetSetting("XmlRedirectsLocation") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="cacheLocation" Text="Image Cache Location:" CssClass="label"
											AssociatedControlID="_cacheLocation" />
										<asp:TextBox runat="server" ID="_cacheLocation" Text='<%# GetSetting("cacheLocation") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblPodcastFeed" Text="Podcast Feed URL:" CssClass="label"
											AssociatedControlID="_PodcastFeed" />
										<asp:TextBox runat="server" ID="_PodcastFeed" Text='<%# GetSetting("PodcastFeed") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblPodcastPaging" Text="Podcast Paging Size:" CssClass="label"
											AssociatedControlID="_PodcastPaging" />
										<asp:TextBox runat="server" ID="_PodcastPaging" Text='<%# GetSetting("PodcastPaging") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblnavExceptions" Text="Nav Word Exceptions:" CssClass="label"
											AssociatedControlID="_navExceptions" />
										<asp:TextBox runat="server" ID="_navExceptions" Text='<%# GetSetting("navExceptions") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblvalidExternalDomains" Text="Valid External Domains:"
											CssClass="label" AssociatedControlID="_validExternalDomains" />
										<asp:TextBox runat="server" ID="_validExternalDomains" Text='<%# GetSetting("validExternalDomains") %>' />
									</span><span class="row">
										<asp:Label runat="server" ID="lblBibleGateWayVersion" Text="Bible GateWay Version:"
											CssClass="label" AssociatedControlID="_BibleGateWayVersion" />
										<asp:TextBox runat="server" ID="_BibleGateWayVersion" Text='<%# GetSetting("BibleGateWayVersion") %>' />
									</span>
								</ContentTemplate>
							</AjaxControlToolkit:TabPanel>
						</AjaxControlToolkit:TabContainer>
						<span class="row">
							<asp:LinkButton runat="server" ID="btnSave" OnClick="SaveConfig" Text="Save" />
							<asp:Literal runat="server" ID="configStatus" />
						</span>
					</asp:Panel>
				</ContentTemplate>
			</asp:UpdatePanel>
			<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upSettings"
				DisplayAfter="500">
				<ProgressTemplate>
					<p class="loading">
						Loading ...</p>
				</ProgressTemplate>
			</asp:UpdateProgress>
		</div>
	</div>
</asp:Content>
