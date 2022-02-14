create procedure [dbo].[Card_Insert]
	@Name	T_CardName,
	@Id		int out
as
begin
	if @Name is null
	begin
		raiserror('@Name cannot be null', 18, 1);
	end

	insert into [Card] ([Name])
	values (@Name);

	set @Id = scope_Identity();
end
