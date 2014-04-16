alter table [dbo].[sites]
add password nvarchar(255) null

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