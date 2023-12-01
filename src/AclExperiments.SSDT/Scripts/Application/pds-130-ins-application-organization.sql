PRINT 'Inserting [Application].[Organization] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[Organization]
-----------------------------------------------
MERGE INTO [Application].[Organization] AS [Target]
USING (VALUES 
      (1,	'Organization #1',    'Organization #1', NULL, 1, @ValidFrom, @ValidTo)
    , (2,	'Organization #2',    'Organization #2', NULL, 1, @ValidFrom, @ValidTo)
) AS [Source]([OrganizationID], [Name], [Description],  [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[OrganizationID] = [Source].[OrganizationID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([OrganizationID], [Name], [Description],  [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[OrganizationID], [Source].[Name], [Source].[Description],  [Source].[RowVersion], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);