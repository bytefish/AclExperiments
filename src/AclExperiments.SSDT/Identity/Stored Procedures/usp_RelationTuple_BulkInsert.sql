CREATE PROCEDURE [Identity].[usp_RelationTuple_BulkInsert]
    @RelationTuples [Identity].[udt_RelationTupleType] ReadOnly
AS BEGIN

    SET NOCOUNT ON;

    INSERT INTO [Identity].[RelationTuple]
    (
        [Namespace]        
       ,[Object]           
       ,[Relation]         
       ,[Subject]          
       ,[LastEditedBy]     
    )
    SELECT 
        [Namespace]      
       ,[Object]         
       ,[Relation]       
       ,[Subject]        
       ,[LastEditedBy]   
    FROM 
        @RelationTuples;

END