namespace DomKultury.Models
{
    public class Kategoria
    {
        public int Id { get; set; }
        public string Nazwa { get; set; } // np. "Muzyka", "Teatr", "Sport" itd.

        // Relacja One-to-Many z Wydarzeniami
        public virtual ICollection<Wydarzenie>? Wydarzenia { get; set; }
    }
}
