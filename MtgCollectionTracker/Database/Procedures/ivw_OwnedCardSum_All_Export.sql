create procedure [dbo].[ivw_OwnedCardSum_All_Export]
as
	select 
		cd.[Name] as CardName,
		s.[Name] as SetName,
		c.[Name] as CollectionName,
		c.IsDeck,
		[IsFoil],
		[Count]
	from [ivw_OwnedCardSum] as ocs
	inner join [Collection] as c on ocs.CollectionId = c.Id
	inner join [CardPrint] as cp on ocs.CardPrintId = cp.Id
	inner join [Card] as cd on cp.CardId = cd.Id
	inner join [Set] as s on cp.SetId = s.Id
	order by c.Id;