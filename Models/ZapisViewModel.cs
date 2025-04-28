using System.ComponentModel.DataAnnotations;

namespace DomKultury.Models
{
    public class ZapisViewModel
    {
        [Required]
        public int ZajecieId { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane.")]
        public string Imie { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        public string Nazwisko { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy adres email.")]
        public string Email { get; set; }
    }
}
