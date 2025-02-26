CREATE PROCEDURE [dbo].[sp_GetUsersWithRoles]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.Id,
        CONCAT(u.FirstName, ' ', u.LastName) AS FullName,
        u.Email,
        STRING_AGG(r.Name, ',') AS Roles
    FROM Users u
    LEFT JOIN UserRoles ur ON u.Id = ur.UserId
    LEFT JOIN Roles r ON ur.RoleId = r.Id
    GROUP BY 
        u.Id,
        u.FirstName,
        u.LastName,
        u.Email
    ORDER BY u.FirstName, u.LastName;
END 