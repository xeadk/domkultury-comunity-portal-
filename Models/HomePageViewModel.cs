namespace DomKultury.Models
{
    public class HomePageViewModel
    {
        public List<ZajecieWidok> Zajecia { get; set; }
        public List<WydarzenieWidok> Wydarzenia { get; set; }
        public WeatherInfo Pogoda { get; set; }

    }

    public class ZajecieWidok
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string OpisKrotki { get; set; }
        public DateTime Termin { get; set; }
        public string? ObrazekUrl { get; set; }
    }

    public class WydarzenieWidok
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string OpisKrotki { get; set; }
        public DateTime Data { get; set; }
        public string? ObrazekUrl { get; set; }
    }
}
