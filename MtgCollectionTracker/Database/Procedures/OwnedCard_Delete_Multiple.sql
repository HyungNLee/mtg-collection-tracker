create procedure dbo.OwnedCard_Delete_Multiple
	@CardPrintId		int,
	@CollectionId		int,
	@IsFoil				bit,
	@NumberToDelete		int
as
begin
	delete top (@NumberToDelete)
	from OwnedCard
	where
		CardPrintId = @CardPrintId
		and CollectionId = @CollectionId
		and IsFoil = @IsFoil;
end