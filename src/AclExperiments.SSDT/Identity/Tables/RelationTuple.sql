CREATE TABLE [Identity].[RelationTuple](
    [RelationTupleID]       INT                                         CONSTRAINT [DF_Identity_RelationTuple_RelationTupleID] DEFAULT (NEXT VALUE FOR [Identity].[sq_RelationTuple]) NOT NULL,
    [Namespace]             NVARCHAR(255)                               NOT NULL,
    [Object]                NVARCHAR(255)                               NOT NULL,
    [Relation]              NVARCHAR(255)                               NOT NULL,
    [Subject]               NVARCHAR(255)                               NOT NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_RelationTuple] PRIMARY KEY ([RelationTupleID]),
    CONSTRAINT [FK_RelationTuple_LastEditedBy_User_UserID] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[RelationTupleHistory]));