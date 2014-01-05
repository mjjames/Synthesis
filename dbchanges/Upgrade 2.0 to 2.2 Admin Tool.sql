alter table [dbo].[offers]
add pagetitle nvarchar(max) null,
metadescription nvarchar(max) null

alter table [dbo].[projects]
add pagetitle nvarchar(max) null,
metadescription nvarchar(max) null

alter table [dbo].[articles]
add pagetitle nvarchar(max) null,
metadescription nvarchar(max) null

alter table [dbo].[pages]
add linkurlispermenant bit not null default(0)
