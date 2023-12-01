CREATE TABLE [Application].[Team](
    [TeamID]                INT                                         CONSTRAINT [DF_Application_Team_TeamID] DEFAULT (NEXT VALUE FOR [Application].[sq_Team]) NOT NULL,
    [Name]                  NVARCHAR(255)                               NOT NULL,
    [Description]           NVARCHAR(2000)                              NOT NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_Team] PRIMARY KEY ([TeamID]),
    CONSTRAINT [FK_Team_User_LastEditedBy] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[TeamHistory]));