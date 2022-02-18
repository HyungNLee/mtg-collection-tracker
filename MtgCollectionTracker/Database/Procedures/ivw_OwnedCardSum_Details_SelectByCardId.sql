create procedure dbo.ivw_OwnedCardSum_Details_SelectBy_CardId
	@CardId int
as
begin
	select
		cpd.CardId,
		cpd.CardName,
		ocs.CardPrintId,
		cpd.SetId,
		cpd.SetName,
		ocs.CollectionId,
		c.[Name] as [CollectionName],
		ocs.IsFoil,
		cpd.PictureUrl,
		cpd.FlipPictureUrl,
		ocs.[Count]
	from ivw_OwnedCardSum as ocs
	inner join [Collection] as c on ocs.CollectionId = c.Id
	inner join vw_CardPrintDetails as cpd on ocs.CardPrintId = cpd.Id
	where cpd.CardId = @CardId;
end