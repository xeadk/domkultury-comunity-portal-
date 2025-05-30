namespace DomKultury.Models
{
    public class DowiedzSieWiecejViewModel
    {
        public List<Informacja> Informacje { get; set; } = new List<Informacja>();
        public List<Faq> Faq { get; set; } = new List<Faq>(); // <- ważne: inicjalizacja lub ustawianie w kontrolerze
    }


}
