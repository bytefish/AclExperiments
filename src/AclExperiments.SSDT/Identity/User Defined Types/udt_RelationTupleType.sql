CREATE TYPE [Identity].[udt_RelationTupleType] AS TABLE 
(
    [RelationTupleID]       INT          
   ,[Namespace]             NVARCHAR(50)
   ,[Object]                NVARCHAR(50)
   ,[Relation]              NVARCHAR(50)
   ,[Subject]               NVARCHAR(50)
   ,[RowVersion]            ROWVERSION   
   ,[LastEditedBy]          INT          
   ,[ValidFrom]             DATETIME2 (7)
   ,[ValidTo]               DATETIME2 (7)
);