public class Kuyu
{
    public int KuyuId { get; set; }
    public string kuyuAdı { get; set; }
    public List<Wellbore> wellbores { get; set; }
    //şimdi burada ilişikiyi kurmamız gerek Saha ile Kuyu arasında one-to-many ilişkisi var yani bir sahanın birden fazla kuyusu olabilir ama kuyunun bir sahası olur bu yüzden mesela Saha classına bir liste olarak sahip olduğu kuyuların listesini vereceğiz Kuyuya ise ait olduğu Sahanın Id'sini ve objesini database'den çekeceğiz 
    public Saha Saha { get; set; }//navigationProperty
    public int? SahaId { get; set; }//foreign key


    public Kuyu() { }
    public Kuyu(string sahaAdı, int index, int wellboreCount)
    {
        if (index == 0)
        {
            this.kuyuAdı = $"{sahaAdı}";
        }
        else
        {
            this.kuyuAdı = $"{sahaAdı}-{index}";
        }


        wellbores = new List<Wellbore>();

        for (int i = 0; i < wellboreCount; i++)
        {

            string wellboreName;
            if (i == 0) { wellboreName = $"{kuyuAdı}"; }
            else if (i == 1) { wellboreName = $"{kuyuAdı}/M"; }
            else wellboreName = $"{kuyuAdı}/M{i - 1}";
            var wellbore = new Wellbore(wellboreName);
            wellbore.Kuyu = this;
            wellbores.Add(wellbore);
        }


    }

}