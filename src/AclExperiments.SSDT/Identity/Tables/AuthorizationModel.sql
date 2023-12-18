CREATE TABLE [Identity].[AuthorizationModel](
    [AuthorizationModelID]          INT                                         CONSTRAINT [DF_Identity_AuthorizationModel_AuthorizationModelID] DEFAULT (NEXT VALUE FOR [Identity].[sq_AuthorizationModel]) NOT NULL,
    [ModelKey]                      VARCHAR(255)                                NOT NULL,
    [Name]                          NVARCHAR(255)                               NOT NULL,
    [Description]                   NVARCHAR(1000)                              NOT NULL,
    [Content]                       NVARCHAR(MAX)                               NOT NULL,
    [RowVersion]                    ROWVERSION                                  NULL,
    [LastEditedBy]                  INT                                         NOT NULL,
    [ValidFrom]                     DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]                       DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_AuthorizationModel] PRIMARY KEY ([AuthorizationModelID]),
    CONSTRAINT [FK_AuthorizationModel_LastEditedBy_User_UserID] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[AuthorizationModelHistory]));