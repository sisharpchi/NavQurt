namespace NavQurt.Shared;

public static class CacheKeys
{
    public static string OrganizationSetting(int organizationId) => $"organization:setting:{organizationId}";
    public static string CompanyAllTgPermissions(string companyToken) => $"permission:tg:{companyToken}";
}
