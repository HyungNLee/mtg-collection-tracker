create table dbo.[Set]
(
	Id		int				identity not null,
	[Name]	nvarchar(100)	not null,

	constraint ixuc_Set
		primary key clustered (Id)
);
go

create unique nonclustered index ixu_Set_Name
	on dbo.[Set] ([Name]);
