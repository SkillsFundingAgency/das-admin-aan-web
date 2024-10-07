using System.Dynamic;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEventAttendees;
using SFA.DAS.Admin.Aan.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Admin.Aan.Web.UnitTests.Services
{
    public class CsvHelperServiceTests
    {
        [Test, MoqAutoData]
        public void GenerateCsvFileFromModel_ShouldReturnByteArray(
            GetCalendarEventAttendeesResponse source,
            CsvHelperService sut)
        {
            // Arrange
            var expectedRecords = new List<dynamic>
            {
                new ExpandoObject()
            };
            expectedRecords[0].Name = "John Doe";
            expectedRecords[0].Email = "john.doe@example.com";
            expectedRecords[0].SignUpDate = "2023-09-24";

            // Act
            var result = sut.GenerateCsvFileFromModel(source);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<byte[]>();
        }
    }
}
