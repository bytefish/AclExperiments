PRINT 'Inserting [Identity].[RelationTuple] ...'
GO

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
    --- Namespace           |  Object           |   Relation        |   Subject                 |
    --- --------------------|-------------------|-------------------|---------------------------|
    --- doc                 |   doc_323         |       viewer      |   user_1                  |
    --- doc                 |   doc_152         |       viewer      |   folder:folder_152#...   |
    --- folder              |   folder_152      |       viewer      |   user_2                  |
    ---
      -- doc:doc_232#viewer@user_1
      (1, 'doc',            'doc_323',      'viewer', 'user_1', 1, @ValidFrom, @ValidTo) 
      -- doc:doc_152#parent@folder:folder_152#...
     ,(2, 'doc',            'doc_152',      'parent', 'folder:folder_152#...',   2, 'member', 1, @ValidFrom, @ValidTo)
     -- folder:folder_152#viewer@user_2
     ,(3, 'folder',         'folder_152',   'viewer', 'user_2', 1, @ValidFrom, @ValidTo)
     
     -- User "max@mustermann.local"
) AS [Source]
(
     [RelationTupleID] 
    ,[Namespace] 
    ,[Object]       
    ,[Relation]  
    ,[Subject]
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
            ,[Namespace]
            ,[Object]
            ,[Relation]
            ,[Subject]
            ,[LastEditedBy]
            ,[ValidFrom]
            ,[ValidTo]
        )
    VALUES 
        (
             [Source].[RelationTupleID]
            ,[Source].[Namespace]
            ,[Source].[Object]
            ,[Source].[Relation]
            ,[Source].[Subject]
            ,[Source].[LastEditedBy]
            ,[Source].[ValidFrom]
            ,[Source].[ValidTo]
        );