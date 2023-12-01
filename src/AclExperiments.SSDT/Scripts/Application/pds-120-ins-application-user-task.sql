PRINT 'Inserting [Application].[UserTaskStatus] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[UserTask]
-----------------------------------------------
MERGE INTO [Application].[UserTask] AS [Target]
USING (VALUES 
      (152,	'Call Back',        'Call Back Philipp Wagner',    	NULL, NULL, NULL, NULL,	1,	1,	NULL, 1, @ValidFrom, @ValidTo)
    , (323,	'Sign Document',    'You need to Sign a Document',	NULL, NULL, NULL, NULL,	2,	2,	NULL, 1, @ValidFrom, @ValidTo)
) AS [Source]([UserTaskID], [Title], [Description], [DueDateTime] , [ReminderDateTime], [CompletedDateTime], [AssignedTo], [UserTaskPriorityID], [UserTaskStatusID], [RowVersion] , [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[UserTaskID] = [Source].[UserTaskID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([UserTaskID], [Title], [Description], [DueDateTime] , [ReminderDateTime], [CompletedDateTime], [AssignedTo], [UserTaskPriorityID], [UserTaskStatusID], [RowVersion] , [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[UserTaskID], [Source].[Title], [Source].[Description], [Source].[DueDateTime] , [Source].[ReminderDateTime], [Source].[CompletedDateTime], [Source].[AssignedTo], [Source].[UserTaskPriorityID], [Source].[UserTaskStatusID], [Source].[RowVersion] , [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);