CREATE PROCEDURE [Identity].[usp_NamespaceConfiguration_GetLatestByName]
    @Name NVARCHAR(255)
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
        n.[Name] = @Name
    ORDER BY 
        n.[Version] DESC;

END