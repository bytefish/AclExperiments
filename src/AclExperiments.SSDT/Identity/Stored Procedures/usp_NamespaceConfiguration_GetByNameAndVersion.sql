CREATE PROCEDURE [Identity].[usp_NamespaceConfiguration_GetByNameAndVersion]
    @Name       NVARCHAR(255)
   ,@Version    INT
AS BEGIN

    SET NOCOUNT ON;

    SELECT TOP 1 
        [NamespaceConfigurationID]
       ,[Name]             
       ,[Version]          
       ,[Content]          
       ,[RowVersion]       
       ,[LastEditedBy]     
       ,[ValidFrom]        
       ,[ValidTo]          
    FROM 
        [Identity].[NamespaceConfiguration] n
    WHERE 
        n.[Name] = @Name AND n.[Version] = @Version
    ORDER BY 
        n.[Version] DESC;

END