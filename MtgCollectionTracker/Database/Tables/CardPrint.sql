create table dbo.CardPrint
(
	Id				int				identity not null,
	CardId			int				not null,
	SetId			int				not null,
	PictureUrl		nvarchar(max),
	FlipPictureUrl	nvarchar(max)

	constraint ixuc_CardPrint
		primary key clustered (Id),
	constraint fk_CardPrint_CardId
		foreign key (CardId)
		references dbo.[Card] (Id),
	constraint fk_CardPrint_SetId
		foreign key (SetId)
		references dbo.[Set] (Id)
);
go

create unique nonclustered index ixu_CardPrint_CardId_SetId
	on dbo.CardPrint (CardId, SetId);
