namespace DomKultury.Models
{
    public class Zajecie
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Opis { get; set; }
        public DateTime Termin { get; set; }
        public string Lokalizacja { get; set; }
        public decimal Cena { get; set; }
        public int MaksymalnaLiczbaUczestnikow { get; set; }

        // Relacja One-to-Many z Instruktorem
        public int InstruktorId { get; set; } // Klucz obcy do Instruktora
        public virtual Instruktor Instruktor { get; set; } // Nawigacja do Instruktora

        // Relacja One-to-Many z Uczestnikami
        public ICollection<Uczestnik> Uczestnicy { get; set; } = new List<Uczestnik>();
        public string? ObrazekUrl { get; set; } // Ścieżka lub URL do obrazka

    }

}
