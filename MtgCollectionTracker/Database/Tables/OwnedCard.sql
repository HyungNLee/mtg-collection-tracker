create table dbo.OwnedCard
(
	Id				int		identity not null,
	CardPrintId		int		not null,
	CollectionId	int		not null,

	constraint ixuc_OwnedCard
		primary key clustered (Id),
	constraint fk_OwnedCard_CardPrintId
		foreign key (CardPrintId)
		references dbo.CardPrint (Id),
	constraint fk_OwnedCard_CollectionId
		foreign key (CollectionId)
		references dbo.[Collection] (Id)
)
