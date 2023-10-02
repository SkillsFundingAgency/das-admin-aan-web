﻿using SFA.DAS.Admin.Aan.Web.Infrastructure;

namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

public class OrganiserDetailsViewModel : IEventPageEditFields
{
    public string? OrganiserName { get; set; }
    public string? OrganiserEmail { get; set; }
    public string? PageTitle { get; set; }
    public string? PostLink { get; set; }
    public string? CancelLink { get; set; }
}