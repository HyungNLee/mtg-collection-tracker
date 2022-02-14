create procedure [dbo].[Set_SelectBy_Name]
	@Name T_CardName
as
begin
	if @Name is null
	begin
		raiserror('@Name cannot be null', 18, 1);
	end

	select
		*
	from [Set]
	where [Name] = @Name;
end
