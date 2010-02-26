exec aspnet_Roles_CreateRole '/', 'Editor'
exec aspnet_Roles_CreateRole '/', 'System Admin'
exec aspnet_Roles_CreateRole '/', 'Site Admin'
exec aspnet_Roles_CreateRole '/', 'None'
exec aspnet_Roles_CreateRole '/', 'Content Editor'
exec aspnet_Roles_CreateRole '/', 'Article Aditor'

GO
DECLARE	@return_value int

EXEC	@return_value = [dbo].[aspnet_UsersInRoles_AddUsersToRoles]
		@ApplicationName = N'/',
		@UserNames = N'mjjames',
		@RoleNames = N'system admin',
		@CurrentTimeUtc = NULL

SELECT	'Return Value' = @return_value

GO
