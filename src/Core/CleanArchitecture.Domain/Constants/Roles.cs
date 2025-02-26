public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";

    public static readonly IReadOnlyDictionary<string, string> All = new Dictionary<string, string>
    {
        { Admin, "Administrator with full access" },
        { User, "Standard user with basic access" }
    };
} 