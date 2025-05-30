namespace DomKultury.Models;
using System.ComponentModel.DataAnnotations;

public class Faq
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Pytanie { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Odpowiedz { get; set; }
}

