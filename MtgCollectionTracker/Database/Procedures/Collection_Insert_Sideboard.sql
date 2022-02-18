create procedure dbo.Collection_Insert_Sideboard
	@MainboardId	int		= null,
	@Id				int out
as
begin try
	declare @foundSideboardId int;
	declare @foundMaideboardId int;
	declare @createdSideboardId int;
	declare @mainboardName T_SetName;

	if not exists (select 1 from [Collection] where Id = @MainboardId)
	begin
		raiserror('Mainboard deck does not exist', 18, 1);
	end

	select
		@foundMaideboardId = MainboardId,
		@foundSideboardId = SideboardId,
		@mainboardName = [Name]
	from [Collection]
	where Id = @MainboardId;

	-- Check if sideboard already exists
	if @foundSideboardId is not null
	begin
		raiserror('Sideboard already exists', 18, 1);
	end

	-- Check if this deck is a sideboard
	if @foundMaideboardId is not null
	begin
		raiserror('Cannot create a sideboard for a sideboard.', 18, 1);
	end

	select	@mainboardName += ' - Sideboard';

	-- Create sideboard deck
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
end try
begin catch
	declare @message varchar(MAX) = Error_Message();

	raiserror(@message, 18, 1);
end catch
