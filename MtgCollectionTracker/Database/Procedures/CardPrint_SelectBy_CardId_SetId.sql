create procedure dbo.CardPrint_SelectBy_CardId_SetId
	@CardId int,
	@SetId int
as
begin
	if @CardId is null
	begin
		raiserror('@CardId cannot be null', 18, 1);
	end

	if @SetId is null
	begin
		raiserror('@SetId cannot be null', 18, 1);
	end

	select
		*
	from CardPrint
	where
		CardId = @CardId
		and SetId = @SetId;
end