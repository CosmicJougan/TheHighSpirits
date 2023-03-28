namespace HighSpirits.Models
{
    public class VMbestellingen
    {
        public Bestelling bestelling { get; set; }
        public LoginCredentials Klant { get; set; }
        public Winkelmandje winkelmandje { get; set; }
        public Totalen totalen { get; set; }
    }
}
