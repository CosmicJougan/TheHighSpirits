using System.ComponentModel.DataAnnotations;

namespace HighSpirits.Models
{
    public class Registratie
    {

        public int KlantId { get; set; }

        public string Gebruikersnaam { get; set; }

        public string Wachtwoord { get; set; }

        public string BevestigWW { get; set; }

        public string Naam { get; set; }

        public string Voornaam { get; set; }

        public string Email { get; set; }

        public string Adress { get; set; }

        public string Plaats { get; set; }

        public string PostCode { get; set; }

        public DateTime Geboortedatum { get; set; }

        public int Telefoonnummer { get; set; }
    }
}
