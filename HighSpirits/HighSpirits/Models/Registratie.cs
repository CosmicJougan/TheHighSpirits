using System.ComponentModel.DataAnnotations;

namespace HighSpirits.Models
{
    public class Registratie
    {

        public int KlantId { get; set; }

        [Required(ErrorMessage = "Verplicht veld!")]
        public string Gebruikersnaam { get; set; }


        [Required(ErrorMessage = "Verplicht veld!")]
        
        public string Wachtwoord { get; set; }

        
        [Required(ErrorMessage ="Verplicht veld!")]
        public string BevestigWW { get; set; }


        [Required(ErrorMessage = "Verplicht veld!")]
        public string Naam { get; set; }


        [Required(ErrorMessage = "Verplicht veld!")]
        public string Voornaam { get; set; }


        [Required(ErrorMessage = "Verplicht veld!")]
        [EmailAddress(ErrorMessage ="Geef een valid e-mail adres in!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Verplicht veld!")]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Verplicht veld!")]
        public string Plaats { get; set; }

        [Required(ErrorMessage = "Verplicht veld!")]
        public string PostCode { get; set; }

        [Required(ErrorMessage = "Verplicht veld!")]
        public DateTime? Geboortedatum { get; set; }

        [Required(ErrorMessage = "Verplicht veld!")]
        public int? Telefoonnummer { get; set; }
    }
}
