namespace SFA.DAS.Admin.Aan.Web.Models;

public record AdminHubViewModel(bool HasManageEventsRole, bool HasManageMembersRole, string ManageEventsUrl, string ManageAmbassadorsUrl);
