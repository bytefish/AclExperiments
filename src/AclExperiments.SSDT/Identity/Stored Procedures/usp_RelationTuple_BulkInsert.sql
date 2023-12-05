CREATE PROCEDURE [Identity].[usp_RelationTuple_BulkInsert]
    @RelationTuples [Identity].[udt_RelationTupleType] ReadOnly
AS BEGIN

    SET NOCOUNT ON;

    INSERT INTO [Identity].[RelationTuple]
    (
        [Namespace]        
       ,[Object]           
       ,[Relation]         
       ,[SubjectNamespace]          
       ,[Subject]          
       ,[SubjectRelation]
       ,[LastEditedBy]     
    )
    SELECT 
        [Namespace]
       ,[Object]
       ,[Relation]
       ,[SubjectNamespace]
       ,[Subject]
       ,[SubjectRelation]
       ,[LastEditedBy]   
    FROM 
        @RelationTuples;

END