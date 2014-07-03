CREATE DATABASE [dbname.domain-auditLog]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'auditLog', FILENAME = N'D:\Databases\dbname.domain-auditLog.mdf' , SIZE = 5824KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'auditLog_log', FILENAME = N'D:\Databases\dbname.domain-auditLog.LDF' , SIZE = 3136KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

CREATE TABLE [dbo].[AuditLogItem](
	 [Area] nvarchar(512) not null,
                [Details] nvarchar(max) not null,
                [EventType] nvarchar(50) not null,
                [Timestamp] datetimeoffset not null,
                [User] nvarchar(254) not null
 CONSTRAINT [PK_audit_log_items] PRIMARY KEY CLUSTERED 
(
	[TimeStamp],
	[EventType],
	[User]
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO