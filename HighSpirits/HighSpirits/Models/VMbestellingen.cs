namespace HighSpirits.Models
{
    public class VMbestellingen
    {
        public Bestellijn bestellijn { get; set; }
        public Bestelling bestelling { get; set; }
        public LoginCredentials Klant { get; set; }
        public Totalen totalen { get; set; }
    }
}
