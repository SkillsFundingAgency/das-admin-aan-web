namespace SFA.DAS.Admin.Aan.Web.Extensions;

public static class DateTimeExtensions
{
    public static string ToApiString(this DateOnly date) => date.ToString("yyyy-MM-dd");
    public static string ToApiString(this DateTime date) => date.ToString("yyyy-MM-dd");
    public static string ToScreenString(this DateTime date) => date.ToString("dd/MM/yyyy");

    private readonly static TimeZoneInfo LocalTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
    public static DateTime UtcToLocalTime(this DateTime date) => TimeZoneInfo.ConvertTimeFromUtc(date, LocalTimeZone);

    public static DateTime LocalToUtcTime(int year, int month, int day, int hour, int minutes)
    {
        var dateToBuild = new DateTime(year, month, day, hour, minutes, 0, DateTimeKind.Unspecified);
        var result = TimeZoneInfo.ConvertTimeToUtc(dateToBuild, LocalTimeZone);
        return result;
    }
}

