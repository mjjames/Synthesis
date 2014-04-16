
alter table [dbo].[projects]
add thumbnailimage nvarchar(255)

alter table [dbo].[pages]
alter column thumbnailimage nvarchar(255)

alter table [dbo].[articles]
alter column thumbnailimage nvarchar(255)

