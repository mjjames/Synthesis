/*
 Admin Tool 1.7 Upgrade Script
*/

/* New Tables */


/****** Object:  Table [dbo].[keyvalues]    Script Date: 02/13/2010 20:59:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[keyvalues](
	[keyvalue_key] [int] NOT NULL,
	[link_fkey] [int] NOT NULL,
	[key_lookup] [int] NOT NULL,
	[value] [nvarchar](max) NOT NULL,
	[link_lookup] [int] NOT NULL,
 CONSTRAINT [PK_keyvalues] PRIMARY KEY CLUSTERED 
(
	[keyvalue_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[keyvalues]  WITH CHECK ADD  CONSTRAINT [FK_keyvalues_lookup] FOREIGN KEY([link_lookup])
REFERENCES [dbo].[lookup] ([lookup_key])
GO

ALTER TABLE [dbo].[keyvalues] CHECK CONSTRAINT [FK_keyvalues_lookup]
GO

ALTER TABLE [dbo].[keyvalues]  WITH CHECK ADD  CONSTRAINT [FK_keyvalues_lookup1] FOREIGN KEY([key_lookup])
REFERENCES [dbo].[lookup] ([lookup_key])
GO

ALTER TABLE [dbo].[keyvalues] CHECK CONSTRAINT [FK_keyvalues_lookup1]
GO

/****** Object:  Table [dbo].[sites]    Script Date: 01/26/2010 13:49:04 ******/



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[sites](
	[site_key] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[hostname] [nvarchar](max) NOT NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK_sites] PRIMARY KEY CLUSTERED 
(
	[site_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[sites] ADD  CONSTRAINT [DF_sites_active]  DEFAULT ((0)) FOR [active]
GO



/****** Object:  Table [dbo].[site_users]    Script Date: 01/26/2010 13:49:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[site_users](
	[siteuser_key] [int] IDENTITY(1,1) NOT NULL,
	[site_fkey] [int] NOT NULL,
	[userid] [uniqueidentifier] NOT NULL,
	[roleid] [uniqueidentifier] NOT NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK_site_users] PRIMARY KEY CLUSTERED 
(
	[siteuser_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[site_users] ADD  CONSTRAINT [DF_site_users_active]  DEFAULT ((0)) FOR [active]
GO

/* New Columns */

Alter Table Articles 
Add site_fkey int null

Alter Table banners
Add site_fkey int null

Alter Table media
Add site_fkey int null

Alter Table media_links
Add site_fkey int null

Alter Table NewsletterReciprients
Add site_fkey int null

Alter Table Newsletters
Add site_fkey int null

Alter Table offers
Add site_fkey int null

Alter Table pages
Add site_fkey int null

Alter Table projects
Add site_fkey int null

Alter Table testimonies
Add site_fkey int null

GO
/* New Foreign Key Constraints */

ALTER TABLE dbo.site_users ADD CONSTRAINT
	FK_site_users_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.site_users ADD CONSTRAINT
	FK_site_users_aspnet_Users FOREIGN KEY
	(
	userid
	) REFERENCES dbo.aspnet_Users
	(
	UserId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.site_users ADD CONSTRAINT
	FK_site_users_aspnet_Roles FOREIGN KEY
	(
	roleid
	) REFERENCES dbo.aspnet_Roles
	(
	RoleId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.site_users SET (LOCK_ESCALATION = TABLE)
GO


ALTER TABLE dbo.articles ADD CONSTRAINT
	FK_site_articles_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.banners ADD CONSTRAINT
	FK_site_banners_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.media ADD CONSTRAINT
	FK_site_media_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.media_links ADD CONSTRAINT
	FK_site_media_links_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.NewsletterReciprients ADD CONSTRAINT
	FK_site_NewsletterReciprients_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.Newsletters ADD CONSTRAINT
	FK_site_Newsletters_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.offers ADD CONSTRAINT
	FK_site_offers_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.pages ADD CONSTRAINT
	FK_site_pages_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.projects ADD CONSTRAINT
	FK_site_projects_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.testimonies ADD CONSTRAINT
	FK_site_testimonies_sites FOREIGN KEY
	(
	site_fkey
	) REFERENCES dbo.sites
	(
	site_key
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO


/****** Object:  Table [dbo].[marketingsites]    Script Date: 02/09/2010 20:04:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[marketingsites](
	[marketingsite_key] [int] IDENTITY(1,1) NOT NULL,
	[site_fkey] [int] NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[host_name] [nvarchar](255) NOT NULL,
	[active] [bit] NOT NULL,
	[body] [ntext] NOT NULL,
	[short_description] [ntext] NOT NULL,
	[template_lookup] [int] NOT NULL,
 CONSTRAINT [PK_marketingsites] PRIMARY KEY CLUSTERED 
(
	[marketingsite_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[marketingsites]  WITH CHECK ADD  CONSTRAINT [FK_marketingsites_sites] FOREIGN KEY([site_fkey])
REFERENCES [dbo].[sites] ([site_key])
GO

ALTER TABLE [dbo].[marketingsites] CHECK CONSTRAINT [FK_marketingsites_sites]
GO

ALTER TABLE [dbo].[marketingsites] ADD  CONSTRAINT [DF_marketingsites_active]  DEFAULT ((1)) FOR [active]
GO

ALTER TABLE [dbo].[marketingsites]  WITH CHECK ADD  CONSTRAINT [FK_marketingsites_lookup] FOREIGN KEY([template_lookup])
REFERENCES [dbo].[lookup] ([lookup_key])
GO

ALTER TABLE [dbo].[marketingsites] CHECK CONSTRAINT [FK_marketingsites_lookup]
GO

