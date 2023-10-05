using SFA.DAS.Admin.Aan.Application.Constants;
using SFA.DAS.Admin.Aan.Application.Extensions;
using SFA.DAS.Admin.Aan.Web.Models.NetworkEvents;

namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class EventFormatViewModel : ManageEventViewModelBase
{
    public EventFormat? EventFormat { get; set; }

    public List<ChecklistLookup> EventFormats =>
        new()
        {
            new ChecklistLookup(Application.Constants.EventFormat.InPerson.GetDescription(), Application.Constants.EventFormat.InPerson.ToString(), EventFormat==Application.Constants.EventFormat.InPerson),
            new ChecklistLookup(Application.Constants.EventFormat.Online.GetDescription(), Application.Constants.EventFormat.Online.ToString(), EventFormat==Application.Constants.EventFormat.Online),
            new ChecklistLookup(Application.Constants.EventFormat.Hybrid.GetDescription(), Application.Constants.EventFormat.Hybrid.ToString(), EventFormat==Application.Constants.EventFormat.Hybrid)
        };
}