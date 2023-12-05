CREATE TABLE [Identity].[RelationTuple](
    [RelationTupleID]       INT                                         CONSTRAINT [DF_Identity_RelationTuple_RelationTupleID] DEFAULT (NEXT VALUE FOR [Identity].[sq_RelationTuple]) NOT NULL,
    [Namespace]             NVARCHAR(50)                                NOT NULL,
    [Object]                NVARCHAR(50)                                NOT NULL,
    [Relation]              NVARCHAR(50)                                NOT NULL,
    [SubjectNamespace]      NVARCHAR(50)                                NOT NULL,
    [Subject]               NVARCHAR(50)                                NOT NULL,
    [SubjectRelation]       NVARCHAR(50)                                NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_RelationTuple] PRIMARY KEY ([RelationTupleID]),
    CONSTRAINT [FK_RelationTuple_LastEditedBy_User_UserID] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[RelationTupleHistory]));