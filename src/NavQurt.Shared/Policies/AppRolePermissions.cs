namespace NavQurt.Shared.Policies;

public static class AppRolePermissions
{
    public static IEnumerable<string> SuperAdmin => [Permissions.SuperAdmin];
    public static IEnumerable<string> OrganizationAdmin => [];
    public static IEnumerable<string> CompanyAdmin => [];
    public static IEnumerable<string> Admin => [];
}
