namespace HighSpirits.Models
{
    public class VMwinkelmandje
    {
        public Totalen totalen { get; set; }
        public LoginCredentials Klant { get; set; }
        
        public WinkelmandjeRepository winkelmandjeRepository { get; set; }
    }
}
