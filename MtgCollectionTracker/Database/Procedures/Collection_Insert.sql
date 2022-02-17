create procedure dbo.Collection_Insert
	@Name	T_SetName,
	@IsDeck bit,
	@Id		int out
as
begin
	if @Name is null
	begin
		raiserror('@Name cannot be null', 18, 1);
	end

	if @IsDeck is null
	begin
		raiserror('@IsDeck cannot be null', 18, 1);
	end

	insert into [Collection] ([Name], IsDeck)
	values (@Name, @IsDeck);

	set @Id = scope_Identity();
end
