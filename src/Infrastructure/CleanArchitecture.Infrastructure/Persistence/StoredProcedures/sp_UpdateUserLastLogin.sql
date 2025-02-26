CREATE PROCEDURE [dbo].[sp_UpdateUserLastLogin]
    @UserId UNIQUEIDENTIFIER,
    @LastLoginDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET LastLoginDate = @LastLoginDate
    WHERE Id = @UserId;

    RETURN @@ROWCOUNT;
END 