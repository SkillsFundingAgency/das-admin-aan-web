namespace SFA.DAS.Admin.Aan.Application.OuterApi.Members;

public record GetMembersResponse(int Page, int PageSize, int TotalPages, int TotalCount, IEnumerable<MemberSummary> Members);
