CREATE TYPE [Identity].[udt_NamespaceConfigurationType] AS TABLE 
(
     [NamespaceConfigurationID]      INT           
    ,[Name]                          NVARCHAR(255) 
    ,[Content]                       NVARCHAR(MAX) 
    ,[Version]                       INT           
    ,[RowVersion]                    VARBINARY(8)    
    ,[LastEditedBy]                  INT           
    ,[ValidFrom]                     DATETIME2 (7) 
    ,[ValidTo]                       DATETIME2 (7) 
);