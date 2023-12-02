CREATE PROCEDURE [Identity].[usp_RelationTuple_BulkDelete]
    @RelationTuples [Identity].[udt_RelationTupleType] ReadOnly
AS BEGIN
    
    SET NOCOUNT ON;

    DELETE r
    FROM [Identity].[RelationTuple] r
        INNER JOIN  @RelationTuples d ON r.[Namespace] = d.[Namespace] 
            AND r.[Object] = d.[Object] 
            AND r.[Relation] = d.[Relation]
            AND r.[Subject] = d.[Subject];

END