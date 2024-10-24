namespace SFA.DAS.Admin.Aan.Application.OuterApi.Locations
{
    public class GetLocationsBySearchApiResponse
    {
        public List<Location> Locations { get; set; } = [];

        public class Location
        {
            public string Name { get; set; }
        }
    }
}
