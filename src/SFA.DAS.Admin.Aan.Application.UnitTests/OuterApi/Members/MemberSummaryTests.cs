using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Aan.SharedUi.Models.NetworkDirectory;
using SFA.DAS.Admin.Aan.Application.OuterApi.Members;

namespace SFA.DAS.Admin.Aan.Application.UnitTests.OuterApi.Members;

public class MemberSummaryTests
{
    [Test, AutoData]
    public void Operator_ConvertsTo_MembersViewModel(MemberSummary sut)
    {
        MembersViewModel model = sut;
        sut.Should().BeEquivalentTo(model, options => options.ExcludingMissingMembers());
    }

    [Test]
    [InlineAutoData(true)]
    [InlineAutoData(false)]
    public void Operator_DependingOnMemberIsRegionalChair_PopulatesUserRole(bool isRegionalChair, MemberSummary sut)
    {
        sut = sut with { IsRegionalChair = isRegionalChair };
        MembersViewModel model = sut;
        if (isRegionalChair)
        {
            model.UserRole.Should().Be(Role.RegionalChair);
        }
        else
        {
            model.UserRole.ToString().Should().Be(sut.UserType.ToString());
        }
    }

    [Test]
    [InlineAutoData(null, "")]
    [InlineAutoData(1, "regionName")]
    public void Operator_RegionalIdIsNull_PopulatesUserRole(int? regionId, string regionName, MemberSummary sut)
    {
        sut = sut with { RegionId = regionId, RegionName = regionName };
        MembersViewModel model = sut;
        if (regionId == null)
        {
            model.RegionName.Should().Be(MemberSummary.MultiRegional);
        }
        else
        {
            model.RegionName.Should().Be(sut.RegionName);
        }
    }
}
