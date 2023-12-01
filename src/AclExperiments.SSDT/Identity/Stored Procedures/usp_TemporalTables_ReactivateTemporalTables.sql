CREATE PROCEDURE [Identity].[usp_TemporalTables_ReactivateTemporalTables]
AS BEGIN

	IF OBJECTPROPERTY(OBJECT_ID('[Identity].[User]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Identity].[User]'

		ALTER TABLE [Identity].[User] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Identity].[User] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[UserHistory], DATA_CONSISTENCY_CHECK = ON));
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Identity].[RelationTuple]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Identity].[RelationTuple]'

		ALTER TABLE [Identity].[RelationTuple] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Identity].[RelationTuple] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[RelationTupleHistory], DATA_CONSISTENCY_CHECK = ON));
	END
    
END