using System.ComponentModel.DataAnnotations;

namespace HighSpirits.Models
{
    public class Winkelmandje
    {
        public int KlantId { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Verplicht veld")]
        public int Aantalstuks { get; set; }
    }
}
