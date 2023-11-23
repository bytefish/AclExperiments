CREATE TABLE [Identity].[Role](
    [RoleID]                INT                                         CONSTRAINT [DF_Identity_Role_RoleID] DEFAULT (NEXT VALUE FOR [Identity].[sq_Role]) NOT NULL,
    [Name]                  NVARCHAR(255)                               NOT NULL,
    [Description]           NVARCHAR(2000)                              NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY ([RoleID]),
    CONSTRAINT [FK_Role_LastEditedBy_User_UserID] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[RoleHistory]));