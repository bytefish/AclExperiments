PRINT 'Inserting [Identity].[RelationTuple] ...'
GO

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'


-----------------------------------------------
-- [Identity].[NamespaceConfiguration]
-----------------------------------------------


DECLARE @nsconfig_doc NVARCHAR(MAX) = '
﻿name: "doc"
    relation { name: "owner" }
    
    relation {
        name: "editor"

        userset_rewrite {
            union {
                child { _this {} }
                child { computed_userset { relation: "owner" } }
            } } }
    
    relation {
        name: "viewer"
        userset_rewrite {
            union {
                child { _this {} }
                child { computed_userset { relation: "editor" } }
                child { tuple_to_userset {
                    tupleset { 
                        relation: "parent" 
                    }
                    computed_userset {
                        relation: "viewer"
                } } }
} } }';

DECLARE @nsconfig_folder NVARCHAR(MAX) = '
name: "folder"

relation { name: "parent" }

relation { name: "owner" }

relation {
  name: "editor"
  userset_rewrite {
    union {
      child { _this {} }
      child { computed_userset { relation: "owner" } }
}}}

relation {
  name: "viewer"
  userset_rewrite {
  union {
    child { _this {} }
    child { computed_userset { relation: "editor" } }
}}}';

MERGE INTO [Identity].[NamespaceConfiguration] AS [Target]
USING (VALUES 
       (1, 'doc',            1,      @nsconfig_doc, 1, @ValidFrom, @ValidTo) 
      ,(2, 'folder',         1,      @nsconfig_folder, 1, @ValidFrom, @ValidTo) 
) AS [Source]
(
     [NamespaceConfigurationID] 
    ,[Name] 
    ,[Version]       
    ,[Content]  
    ,[LastEditedBy]    
    ,[ValidFrom]       
    ,[ValidTo]         
)
ON (
    [Target].[NamespaceConfigurationID] = [Source].[NamespaceConfigurationID]
)
WHEN NOT MATCHED BY TARGET THEN
    INSERT 
        (
             [NamespaceConfigurationID] 
            ,[Name] 
            ,[Version]       
            ,[Content]  
            ,[LastEditedBy]    
            ,[ValidFrom]       
            ,[ValidTo]         
        )
    VALUES 
        (
             [Source].[NamespaceConfigurationID] 
            ,[Source].[Name] 
            ,[Source].[Version]       
            ,[Source].[Content]  
            ,[Source].[LastEditedBy]    
            ,[Source].[ValidFrom]       
            ,[Source].[ValidTo]         
        );