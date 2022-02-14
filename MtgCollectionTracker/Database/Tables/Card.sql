create table dbo.[Card]
(
	Id		int				identity not null,
	[Name]	T_CardName		not null,

	constraint ixuc_Card
		primary key clustered (Id)
);
go

create unique nonclustered index ixu_Card_Name
	on dbo.[Card] ([Name]);
