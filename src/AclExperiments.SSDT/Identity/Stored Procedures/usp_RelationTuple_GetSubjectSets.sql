CREATE PROCEDURE [Identity].[usp_RelationTuple_GetSubjectSets]
    @Namespace          NVARCHAR(50)
   ,@Object             NVARCHAR(50)
   ,@Relation           NVARCHAR(50)
   ,@SubjectNamespace   NVARCHAR(50)
AS BEGIN
    
    SET NOCOUNT ON;

    SELECT 
        r.[RelationTupleID]  
       ,r.[Namespace]        
       ,r.[Object]           
       ,r.[Relation]         
       ,r.[Subject]          
       ,r.[RowVersion]       
       ,r.[LastEditedBy]     
       ,r.[ValidFrom]        
       ,r.[ValidTo]
    FROM 
        [Identity].[RelationTuple] r
    WHERE 
        r.[Namespace] = @Namespace 
        AND r.[Object] = @Object 
        AND r.[Relation] = @Relation 
        AND r.[SubjectNamespace] IS NOT NULL 
        AND r.[Subject] IS NOT NULL 
        AND r.[SubjectRelation] IS NOT NULL;

END