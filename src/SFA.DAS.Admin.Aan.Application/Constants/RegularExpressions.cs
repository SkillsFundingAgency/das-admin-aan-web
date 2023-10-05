namespace SFA.DAS.Admin.Aan.Application.Constants;
public static class RegularExpressions
{
    public const string ExcludedCharactersRegex = @"^[^@#$^=+\\\/<>%]*$";
    public const string EmailRegex = @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z0-9_](-?[a-zA-Z0-9_])*(\.[a-zA-Z0-9](-?[a-zA-Z0-9])*)+$";
    public const string UrlRegex = "^https://(?:www\\.)?[a-zA-Z0-9-]+\\.[a-zA-Z]{2,}(/\\S*)?$";
}
