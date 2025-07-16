using Microsoft.VisualBasic;

public class LocationInfo
{
    public string city { get; set; }
    public string county { get; set; }
    public string district { get; set; }
    public string suburb { get; set; }
    public string village { get; set; }

    public double latitude { get; set; }

    public double longitude { get; set; }

    public string mostSpecificInfo { get; set; }

    public static LocationInfo FromApiResponse(Root response)
    {
        var prop = response.features[0].properties;
        return new LocationInfo
        {
            county = prop.county,
            city = prop.city,
            district = prop.district,
            suburb = prop.suburb,
            village = prop.village,
            latitude = response.query.lat,
            longitude = response.query.lon,

        };

    }
}