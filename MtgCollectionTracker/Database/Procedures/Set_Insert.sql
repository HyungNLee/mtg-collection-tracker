create procedure [dbo].[Set_Insert]
	@Name	T_SetName,
	@Id		int out
as
begin
	if @Name is null
	begin
		raiserror('@Name cannot be null', 18, 1);
	end

	insert into [Set] ([Name])
	values (@Name);

	set @Id = scope_Identity();
end
