using CsvHelper;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEventAttendees;
using System.Dynamic;
using System.Globalization;

namespace SFA.DAS.Admin.Aan.Web.Services
{
    public interface ICsvHelperService
    {
        byte[] GenerateCsvFileFromModel(GetCalendarEventAttendeesResponse source);
    }

    public class CsvHelperService : ICsvHelperService
    {
        public byte[] GenerateCsvFileFromModel(GetCalendarEventAttendeesResponse source)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, new CultureInfo("en-GB"));

            var records = SetUpColumnsOnCsv(source);
            csv.WriteRecords(records);

            csv.Flush();

            return memoryStream.ToArray();
        }

        private static IEnumerable<dynamic> SetUpColumnsOnCsv(GetCalendarEventAttendeesResponse model)
        {
            var listOfRecords = new List<dynamic>();

            foreach (var attendee in model.Attendees)
            {
                dynamic record = new ExpandoObject();

                AddProperty(record, "Name", attendee.Name);
                AddProperty(record, "Email", attendee.Email);
                AddProperty(record, "Sign-up date", attendee.SignUpDate.ToString("yyy-MM-dd"));

                listOfRecords.Add(record);
            }

            return listOfRecords;
        }

        private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var expandoDict = expando as IDictionary<string, object>;
            expandoDict[propertyName] = propertyValue;
        }
    }
}
