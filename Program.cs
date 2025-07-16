using System;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;//system.ıo'nun import edilme nedeni dosya işlemleri kullanacağımızdan kaynaklanıyor
using System.Text;
using CsvHelper;//terminalden CsvHelper'ı dahil ettik dotnet add package CsvHelper ile bunu yapmamızın sebebi Csv dosyalarını okuma yazma işlemlerini kolaylaştırmasıdır.Obje üzerinden yapar okuma yazma 
using System.Globalization;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;


/*
c# -> Yazılım dilidir-Kod yazmamızı sağlar
.Net -> Yazılım geliştirme platformu-Yazdığımız c# kodlarının çalıştırılmasını ve yönetilmesini sağlar


*/
/*
Şimdi öncelikle biz şehirlerin max min enlem boylamını alıcaz daha sonra bu enlem boylam ile sınırlandırarak istek göndereceğiz Apiye daha sonra bu apiden gelen verileri istediğimiz kullanacağımız kısımları tutucaz



Apiden veri JSON formatında gelecek bizim bu gelen JSON verisini c# nesnesine dönüştürmemiz gerek yani deserialization yapmamız gerek bunu da OpenCageResponse da  yaptık ve nasıl yapılacağını öğrendik json olarak gelen bir verinin nasıl c# nesne yapısına dönüşeceğini öğrenmiş olduk



*/
class Program
{
    public static void SaveLocationsToCsv(List<Saha> sahaList, string filePath)
    {
        bool dosyaVarMi = File.Exists(filePath);
        var allRows = new List<SahaCSVForm>();

        // Saha listelerinden Csv satırlarını çıkar
        foreach (var saha in sahaList)
        {
            allRows.AddRange(saha.ToCsvForm());
        }

        // Append modunda aç (dosya varsa üzerine yazmaz, altına ekler)
        using (var stream = new StreamWriter(filePath, append: true))//append:true eklemeyi sağlar
        using (var csv = new CsvWriter(stream, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = !dosyaVarMi // Sadece ilk yazışta başlık yaz
        }))
        {
            if (!dosyaVarMi)
            {
                csv.WriteHeader<SahaCSVForm>();
                csv.NextRecord(); // bir alt satıra geç
            }

            csv.WriteRecords(allRows); // verileri yaz
        }

    }

    public static async Task<LocationInfo> SendCoordinateToApi(double latitude, double longitude, string apiKey)
    {
        if (apiKey == null)
        {
            return null;
        }
        else
        {
            //
            string url = $"https://api.geoapify.com/v1/geocode/reverse?lat={latitude}&lon={longitude}&apiKey={apiKey}";
            // Console.WriteLine(url);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        Root responseOpen = JsonConvert.DeserializeObject<Root>(json);
                        if (responseOpen != null)
                        {



                            LocationInfo locationInfo = LocationInfo.FromApiResponse(responseOpen);

                            locationInfo.latitude = latitude;
                            locationInfo.longitude = longitude;
                            return locationInfo;
                        }


                        // var city = responseOpen.Results[0].Components.City;


                        else
                        {
                            return null;
                        }

                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }

    public static bool IsCoordinateAlreadySaved(double lat, double lng, List<LocationInfo> savedLocations)//burada parametreler latitude random ürettiğimiz enlem boylam ve savedLocation da kaydettiğimiz şehirlerin bir listesi bu liste üzerinde bu koordinatları karşılaştırıcaz aynı ise listeye almayacağız aynı ise true döner 
    {

        return savedLocations.Any(location => location.latitude == lat && location.longitude == lng);
    }

    // public static string GetCityFromApiResponse(Root response,City city)
    // {
    //     if(response == null)
    //     return response.features[0].properties.city ?? "";
    // }

    public static bool IsCoordinateInCity(City city, double latitude, double longitude)
    {
        return latitude >= city.MinLat && latitude <= city.MaxLat && longitude >= city.MinLng && longitude <= city.MaxLng;
    }
    public static (double Latitude, double Longitude) GenerateRandomCoordinate(City city, Random random) //burada kullanılan (double latitude,double longitude diye belirtilen yapının ismi tuple olarak geçer eğer iki değişkeni return etmek istiyorsak ve bunun için bir sınıf kullanmak istemiyorsak yaparız)
    {
        double numLat = random.NextDouble() * (city.MaxLat - city.MinLat) + city.MinLat;
        double numLng = random.NextDouble() * (city.MaxLng - city.MinLng) + city.MinLng;
        return (numLat, numLng);

    }
    static async Task Main(string[] args)
    {


        City batman = new City("Batman", 37.68, 37.90, 41.06, 41.40);
        City siirt = new City("Siirt", 37.82, 38.22, 41.86, 42.20);
        City diyarbakır = new City("Diyarbakır", 37.45, 38.45, 39.25, 41.00);
        City adıyaman = new City("Adıyaman", 37.40, 38.10, 37.90, 39.00);
        City sırnak = new City("Şırnak", 37.10, 38.00, 41.10, 43.00);



        List<LocationInfo> savedLocationBatman = await GetUniqueLocations(batman, 60);//her şey tamam 
        List<LocationInfo> savedLocationSiirt = await GetUniqueLocations(siirt, 60);//tamam
        List<LocationInfo> savedLocationDiyarbakır = await GetUniqueLocations(diyarbakır, 60);//Tamam
        List<LocationInfo> savedLocationAdıyaman = await GetUniqueLocations(adıyaman, 60);//tamam 
        List<LocationInfo> savedLocationSırnak = await GetUniqueLocations(sırnak, 60);//tamam 

















    }
    public static async Task<List<LocationInfo>> GetUniqueLocations(City city, int targetCount)
    {
        List<LocationInfo> savedLocations = new List<LocationInfo>();
        //hızlı bi kontrol etmek için hashset oluşturduk
        HashSet<string> uniqueLocationKeys = new HashSet<string>();
        //burada random bir sayı oluşturucaz belli bir aralıkta çünkü mesela siirt ilindeki koordinatların içinde kalıp istek atıcaz farklı farklı
        Random random = new Random();

        string apiKey = "31c20dd1b7e04912bc4ab253be98f8e9";
        List<Saha> sahaList = new List<Saha>();
        while (savedLocations.Count < targetCount)
        {
            var coord = GenerateRandomCoordinate(city, random);
            if (IsCoordinateAlreadySaved(coord.Latitude, coord.Longitude, savedLocations)) continue;//Eğer listenin içerisinde bu koordinatlarla eşleşen varsa while da 1 artır geç


            LocationInfo location = await SendCoordinateToApi(coord.Latitude, coord.Longitude, apiKey);
            if (location == null) continue;
            if (city.CityName != location.county) continue;
            string specificSahaAdi = GetSpecificLocation(location);
            if (string.IsNullOrWhiteSpace(specificSahaAdi)) continue;

            // string uniqueKey = $"{location.county}_{location.mostSpecificInfo}"; //bir key tasarlıyoruz kendimize göre bu aynı olursa aynı yeri almış olucak zaten buna göre karşılaştırıcaz aynısı varsa eklemeyeceğiz yoksa savedLocation list'ine ekleyeceğiz
            if (uniqueLocationKeys.Contains(location.mostSpecificInfo)) continue;
            uniqueLocationKeys.Add(location.mostSpecificInfo);

            // if (uniqueLocationKeys.Contains(uniqueKey)) continue;
            // uniqueLocationKeys.Add(uniqueKey); ;//burada hashsete atıyoruz uniqueKey i

            savedLocations.Add(location); //burada da savedLocation'a atıyorz
            Saha saha = new Saha(specificSahaAdi, 10, 5);
            saha.Latitude = coord.Latitude;
            saha.Longitude = coord.Longitude;
            saha.City = location.city ?? location.county;

            sahaList.Add(saha);
            SaveLocationsToCsv(sahaList, "C:\\Users\\enesg\\OneDrive\\Masaüstü\\tpao proje\\output.csv");

            // saha.PrintStructure();



            // Console.WriteLine($"{savedLocations.Count}) {location.mostSpecificInfo}");

        }
        // DeleteFromToDatabase();

        SaveLocationsToDatabase(sahaList);
        Console.WriteLine("işlem tamamlandı");

        return savedLocations;

    }
    public static void SaveLocationsToDatabase(List<Saha> sahaList)
    {
        using (var db = new SahaContext())
        {

            db.Sahalar.AddRange(sahaList);

            db.SaveChanges();

        }


    }
    public static void DeleteFromToDatabase()
    {

        using (var db = new SahaContext())
        {
            db.Sahalar.RemoveRange(db.Sahalar);
            db.SaveChanges();
            db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Sahalar', RESEED, 0)");
            db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Kuyular', RESEED, 0)");
            db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Wellbores', RESEED, 0)");
        }

    }


    public static string GetSpecificLocation(LocationInfo location)
    {

        if (location == null) return null;

        if (!string.IsNullOrEmpty(location.village))
        {
            location.mostSpecificInfo = location.village;
            return location.village;
        }
        if (!string.IsNullOrEmpty(location.suburb))
        {
            location.mostSpecificInfo = location.suburb;
            return location.suburb;
        }
        if (!string.IsNullOrEmpty(location.district))
        {
            location.mostSpecificInfo = location.district;
            return location.district;
        }
        if (!string.IsNullOrEmpty(location.county))
        {
            location.mostSpecificInfo = location.county;
            return location.county;
        }
        if (!string.IsNullOrEmpty(location.city))
        {
            location.mostSpecificInfo = location.city;
            return location.city;
        }

        location.mostSpecificInfo = "Konum bulunamadı";
        return "Konum bulunamadı";
    }
}

