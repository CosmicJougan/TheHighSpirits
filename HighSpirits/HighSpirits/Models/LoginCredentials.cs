using System.ComponentModel.DataAnnotations;
namespace HighSpirits.Models
{
    public class LoginCredentials
    {
        public int KlantId { get; set; }

        [Required (ErrorMessage = "Verplicht veld!")]
        public string Gebruikersnaam { get; set; }

        [Required(ErrorMessage = "Verplicht veld!")]
        public string Wachtwoord { get; set; }
        
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
