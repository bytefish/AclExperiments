﻿CREATE PROCEDURE [Identity].[usp_TemporalTables_DeactivateTemporalTables]
AS BEGIN
	IF OBJECTPROPERTY(OBJECT_ID('[Identity].[User]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Identity].[User]'

		ALTER TABLE [Identity].[User] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Identity].[User] DROP PERIOD FOR SYSTEM_TIME;
	END

    IF OBJECTPROPERTY(OBJECT_ID('[Identity].[AuthorizationModel]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Identity].[[AuthorizationModel]'

		ALTER TABLE [Identity].[AuthorizationModel] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Identity].[AuthorizationModel] DROP PERIOD FOR SYSTEM_TIME;
	END


	IF OBJECTPROPERTY(OBJECT_ID('[Identity].[RelationTuple]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Identity].[RelationTuple]'

		ALTER TABLE [Identity].[RelationTuple] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Identity].[RelationTuple] DROP PERIOD FOR SYSTEM_TIME;
	END

END