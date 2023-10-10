using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Admin.Aan.Application.OuterApi.CalendarEvents;

namespace SFA.DAS.Admin.Aan.Application.UnitTests.OuterApi.Models;

public class GuestTests
{
    [Test, AutoData]
    public void Guest_ConstructorTest(string name, string jobTitle)
    {
        var guest = new Guest(name, jobTitle);
        guest.GuestName.Should().Be(name);
        guest.GuestJobTitle.Should().Be(jobTitle);
    }
}