namespace DomKultury.Models
{
    public class Konkurs
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Opis { get; set; }
        public DateTime Termin { get; set; }
        public string Organizator { get; set; }
        public decimal Cena { get; set; }
        public string ObrazekUrl { get; set; }
    }

}
