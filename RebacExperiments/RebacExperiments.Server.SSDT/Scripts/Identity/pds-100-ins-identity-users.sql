PRINT 'Inserting [Identity].[User] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Identity].[User]
-----------------------------------------------
MERGE INTO [Identity].[User] AS [Target]
USING (VALUES 
     (1, 'Data Conversion Only', 'Data Conversion Only', 0, NULL, NULL, 1, @ValidFrom, @ValidTo)
    ,(2, 'Philipp Wagner',  'Philipp Wagner',   1, 'philipp@bytefish.de',   'AQAAAAIAAYagAAAAELbMFL9utkwA7FK4QoUCZEK/jPiHhTMzuFllrszW7FuCJBHjLVBCWXJCuFFJyRllYg==', 1, @ValidFrom, @ValidTo) --5!F25GbKwU3P
    ,(3, 'John Doe',        'John Doe',         0, 'john@doe.localhost',    NULL, 1, @ValidFrom, @ValidTo)
    ,(4, 'Max Powers',      'Max Powers',       0, 'max@powers.localhost',  NULL, 1, @ValidFrom, @ValidTo)
    ,(5, 'James Bond',      '007',              0, 'james@bond.localhost',  NULL, 1, @ValidFrom, @ValidTo)
    ,(6, 'John Connor',     'John Connor',      0, 'john@connor.localhost', NULL, 1, @ValidFrom, @ValidTo)
    ,(7, 'Max Mustermann',  'Max Mustermann',   1, 'max@mustermann.local',  'AQAAAAIAAYagAAAAELbMFL9utkwA7FK4QoUCZEK/jPiHhTMzuFllrszW7FuCJBHjLVBCWXJCuFFJyRllYg==', 1, @ValidFrom, @ValidTo) --5!F25GbKwU3P
) AS [Source]([UserID], [FullName], [PreferredName], [IsPermittedToLogon], [LogonName], [HashedPassword], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[UserID] = [Source].[UserID])
WHEN NOT MATCHED BY TARGET THEN
    INSERT 
        ([UserID], [FullName], [PreferredName], [IsPermittedToLogon], [LogonName], [HashedPassword], [LastEditedBy], [ValidFrom], [ValidTo])
    VALUES 
        ([Source].[UserID], [Source].[FullName], [Source].[PreferredName], [Source].[IsPermittedToLogon], [Source].[LogonName], [Source].[HashedPassword], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
