CREATE TYPE [Identity].[udt_RelationTupleType] AS TABLE 
(
    [RelationTupleID]       INT          
   ,[Namespace]             NVARCHAR(50)
   ,[Object]                NVARCHAR(50)
   ,[Relation]              NVARCHAR(50)
   ,[SubjectNamespace]      NVARCHAR(50)
   ,[Subject]               NVARCHAR(50)
   ,[SubjectRelation]       NVARCHAR(50)
   ,[RowVersion]            VARBINARY(8)  
   ,[LastEditedBy]          INT          
   ,[ValidFrom]             DATETIME2 (7)
   ,[ValidTo]               DATETIME2 (7)
);