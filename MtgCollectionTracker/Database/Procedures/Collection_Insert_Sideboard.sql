create procedure dbo.Collection_Insert_Sideboard
	@MainboardId	int		= null,
	@Id				int out
as
begin
	declare @foundSideboardId int;
	declare @createdSideboardId int;
	declare @mainboardName T_SetName;

	if not exists (select 1 from [Collection] where Id = @MainboardId)
	begin
		raiserror('Mainboard deck does not exist', 18, 1);
	end

	select
		@foundSideboardId = SideboardId,
		@mainboardName = [Name]
	from [Collection]
	where Id = @MainboardId;

	if @foundSideboardId is not null
	begin
		raiserror('Sideboard already exists', 18, 1);
	end

	select	@mainboardName += ' - Sideboard';

	-- create sideboard deck
	declare
		@isDeck int = 1,
		@sideboardId int = null;

	exec Collection_Insert
		@mainboardName,
		@isDeck,
		@MainboardId,
		@sideboardId,
		@Id = @Id OUTPUT;

	-- Update mainboard deck with sideboard Id
	update [Collection]
	set SideboardId = @Id
	where Id = @MainboardId;
end
