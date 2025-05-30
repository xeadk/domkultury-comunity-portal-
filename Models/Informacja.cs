using System.ComponentModel.DataAnnotations;

namespace DomKultury.Models
{
    public class Informacja
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Tytul { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Tresc { get; set; }
        public string Ikona { get; set; } // np. nazwa klasy FontAwesome, np. "fa-users"
    }

}
