CREATE PROCEDURE [dbo].[sp_GetUserDetails]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.Id,
        u.FirstName,
        u.LastName,
        u.Email,
        u.CreatedAt,
        u.LastLoginDate,
        STRING_AGG(r.Name, ',') AS Roles
    FROM Users u
    LEFT JOIN UserRoles ur ON u.Id = ur.UserId
    LEFT JOIN Roles r ON ur.RoleId = r.Id
    WHERE u.Id = @UserId
    GROUP BY 
        u.Id,
        u.FirstName,
        u.LastName,
        u.Email,
        u.CreatedAt,
        u.LastLoginDate;
END 