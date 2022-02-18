create view dbo.ivw_OwnedCardSum
with schemabinding
as
	select
		CardPrintId,
		CollectionId,
		IsFoil,
		Count_Big(*) as [Count]
	from dbo.OwnedCard
	group by
		CardPrintId,
		CollectionId,
		IsFoil;
go

create unique clustered index ixuc_ivw_OwnedCardSum_CardPrintId
	on ivw_OwnedCardSum(CardPrintId, IsFoil, CollectionId);

go

create index ix_ivw_OwnedCardSum
	on ivw_OwnedCardSum(CollectionId);

