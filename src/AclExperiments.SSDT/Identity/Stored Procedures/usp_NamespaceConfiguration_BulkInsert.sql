CREATE PROCEDURE [Identity].[usp_NamespaceConfiguration_BulkInsert]
    @NamespaceConfigurations [Identity].[udt_NamespaceConfigurationType] ReadOnly
AS BEGIN
    
    SET NOCOUNT ON;

    INSERT INTO 
        [Identity].[NamespaceConfiguration]([NamespaceConfigurationID], [Name], [Version], [Content], [LastEditedBy])
    SELECT 
        [NamespaceConfigurationID],[Name] ,[Version], [Content], [LastEditedBy]     
    FROM 
        @NamespaceConfigurations;

END