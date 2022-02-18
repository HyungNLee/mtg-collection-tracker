create table dbo.[Collection]
(
	Id				int			identity not null,
	[Name]			T_SetName	not null,
	IsDeck			bit			not null,

	-- If this collection is a sideboard it will have a 'MainboardId'
	-- which is the main deck collection Id.
	MainboardId		int			null,

	-- This SideboardId references this table. A collection/deck/sideboard are all 'collections'
	-- but if it is a deck, it has the ability to have a sideboard.
	SideboardId		int			null

	constraint ixuc_Collection
		primary key clustered (Id),
	constraint chk_Collection_Deck check (
		(IsDeck = 0 and MainboardId is null and SideboardId is null)
		or
		(IsDeck = 1 and 
			(
				-- Deck is mainboard without a sideboard created.
				(MainboardId is null and SideboardId is null)
				or
				-- Deck is a sideboard.
				(MainboardId is not null and SideboardId is null)
				or
				-- Deck is a mainboard with a sideboard created.
				(MainboardId is null and SideboardId is not null)
			)
		)
	)
);
go

create unique nonclustered index ixu_Collection_Name
	on dbo.[Collection] ([Name]);
