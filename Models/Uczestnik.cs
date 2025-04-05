namespace DomKultury.Models
{
    public class Uczestnik
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Email { get; set; }
        public string NumerTelefonu { get; set; }
        public DateTime DataRejestracji { get; set; }

        // relacja Many-to-Many z Zajeciami
        public ICollection<Zajecie> Zajecia { get; set; }

    }

}
