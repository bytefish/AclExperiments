PRINT 'Inserting [Identity].[Role] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Identity].[Role]
-----------------------------------------------
MERGE INTO [Identity].[Role] AS [Target]
USING (VALUES 
      (1,	'User',             'User Role', NULL, 1, @ValidFrom, @ValidTo)
    , (2,	'Administrator',    'Aministrator Role', NULL, 1, @ValidFrom, @ValidTo)
) AS [Source]([RoleID], [Name], [Description],  [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[RoleID] = [Source].[RoleID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([RoleID], [Name], [Description],  [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[RoleID], [Source].[Name], [Source].[Description],  [Source].[RowVersion], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);