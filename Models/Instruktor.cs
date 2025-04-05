namespace DomKultury.Models
{
    public class Instruktor
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Email { get; set; }

        // Relacja One-to-Many z Zajęciami
        public virtual ICollection<Zajecie>? Zajecia { get; set; }
    }
}
