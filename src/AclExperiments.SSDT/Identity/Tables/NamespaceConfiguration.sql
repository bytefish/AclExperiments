CREATE TABLE [Identity].[NamespaceConfiguration](
    [NamespaceConfigurationID]      INT                                         CONSTRAINT [DF_Identity_NamespaceConfiguration_NamespaceConfigurationID] DEFAULT (NEXT VALUE FOR [Identity].[sq_NamespaceConfiguration]) NOT NULL,
    [Name]                          NVARCHAR(255)                               NOT NULL,
    [Version]                       INT                                         NOT NULL,
    [Content]                       NVARCHAR(255)                               NOT NULL,
    [RowVersion]                    ROWVERSION                                  NULL,
    [LastEditedBy]                  INT                                         NOT NULL,
    [ValidFrom]                     DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]                       DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_NamespaceConfiguration] PRIMARY KEY ([NamespaceConfigurationID]),
    CONSTRAINT [FK_NamespaceConfiguration_LastEditedBy_User_UserID] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[NamespaceConfigurationHistory]));