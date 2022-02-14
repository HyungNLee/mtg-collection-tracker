create table dbo.[Collection]
(
	Id		int				identity not null,
	[Name]	nvarchar(150)	not null,
	IsDeck	bit				not null,

	constraint ixuc_Collection
		primary key clustered (Id)
);
go

create unique nonclustered index ixu_Collection_Name
	on dbo.[Collection] ([Name]);
