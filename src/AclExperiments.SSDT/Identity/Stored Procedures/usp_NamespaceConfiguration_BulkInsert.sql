CREATE PROCEDURE [Identity].[usp_NamespaceConfiguration_BulkInsert]
    @NamespaceConfigurations [Identity].[udt_NamespaceConfigurationType] ReadOnly
AS BEGIN
    
    SET NOCOUNT ON;

    INSERT INTO 
        [Identity].[NamespaceConfiguration]([Name], [Version], [Content], [LastEditedBy])
    SELECT 
        [Name] ,[Version], [Content], [LastEditedBy]     
    FROM 
        @NamespaceConfigurations;

END