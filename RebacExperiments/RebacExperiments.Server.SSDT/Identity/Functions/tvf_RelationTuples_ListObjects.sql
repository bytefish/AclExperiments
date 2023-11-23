CREATE FUNCTION [Identity].[tvf_RelationTuples_ListObjects]
(
     @ObjectNamespace NVARCHAR(50) 
    ,@ObjectRelation NVARCHAR(50)
    ,@SubjectNamespace NVARCHAR(50)
    ,@SubjectKey INT
)
RETURNS @returntable TABLE
(

     [RelationTupleID]   INT
    ,[ObjectNamespace]   NVARCHAR(50)
    ,[ObjectKey]         INT
    ,[ObjectRelation]    NVARCHAR(50)
    ,[SubjectNamespace]  NVARCHAR(50)
    ,[SubjectKey]        INT
    ,[SubjectRelation]   NVARCHAR(50)
)
AS
BEGIN

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
		    [SubjectNamespace] = @SubjectNamespace AND [SubjectKey] = @SubjectKey
	  
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
	    cte.[ObjectKey] = r.[SubjectKey] 
		    AND cte.[ObjectNamespace] = r.[SubjectNamespace] 
		    AND cte.[ObjectRelation] = r.[SubjectRelation]
    )
    INSERT 
        @returntable
    SELECT DISTINCT 
	    [RelationTupleID], [ObjectNamespace], [ObjectKey], [ObjectRelation], [SubjectNamespace], [SubjectKey], [SubjectRelation]
    FROM 
	    [RelationTuples] 
    WHERE
	    [ObjectNamespace] = @ObjectNamespace AND [ObjectRelation] = @ObjectRelation;

    RETURN;

END
