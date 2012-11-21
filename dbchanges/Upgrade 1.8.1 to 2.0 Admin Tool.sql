
ALTER TABLE [dbo].[lookup] DROP CONSTRAINT [DF_lookup_active]
GO

alter table lookup
alter column active bit not null
go

ALTER TABLE [dbo].[lookup] ADD  CONSTRAINT [DF_lookup_active]  DEFAULT ((1)) FOR [active]
GO

update [aspnet_Applications]
set ApplicationName = 'synthesis', LoweredApplicationName = 'synthesis'
where ApplicationName = '/'
go
