create procedure dbo.[CardPrint_Insert]
	@CardId			int,
	@SetId			int,
	@PictureUrl		nvarchar(max),
	@FlipPictureUrl nvarchar(max),
	@Id				int out
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


	insert into CardPrint (CardId, SetId, PictureUrl, FlipPictureUrl)
	values (@CardId, @SetId, @PictureUrl, @FlipPictureUrl);

	set @Id = scope_Identity();
end
