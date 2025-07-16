using System.ComponentModel.DataAnnotations;

public class Wellbore
{
    public int WellboreId { get; set; }
    [Required]
    [MaxLength(100)]
    public string wellboreName { get; set; }
    public int KuyuId { get; set; }
    public Kuyu Kuyu { get; set; }
    public Wellbore() { }
    public Wellbore(string wellboreName)
    {
        this.wellboreName = wellboreName;
    }


}