﻿namespace SFA.DAS.Admin.Aan.Web.Models.ManageEvent;

public class EventTypeViewModel : ManageEventViewModelBase
{
    public string? EventTitle { get; set; }
    public int? EventTypeId { get; set; }
    public int? EventRegionId { get; set; }

    public List<RegionSelection> EventRegions { get; set; } = [];
    public List<EventTypeSelection> EventTypes { get; set; } = [];
}

public class RegionSelection
{
    public int RegionId { get; set; }
    public string Name { get; set; }

    public RegionSelection(string name, int id)
    {
        Name = name;
        RegionId = id;
    }
}

public class EventTypeSelection
{
    public int EventTypeId { get; set; }
    public string Name { get; set; }

    public EventTypeSelection(string name, int id)
    {
        Name = name;
        EventTypeId = id;
    }
}