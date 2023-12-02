CREATE PROCEDURE [Identity].[usp_RelationTuple_GetRowCount]
    @Namespace  NVARCHAR(50)
   ,@Object     NVARCHAR(50)
   ,@Relation   NVARCHAR(50)
   ,@Subject    NVARCHAR(50)
   ,@RowCount   INT OUTPUT
AS BEGIN

    SET NOCOUNT ON;

    SET @RowCount = (SELECT 
        COUNT(*)
    FROM
        [Identity].[RelationTuple]
    WHERE
        [Namespace] = @Namespace AND [Object] = @Object AND [Relation] = @Relation AND [Subject] = @Subject);

END