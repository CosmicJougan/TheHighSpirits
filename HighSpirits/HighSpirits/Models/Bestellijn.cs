namespace HighSpirits.Models
{
    public class Bestellijn
    {
        public int ProductId { get; set; }
        public int BestellingId { get; set; }
        public double HistorischePrijs { get; set; }
        public int Aantalstuks { get; set; }
    }
}
