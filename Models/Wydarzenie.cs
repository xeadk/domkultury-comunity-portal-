using System.ComponentModel.DataAnnotations;

namespace DomKultury.Models
{
    public class Wydarzenie
    {
        public int Id { get; set; }

        [Required]
        public string Nazwa { get; set; }

        // relacja One-to-Many z Kategoria
        public int KategoriaId { get; set; }

        public virtual Kategoria Kategoria { get; set; }

        [Required]
        public string Organizator { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        [MaxLength(200)]
        public string Lokalizacja { get; set; }

        [MaxLength(500)]
        public string Opis { get; set; }

        public bool Status { get; set; } // True - zaplanowane, False - odwołane
    }
}
