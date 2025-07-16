using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Saha
{
    public int SahaId { get; set; }
    [Required]
    [MaxLength(100)]
    public string sahaAdı { get; set; }
    public List<Kuyu> kuyuList { get; set; } = new();//newliyoruz çünkü bu sefer EF görmüyor

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string City { get; set; }


    public Saha(string sahaAdı)//bu Constructor Entity Framework için bunu yaptık çünkü EF wellcount veya perWell'i bilmesine gerek yok onların database ile alakası yok
    {
        this.sahaAdı = sahaAdı;
    }
    public Saha(string sahaAdı, int wellCount, int wellboreCountPerWell) : this(sahaAdı)
    {

        this.sahaAdı = sahaAdı;
        // kuyuList = new List<Kuyu>();//her sahanın kendi birden fazla kuyusu olabilir

        for (int i = 1; i <= wellCount; i++)
        {
            var kuyu = new Kuyu(sahaAdı, i, wellboreCountPerWell);
            kuyu.Saha = this;
            kuyuList.Add(kuyu);
        }
    }

    public List<SahaCSVForm> ToCsvForm() //burada SahaCSVForm aslında herbir satırı temsil ediyor sonra bundan liste yapıp bütün listeyi csv ye kaydedicez rows dediğimiz şey tüm rowları listede tutuyor
    {
        List<SahaCSVForm> rows = new List<SahaCSVForm>();
        foreach (var kuyu in kuyuList)
        {
            foreach (var wellbore in kuyu.wellbores)
            {
                rows.Add(new SahaCSVForm
                {
                    Saha = this.sahaAdı,
                    Kuyu = kuyu.kuyuAdı,
                    Wellbore = wellbore.wellboreName
                });
            }
        }

        return rows;
    }

    public void PrintStructure()
    {
        Console.WriteLine($"Saha: {sahaAdı}");
        foreach (var well in kuyuList)
        {
            Console.WriteLine($"  Kuyu: {well.kuyuAdı}");
            foreach (var wb in well.wellbores)
            {
                Console.WriteLine($"    Wellbore: {wb.wellboreName}");
            }
        }
    }


}