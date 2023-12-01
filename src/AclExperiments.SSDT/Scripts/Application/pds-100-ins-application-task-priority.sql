PRINT 'Inserting [Application].[UserTaskPriority] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[UserTaskPriority]
-----------------------------------------------
MERGE INTO [Application].[UserTaskPriority] AS [Target]
USING (VALUES 
			  (1, 'Low', 1, @ValidFrom, @ValidTo)
			, (2, 'Normal', 1, @ValidFrom, @ValidTo)
			, (3, 'High', 1, @ValidFrom, @ValidTo)
		) AS [Source]([UserTaskPriorityID], [Name], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[UserTaskPriorityID] = [Source].[UserTaskPriorityID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([UserTaskPriorityID], [Name], [LastEditedBy], [ValidFrom], [ValidTo]) 
	VALUES 
		([Source].[UserTaskPriorityID], [Source].[Name], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);