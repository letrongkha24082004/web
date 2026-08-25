namespace BlogManager.Security;

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Editor = "Editor";
    public const string User = "User";

    public const string CanEditPosts = Admin + "," + Editor + "," + User;

    public static readonly string[] All = [Admin, Editor, User];
}
