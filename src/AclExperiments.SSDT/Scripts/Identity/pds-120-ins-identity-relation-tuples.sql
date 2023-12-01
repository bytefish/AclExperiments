PRINT 'Inserting [Identity].[RelationTuple] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Identity].[RelationTuple]
-----------------------------------------------
MERGE INTO [Identity].[RelationTuple] AS [Target]
USING (VALUES 

    --- The Relationship-Table is given below.
    --- 
    --- ObjectKey           |  ObjectNamespace  |   ObjectRelation  |   SubjectKey          |   SubjectNamespace    |   SubjectRelation
    --- --------------------|-------------------|-------------------|-----------------------|-----------------------|-------------------
    --- :task_323  :        |   UserTask        |       viewer      |   :organization_1:    |       Organization    |   member
    --- :task_152  :        |   UserTask        |       viewer      |   :organization_1:    |       Organization    |   member
    --- :task_152  :        |   UserTask        |       viewer      |   :organization_2:    |       Organization    |   member
    --- :organization_1:    |   Organization    |       member      |   :user_2:            |       User            |   NULL
    --- :organization_2:    |   Organization    |       member      |   :user_7:            |       User            |   NULL
    --- :role_1:            |   Role            |       member      |   :user_2:            |       User            |   NULL
    --- :role_2:            |   Role            |       member      |   :user_2:            |       User            |   NULL
    --- :role_1:            |   Role            |       member      |   :user_7:            |       User            |   NULL
    --- :task_323:          |   UserTask        |       owner       |   :user_2:            |       User            |   member
      (1, 'UserTask', 323, 'viewer', 'Organization',   1, 'member', 1, @ValidFrom, @ValidTo)
     ,(2, 'UserTask', 152, 'viewer', 'Organization',   2, 'member', 1, @ValidFrom, @ValidTo)
     ,(3, 'UserTask', 152, 'viewer', 'Organization',   1, 'member', 1, @ValidFrom, @ValidTo)
     ,(4, 'Organization',  1,   'member', 'User',  2, NULL, 1, @ValidFrom, @ValidTo)
     ,(5, 'Organization',  2,   'member', 'User',  7, NULL, 1, @ValidFrom, @ValidTo)
     ,(6, 'Role',          1,   'member', 'User',  2, NULL, 1, @ValidFrom, @ValidTo)
     ,(7, 'Role',          2,   'member', 'User',  2, NULL, 1, @ValidFrom, @ValidTo)
     ,(8, 'Role',          1,   'member', 'User',  7, NULL, 1, @ValidFrom, @ValidTo)
     ,(9, 'UserTask', 323, 'owner', 'User',   2, NULL, 1, @ValidFrom, @ValidTo)
     
     -- User "max@mustermann.local"
) AS [Source]
(
     [RelationTupleID] 
    ,[ObjectNamespace] 
    ,[ObjectKey]       
    ,[ObjectRelation]  
    ,[SubjectNamespace]
    ,[SubjectKey]      
    ,[SubjectRelation] 
    ,[LastEditedBy]    
    ,[ValidFrom]       
    ,[ValidTo]         
)
ON (
    [Target].[RelationTupleID] = [Source].[RelationTupleID]
)
WHEN NOT MATCHED BY TARGET THEN
    INSERT 
        (
             [RelationTupleID]
            ,[ObjectNamespace]
            ,[ObjectKey]
            ,[ObjectRelation]
            ,[SubjectNamespace]
            ,[SubjectKey]
            ,[SubjectRelation]
            ,[LastEditedBy]
            ,[ValidFrom]
            ,[ValidTo]
        )
    VALUES 
        (
             [Source].[RelationTupleID]
            ,[Source].[ObjectNamespace]
            ,[Source].[ObjectKey]
            ,[Source].[ObjectRelation]
            ,[Source].[SubjectNamespace]
            ,[Source].[SubjectKey]
            ,[Source].[SubjectRelation]
            ,[Source].[LastEditedBy]
            ,[Source].[ValidFrom]
            ,[Source].[ValidTo]
        );