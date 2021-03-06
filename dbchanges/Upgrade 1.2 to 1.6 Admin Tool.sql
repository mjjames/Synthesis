/*
	1.2 to 1.6 Upgrade Script
*/

/* Table Changes */

/* 1. Table Renames */

EXEC sp_rename 'page', 'pages';
GO

/* 2. New Columns */

ALTER TABLE [pages]
ADD lastmodified DateTime Not Null Default GetDate()
GO

ALTER TABLE [pages]
ADD [page_url] nvarchar(max) Null
GO

ALTER TABLE [pages]
ADD [passwordprotect] bit null
GO

ALTER TABLE [pages]
ADD [password] nvarchar(max) null
GO

/* 3. New Tables */

/****** Object:  Table [dbo].[media]    Script Date: 11/10/2009 22:49:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[media](
	[media_key] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](50) NOT NULL,
	[description] [text] NULL,
	[filename] [nvarchar](max) NOT NULL,
	[mediatype_lookup] [int] NOT NULL,
	[active] [bit] NOT NULL,
	[link] [nvarchar](max) NULL,
 CONSTRAINT [PK_media] PRIMARY KEY CLUSTERED 
(
	[media_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[media]  WITH CHECK ADD  CONSTRAINT [FK_media_lookup] FOREIGN KEY([mediatype_lookup])
REFERENCES [dbo].[lookup] ([lookup_key])
GO

ALTER TABLE [dbo].[media] CHECK CONSTRAINT [FK_media_lookup]
GO

ALTER TABLE [dbo].[media] ADD  CONSTRAINT [DF_media_active]  DEFAULT ((0)) FOR [active]
GO

/****** Object:  Table [dbo].[media_links]    Script Date: 11/10/2009 22:48:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[media_links](
	[medialink_key] [int] IDENTITY(1,1) NOT NULL,
	[media_fkey] [int] NOT NULL,
	[link_fkey] [int] NOT NULL,
	[linktype_lookup] [int] NOT NULL,
 CONSTRAINT [PK_media_links] PRIMARY KEY CLUSTERED 
(
	[medialink_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[media_links]  WITH CHECK ADD  CONSTRAINT [FK_media_links_lookup] FOREIGN KEY([linktype_lookup])
REFERENCES [dbo].[lookup] ([lookup_key])
GO

ALTER TABLE [dbo].[media_links] CHECK CONSTRAINT [FK_media_links_lookup]
GO

ALTER TABLE [dbo].[media_links]  WITH CHECK ADD  CONSTRAINT [FK_media_links_media_links] FOREIGN KEY([media_fkey])
REFERENCES [dbo].[media] ([media_key])
GO

ALTER TABLE [dbo].[media_links] CHECK CONSTRAINT [FK_media_links_media_links]
GO

/* Stored Proc Changes */

/* 1. Main Navigation */

/****** Object:  StoredProcedure [dbo].[proc_GetSiteMap]    Script Date: 11/10/2009 21:52:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[proc_GetSiteMap]
@filter nvarchar(max), @urlprefix nvarchar(50) = ' ', @urlwriting bit = 0
AS

DECLARE @sql nvarchar(max)
DECLARE @parent nvarchar(max) 
DECLARE @url nvarchar(max)
DECLARE @visible nvarchar(max)

select @visible = '1' --visible by default

select @parent = '[page_fkey]'
select  @url = ''''+@urlprefix + ''' +  CAST([page_key] AS varchar)'


if @urlwriting != 0 
BEGIN
	select @url = '[page_url]'
END

if @filter != ''
BEGIN
 select @visible = 'case ' + @filter  + ' when 1 then 1 else 0 end'
END

    select @sql =	'SELECT [page_key] AS [ID], COALESCE([NavTitle], [Title]) AS [Title], [metadescription] AS [Description], '
					 + @url + ' AS [Url], NULL AS[Roles], ' + @parent + 'AS [Parent],'
					+ @visible + ' AS [visible], [accesskey] AS [AccessKey], [lastmodified] AS [LastModified]
    FROM [pages] WHERE [active] = 1 ORDER BY [parent], [sortorder]'




exec (@sql)

	RETURN

/* 2. Featured Nav Item */

/****** Object:  StoredProcedure [dbo].[getFeatured]    Script Date: 11/10/2009 21:58:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[getFeatured]
	
	(
	@maxnumber int,
	@randomize tinyint
	)
AS
	DECLARE @sql nvarchar(max)
		SELECT @sql = 'SELECT TOP ' + CAST(@maxnumber AS nvarchar) +' [page_key], [navtitle], [thumbnailimage], newid() AS [random] FROM PAGES WHERE [showonhome] = 1 AND [active] = 1'
	
	IF @randomize = 1
		BEGIN
			select @sql = @sql + ' ORDER BY [random]'
		END
		
	EXECUTE (@sql)
	RETURN



/* 1.5 - 1.6 Changes */

exec aspnet_Roles_CreateRole '/', 'Editor'
exec aspnet_Roles_CreateRole '/', 'System Admin'
exec aspnet_Roles_CreateRole '/', 'Site Admin'
exec aspnet_Roles_CreateRole '/', 'None'
exec aspnet_Roles_CreateRole '/', 'Content Editor'
exec aspnet_Roles_CreateRole '/', 'Article Aditor'

GO
DECLARE	@return_value int

EXEC	@return_value = [dbo].[aspnet_UsersInRoles_AddUsersToRoles]
		@ApplicationName = N'/',
		@UserNames = N'mjjames',
		@RoleNames = N'system admin',
		@CurrentTimeUtc = NULL

SELECT	'Return Value' = @return_value

GO
