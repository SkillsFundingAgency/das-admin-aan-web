using SFA.DAS.Aan.SharedUi.Models.PublicProfile;

namespace SFA.DAS.Admin.Aan.Web.Models;

public class AmbassadorProfileViewModel
{
    public ConnectViaLinkedInViewModel ConnectViaLinkedIn { get; set; } = new ConnectViaLinkedInViewModel();


    public MemberInformationSectionViewModel MemberInformation { get; set; } = new MemberInformationSectionViewModel();


    public ApprenticeshipSectionViewModel ApprenticeshipInformation { get; set; } = new ApprenticeshipSectionViewModel();


    public AreasOfInterestSectionViewModel AreasOfInterest { get; set; } = new AreasOfInterestSectionViewModel();
}
