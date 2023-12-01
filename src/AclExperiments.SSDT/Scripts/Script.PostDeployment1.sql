/*
Post-Deployment Script Template                            
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.        
 Use SQLCMD syntax to include a file in the post-deployment script.            
 Example:      :r .\myfile.sql                                
 Use SQLCMD syntax to reference a variable in the post-deployment script.        
 Example:      :setvar TableName MyTable                            
               SELECT * FROM [$(TableName)]                    
--------------------------------------------------------------------------------------
*/

/*
  We need to deactivate all Temporal Tables before the initial data load.
*/

EXEC [Identity].[usp_TemporalTables_DeactivateTemporalTables]
GO

EXEC [Application].[usp_TemporalTables_DeactivateTemporalTables]
GO

/* 
    Set the initial data for the [Identity] schema
*/
:r .\Identity\pds-100-ins-identity-users.sql
GO

:r .\Identity\pds-110-ins-identity-roles.sql
GO

:r .\Identity\pds-120-ins-identity-relation-tuples.sql
GO

/* 
    Set the initial data for the [Application] schema
*/
:r .\Application\pds-100-ins-application-task-priority.sql
GO

:r .\Application\pds-110-ins-application-task-status.sql
GO

:r .\Application\pds-120-ins-application-user-task.sql
GO

:r .\Application\pds-130-ins-application-organization.sql
GO

:r .\Application\pds-140-ins-application-team.sql
GO

/*
  We need to reactivate all Temporal Tables after the initial data load.
*/
EXEC [Identity].[usp_TemporalTables_ReactivateTemporalTables]
GO

EXEC [Application].[usp_TemporalTables_ReactivateTemporalTables]
GO

