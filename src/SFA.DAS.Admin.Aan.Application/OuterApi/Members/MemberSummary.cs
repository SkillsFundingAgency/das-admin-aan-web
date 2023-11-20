using SFA.DAS.Aan.SharedUi.Models.AmbassadorProfile;

namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members;

public record MemberSummary(Guid MemberId, string FullName, int? RegionId, string? RegionName, MemberUserType UserType, bool IsRegionalChair, DateTime JoinedDate);
