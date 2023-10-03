﻿namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class EventDescriptionViewModel : EventPageEditFields
{
    public string? EventOutline { get; set; }
    public string? EventSummary { get; set; }

    public int EventOutlineMaxCount => Application.Constants.ManageEventValidation.EventOutlineMaxLength;
    public int EventSummaryMaxCount => Application.Constants.ManageEventValidation.EventSummaryMaxLength;
}