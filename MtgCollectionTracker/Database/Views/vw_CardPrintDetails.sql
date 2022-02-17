create view vw_CardPrintDetails
with schemabinding
as
	select
		cp.Id,
		cp.CardId,
		c.[Name] as CardName,
		cp.SetId,
		s.[Name] as SetName,
		cp.PictureUrl,
		cp.FlipPictureUrl
	from dbo.CardPrint as cp
	inner join dbo.[Card] as c on cp.CardId = c.Id
	inner join dbo.[Set] as s on cp.SetId = s.Id;
