CREATE TYPE [Identity].[udt_NamespaceConfigurationType] AS TABLE 
(
     [NamespaceConfigurationID]      INT           
    ,[Name]                          NVARCHAR(255) 
    ,[Version]                       INT           
    ,[Content]                       NVARCHAR(255) 
    ,[RowVersion]                    ROWVERSION    
    ,[LastEditedBy]                  INT           
    ,[ValidFrom]                     DATETIME2 (7) 
    ,[ValidTo]                       DATETIME2 (7) 
);