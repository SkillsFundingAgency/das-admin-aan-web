using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.Admin.Aan.Application.Extensions;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class EventFormatViewModel : ManageEventViewModelBase
{
    public EventFormat? EventFormat { get; set; }

    public List<ChecklistLookup> EventFormats =>
        new()
        {
            new ChecklistLookup(DAS.Aan.SharedUi.Constants.EventFormat.InPerson.GetDescription(), DAS.Aan.SharedUi.Constants.EventFormat.InPerson.ToString(), EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.InPerson),
            new ChecklistLookup(DAS.Aan.SharedUi.Constants.EventFormat.Online.GetDescription(), DAS.Aan.SharedUi.Constants.EventFormat.Online.ToString(), EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.Online),
            new ChecklistLookup(DAS.Aan.SharedUi.Constants.EventFormat.Hybrid.GetDescription(), DAS.Aan.SharedUi.Constants.EventFormat.Hybrid.ToString(), EventFormat == DAS.Aan.SharedUi.Constants.EventFormat.Hybrid)
        };
}