CREATE PROCEDURE [Identity].[usp_NamespaceConfiguration_BulkDelete]
    @NamespaceConfigurations [Identity].[udt_NamespaceConfigurationType] ReadOnly
AS BEGIN
    
    SET NOCOUNT ON;

    DELETE n
    FROM [Identity].[NamespaceConfiguration] n
        INNER JOIN  @NamespaceConfigurations d ON n.[Name] = d.[Name] AND n.[Version] = d.[Version];

END