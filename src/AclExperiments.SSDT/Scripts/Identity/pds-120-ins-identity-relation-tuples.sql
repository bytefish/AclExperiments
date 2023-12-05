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
      (1, 'doc',            'doc_323',      'viewer', 'user', 'user_1', NULL, 1, @ValidFrom, @ValidTo) 
      -- doc:doc_152#parent@folder:folder_152#...
     ,(2, 'doc',            'doc_152',      'parent', 'folder', 'folder_152', '...', 1, @ValidFrom, @ValidTo)
     -- folder:folder_152#viewer@user_2
     ,(3, 'folder',         'folder_152',   'viewer', 'user', 'user_2', NULL, 1, @ValidFrom, @ValidTo)
     
     -- User "max@mustermann.local"
) AS [Source]
(
     [RelationTupleID] 
    ,[Namespace] 
    ,[Object]       
    ,[Relation]  
    ,[SubjectNamespace]
    ,[Subject]
    ,[SubjectRelation]
    ,[LastEditedBy]    
    ,[ValidFrom]       
    ,[ValidTo]         
)
ON (
    [Target].[Namespace] = [Source].[Namespace]
        AND [Target].[Object] = [Source].[Object]
        AND [Target].[Relation] = [Source].[Relation]
        AND [Target].[SubjectNamespace] = [Source].[SubjectNamespace]
        AND [Target].[Subject] = [Source].[Subject]
        AND [Target].[SubjectRelation] = [Source].[SubjectRelation]
)
WHEN NOT MATCHED BY TARGET THEN
    INSERT 
        (
             [RelationTupleID]
            ,[Namespace]
            ,[Object]
            ,[Relation]
            ,[SubjectNamespace]
            ,[Subject]
            ,[SubjectRelation]
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
            ,[SubjectNamespace]
            ,[Subject]
            ,[SubjectRelation]
            ,[Source].[LastEditedBy]
            ,[Source].[ValidFrom]
            ,[Source].[ValidTo]
        );