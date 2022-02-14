create table dbo.[Set]
(
	Id		int				identity not null,
	[Name]	T_SetName		not null,

	constraint ixuc_Set
		primary key clustered (Id)
);
go

create unique nonclustered index ixu_Set_Name
	on dbo.[Set] ([Name]);
