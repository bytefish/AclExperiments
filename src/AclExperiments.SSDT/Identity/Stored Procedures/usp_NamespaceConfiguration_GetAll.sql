CREATE PROCEDURE [Identity].[usp_NamespaceConfiguration_GetAll]
AS BEGIN
    
    SET NOCOUNT ON;
    
    WITH cte AS (
        SELECT 
             [NamespaceConfigurationID]
            ,[Name]             
            ,[Version]          
            ,[Content]          
            ,[RowVersion]       
            ,[LastEditedBy]     
            ,[ValidFrom]        
            ,[ValidTo]
            ,row_number() OVER (PARTITION BY [Name] ORDER BY [Version] DESC) AS rn
        FROM
            [Identity].[NamespaceConfiguration]
   )
    SELECT 
        [NamespaceConfigurationID]
       ,[Name]             
       ,[Version]          
       ,[Content]          
       ,[RowVersion]       
       ,[LastEditedBy]     
       ,[ValidFrom]        
       ,[ValidTo]          
    FROM 
        cte
    WHERE 
        rn = 1;

END