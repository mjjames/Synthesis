alter table [dbo].[sites]
add password nvarchar(255) null

go

insert into [dbo].[lookup]
([lookup_id], [title], [type], [value], [active])
values
('sitelookup','Site Lookup','lookup_type','sitelookup',1)

insert into [dbo].[lookup]
([lookup_id], [title], [type], [value], [active])
values
('pagelookup','Page Lookup','lookup_type','pagelookup',1)

insert into [dbo].[lookup]
([lookup_id], [title], [type], [value], [active])
values
('projectlookup','Project Lookup','lookup_type','projectlookup',1)

insert into [dbo].[lookup]
([lookup_id], [title], [type], [value], [active])
values
('articlelookup','Article Lookup','lookup_type','articlelookup',1)

insert into [dbo].[lookup]
([lookup_id], [title], [type], [value], [active])
values
('medialookup','Media Lookup','lookup_type','medialookup',1)

insert into [dbo].[lookup]
([lookup_id], [title], [type], [value], [active])
values
('marketingsitelookup','Marketing Site Lookup','lookup_type','marketingsitelookup',1)

go

alter table [dbo].[pages]
alter column [pageid] nvarchar(255) null

alter table [dbo].[pages]
alter column [linkurl] nvarchar(255) null

go

alter table [dbo].[media]
add [publishedonutc] datetime default GetUtcDate() not null
go

alter table [dbo].[media]
alter column [title] nvarchar(255) null
go

alter table [dbo].[articles]
alter column [title] nvarchar(255) null
go

alter table [dbo].[pages]
alter column [title] nvarchar(255) null
go

/****** Object:  StoredProcedure [dbo].[proc_GetSiteMap]    Script Date: 10/06/2014 09:26:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		mjjames
-- Description:	Modified [proc_GetSiteMap] to include optional sitekey filter
-- =============================================

ALTER PROCEDURE [dbo].[proc_GetSiteMap]
@filter nvarchar(max), 
@urlprefix nvarchar(50) = ' ', 
@urlwriting bit = 0,
@siteKey int = 0
AS

DECLARE @sql nvarchar(max)
DECLARE @parent nvarchar(max) 
DECLARE @url nvarchar(max)
DECLARE @visible nvarchar(max)
DECLARE @siteScoping nvarchar(max)

select @visible = '1' --visible by default

select @parent = '[page_fkey]'
select  @url = ''''+@urlprefix + ''' +  CAST([page_key] AS varchar)'

if @siteKey > 0
BEGIN
	select @siteScoping = ' AND [site_fkey] =' + CAST(@siteKey AS varchar)
END

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
	FROM [pages] WHERE [active] = 1 ' + @siteScoping + ' ORDER BY [parent], [sortorder]'




exec (@sql)

	RETURN

/****** Object:  StoredProcedure [dbo].[usp_GetHomeNavigation]    Script Date: 03/24/2010 22:30:52 ******/
SET ANSI_NULLS ON
