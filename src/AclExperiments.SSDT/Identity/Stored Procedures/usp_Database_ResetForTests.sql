CREATE PROCEDURE [Identity].[usp_Database_ResetForTests]
AS BEGIN
    
    SET NOCOUNT ON;

    EXECUTE [Identity].[usp_TemporalTables_DeactivateTemporalTables];

    EXEC(N'DELETE FROM [Identity].[RelationTuple]');
    EXEC(N'DELETE FROM [Identity].[NamespaceConfiguration]');

    EXECUTE [Identity].[usp_TemporalTables_ReactivateTemporalTables];

END