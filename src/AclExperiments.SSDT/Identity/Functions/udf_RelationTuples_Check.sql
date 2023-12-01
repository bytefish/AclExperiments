CREATE FUNCTION [Identity].[udf_RelationTuples_Check]
(
     @ObjectNamespace NVARCHAR(50)
    ,@ObjectKey INT
    ,@ObjectRelation NVARCHAR(50)
    ,@SubjectNamespace NVARCHAR(50)
    ,@SubjectKey INT
)
RETURNS BIT
AS
BEGIN

    DECLARE @IsAuthorized BIT = 0;

    WITH RelationTuples AS
    (
       SELECT
    	   [RelationTupleID]
          ,[ObjectNamespace]
          ,[ObjectKey]
          ,[ObjectRelation]
          ,[SubjectNamespace]
          ,[SubjectKey]
          ,[SubjectRelation]
    	  , 0 AS [HierarchyLevel]
        FROM
          [Identity].[RelationTuple]
        WHERE
    		[ObjectNamespace] = @ObjectNamespace AND [ObjectKey] = @ObjectKey AND [ObjectRelation] = @ObjectRelation
    	  
    	UNION All
    	
    	SELECT        
    	   r.[RelationTupleID]
    	  ,r.[ObjectNamespace]
          ,r.[ObjectKey]
          ,r.[ObjectRelation]
          ,r.[SubjectNamespace]
          ,r.[SubjectKey]
          ,r.[SubjectRelation]
    	  ,[HierarchyLevel] + 1 AS [HierarchyLevel]
      FROM 
    	[Identity].[RelationTuple] r, [RelationTuples] cte
      WHERE 
    	cte.[SubjectKey] = r.[ObjectKey] 
    		AND cte.[SubjectNamespace] = r.[ObjectNamespace] 
    		AND cte.[SubjectRelation] = r.[ObjectRelation]
    )
    SELECT @IsAuthorized =
    	CASE
    		WHEN EXISTS(SELECT 1 FROM [RelationTuples] WHERE [SubjectNamespace] = @SubjectNamespace AND [SubjectKey] = @SubjectKey) 
    			THEN 1
    		ELSE 0
    	END;

    RETURN @IsAuthorized;
END
