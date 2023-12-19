using SFA.DAS.Aan.SharedUi.Models.PublicProfile;

namespace SFA.DAS.Admin.Aan.Web.Models;

public class AmbassadorProfileViewModel
{
    public ContactInformationSectionViewModel ContactInformation { get; set; } = new();


    public MemberInformationSectionViewModel MemberInformation { get; set; } = new();


    public ApprenticeshipSectionViewModel ApprenticeshipInformation { get; set; } = new();


    public AreasOfInterestSectionViewModel AreasOfInterest { get; set; } = new();
}
