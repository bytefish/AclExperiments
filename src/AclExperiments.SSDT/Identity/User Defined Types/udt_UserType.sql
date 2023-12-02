CREATE TYPE [Identity].[udt_UserType] AS TABLE
(
    [UserID]                INT,
    [FullName]              NVARCHAR(50),
    [PreferredName]         NVARCHAR(50),
    [IsPermittedToLogon]    BIT,
    [LogonName]             NVARCHAR (256),
    [HashedPassword]        NVARCHAR (MAX),
    [RowVersion]            VARBINARY(8),
    [LastEditedBy]          INT,
    [ValidFrom]             DATETIME2 (7),
    [ValidTo]               DATETIME2 (7)
);