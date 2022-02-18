create procedure dbo.Collection_Insert
	@Name			T_SetName,
	@IsDeck			bit,
	@MainboardId	int		= null,
	@SideboardId	int		= null,
	@Id				int out
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

	insert into [Collection] (
		[Name],
		IsDeck,
		MainboardId,
		SideboardId
	)
	values (
		@Name,
		@IsDeck,
		@MainboardId,
		@SideboardId
	);

	set @Id = scope_Identity();
end
