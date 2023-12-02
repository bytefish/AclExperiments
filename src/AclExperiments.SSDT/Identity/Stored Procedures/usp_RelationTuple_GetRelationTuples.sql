CREATE PROCEDURE [Identity].[usp_RelationTuple_GetRelationTuples]
    @Namespace  NVARCHAR(50)
   ,@Object     NVARCHAR(50)
   ,@Relation   NVARCHAR(50)
   ,@Subject    NVARCHAR(50)
AS BEGIN

    SET NOCOUNT ON;

    SELECT 
        [RelationTupleID]  
       ,[Namespace]        
       ,[Object]           
       ,[Relation]         
       ,[Subject]          
       ,[RowVersion]       
       ,[LastEditedBy]     
       ,[ValidFrom]        
       ,[ValidTo]
    FROM
        [Identity].[RelationTuple]
    WHERE
        (@Namespace IS NULL OR [Namespace] = @Namespace)
            AND (@Object IS NULL OR [Object] = @Object)
            AND (@Relation IS NULL OR [Relation]  = @Relation)
            AND (@Subject IS NULL OR [Subject] LIKE @Subject);

END