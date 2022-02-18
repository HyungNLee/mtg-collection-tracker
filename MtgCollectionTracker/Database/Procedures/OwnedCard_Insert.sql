create procedure dbo.OwnedCard_Insert
	@CardPrintId	int,
	@CollectionId	int,
	@IsFoil			bit,
	@Id				int out
as
begin
	if @CardPrintId is null
	begin
		raiserror('@CardPrintId cannot be null', 18, 1);
	end

	if @CollectionId is null
	begin
		raiserror('@CollectionId cannot be null', 18, 1);
	end

	insert into OwnedCard (CardPrintId, CollectionId, IsFoil)
	values (@CardPrintId, @CollectionId, @IsFoil);

	set @Id = scope_Identity();
end
