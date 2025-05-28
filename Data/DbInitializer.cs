using DomKultury.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomKultury.Data
{
    public static class DbInitializer
    {
        public static void Seed(WydarzeniaContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            context.Database.EnsureCreated();

            // ===== ROLĘ =====
            var roles = new[] { "Admin", "Uzytkownik" };
            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    var result = roleManager.CreateAsync(new IdentityRole(role)).Result;
                    Console.WriteLine($"Dodano rolę: {role}");
                }
            }

            // ===== UŻYTKOWNICY =====
            SeedUser(userManager, "admin@domkultury.pl", "Admin123!", "Admin");
            SeedUser(userManager, "moderator@domkultury.pl", "Moderator123!", "Admin");
            SeedUser(userManager, "user@domkultury.pl", "User123!", "Uzytkownik");

            // ===== DANE APLIKACJI (tylko raz) =====
            if (!context.Instruktor.Any())
            {
                Console.WriteLine("Dodawanie danych aplikacji...");

                var instruktorzy = new List<Instruktor>
                {
                    new Instruktor { Imie = "Jan", Nazwisko = "Kowalski", Email = "jan@domkultury.pl" },
                    new Instruktor { Imie = "Anna", Nazwisko = "Nowak", Email = "anna@domkultury.pl" }
                };
                context.Instruktor.AddRange(instruktorzy);
                context.SaveChanges();

                var zajecia = new List<Zajecie>
                {
                    new Zajecie {
                        Nazwa = "Taniec nowoczesny",
                        Opis = "Zajęcia z tańca nowoczesnego",
                        Termin = DateTime.Now.AddDays(5),
                        Lokalizacja = "Sala A",
                        Cena = 60m,
                        MaksymalnaLiczbaUczestnikow = 20,
                        InstruktorId = instruktorzy[0].Id,
                        ObrazekUrl = "https://lh5.googleusercontent.com/proxy/BtJtcjsdSu79jlXEKl8qYza-90cCIZIutxtQojUIYy6IEkmyAXyedlatRv_-rNdFoljS9kfmmOe_915AeQZQUOy8CvGSzc60U8a_D01ertwKdwTKSDTRj0g5pKus",
                        RozszerzonyOpis= "Taniec nowoczesny to ekspresyjna forma tańca scenicznego, która wywodzi się z potrzeby przekraczania sztywnych zasad baletu klasycznego. Zajęcia opierają się na technikach takich jak technika Grahama, Limóna czy Horton, koncentrując się na pracy z oddechem, ciężarem ciała, dynamiką ruchu oraz emocjami. Uczestnicy uczą się świadomego korzystania z podłogi, kontrastów w ruchu (napięcie – rozluźnienie) oraz improwizacji. To zajęcia rozwijające wrażliwość artystyczną, siłę fizyczną oraz umiejętność wyrażania emocji przez ruch. Polecane są zarówno osobom z doświadczeniem tanecznym, jak i tym, którzy dopiero zaczynają swoją przygodę z tańcem współczesnym o klasycznym rodowodzie.\r\n\r\n"
                    },
                    new Zajecie {
                        Nazwa = "Plastyka dla dzieci",
                        Opis = "Zajęcia artystyczne",
                        Termin = DateTime.Now.AddDays(7),
                        Lokalizacja = "Sala B",
                        Cena = 40m,
                        MaksymalnaLiczbaUczestnikow = 15,
                        InstruktorId = instruktorzy[1].Id,
                        ObrazekUrl = "https://mojedziecikreatywnie.pl/wp-content/uploads/2015/12/maski-12-1000x620.jpg",
                        RozszerzonyOpis= "Zajęcia plastyczne dla dzieci to pełne radości spotkania z twórczością, podczas których najmłodsi mają okazję rozwijać wyobraźnię, sprawność manualną oraz wrażliwość estetyczną. Każde zajęcia to nowa przygoda – dzieci poznają różnorodne techniki artystyczne, takie jak malarstwo, rysunek, collage, rzeźba z mas plastycznych czy tworzenie prac z materiałów recyklingowych. Zajęcia prowadzone są w atmosferze swobodnej ekspresji, bez oceniania, co sprzyja budowaniu pewności siebie i zachęca do samodzielnego eksperymentowania. Program dostosowany jest do wieku uczestników, wspiera rozwój motoryki małej, koncentracji i kreatywności. To doskonała forma spędzania czasu, która łączy naukę z zabawą i daje dzieciom przestrzeń do wyrażania siebie poprzez sztukę."
                    }
                };
                context.Zajecie.AddRange(zajecia);
                context.SaveChanges();

                var uczestnicy = new List<Uczestnik>
                {
                    new Uczestnik {
                        Imie = "Mateusz",
                        Nazwisko = "Kwiatkowski",
                        Email = "mateusz@example.com",
                        NumerTelefonu = "123456789",
                        DataRejestracji = DateTime.Now,
                        Zajecia = new List<Zajecie> { zajecia[0] }
                    },
                    new Uczestnik {
                        Imie = "Ola",
                        Nazwisko = "Zielińska",
                        Email = "ola@example.com",
                        NumerTelefonu = "987654321",
                        DataRejestracji = DateTime.Now,
                        Zajecia = new List<Zajecie> { zajecia[1], zajecia[0] }
                    }
                };
                context.Uczestnik.AddRange(uczestnicy);
                context.SaveChanges();

                var kategorie = new List<Kategoria>
                {
                    new Kategoria { Nazwa = "Muzyka" },
                    new Kategoria { Nazwa = "Teatr" },
                };
                context.Kategoria.AddRange(kategorie);
                context.SaveChanges();

                var wydarzenia = new List<Wydarzenie>
                {
                    new Wydarzenie {
                        Nazwa = "Koncert jazzowy",
                        KategoriaId = context.Kategoria.First(k => k.Nazwa == "Muzyka").Id,
                        Organizator = "Dom Kultury",
                        Data = DateTime.Now.AddDays(10),
                        Lokalizacja = "Sala koncertowa",
                        Opis = "Wieczór z muzyką jazzową",
                        Status = true,
                        ObrazekUrl = "https://i.ytimg.com/vi/09sPm7ygjHE/maxresdefault.jpg",
                        RozszerzonyOpis = "Zapraszamy na wyjątkowy koncert jazzowy, podczas którego scena ożyje brzmieniami klasycznego oraz nowoczesnego jazzu w wykonaniu uznanych muzyków. To wieczór pełen emocji, muzycznej wirtuozerii i spontanicznych improwizacji, które przeniosą słuchaczy w świat elegancji i rytmicznej swobody.\r\n\r\nW programie znajdą się zarówno ponadczasowe standardy jazzowe, jak i autorskie kompozycje inspirowane wielkimi mistrzami gatunku. Kameralna atmosfera oraz nastrojowe oświetlenie stworzą idealne warunki do zanurzenia się w dźwiękach saksofonu, kontrabasu, fortepianu i perkusji.\r\n\r\nTo wydarzenie dedykowane nie tylko koneserom jazzu, ale również tym, którzy dopiero odkrywają jego piękno. Gwarantujemy wieczór pełen artystycznych wrażeń i muzycznej magii."
                    },
                    new Wydarzenie {
                        Nazwa = "Spektakl dla dzieci",
                        KategoriaId = context.Kategoria.First(k => k.Nazwa == "Teatr").Id,
                        Organizator = "Teatrzyk XYZ",
                        Data = DateTime.Now.AddDays(15),
                        Lokalizacja = "Sala teatralna",
                        Opis = "Wesoły spektakl dla najmłodszych",
                        Status = true,
                        ObrazekUrl = "https://teatrkatarynka.pl/wp-content/uploads/2025/02/Teatr-Katarynka-Spektakl-dla-dzieci-W-Labiryncie-Niewypowiedzianych-Emocji-Sekretny-Klucz-do-Zrozumienia-46-1024x576.jpeg",
                        RozszerzonyOpis = "Serdecznie zapraszamy najmłodszych widzów wraz z opiekunami na pełen radości i magii spektakl teatralny przygotowany specjalnie z myślą o dzieciach. Przedstawienie łączy elementy interaktywnej opowieści, kolorowej scenografii oraz wpadającej w ucho muzyki, tworząc niezapomniane doświadczenie teatralne.\r\n\r\nSpektakl porusza tematy przyjaźni, odwagi i marzeń – wartości bliskie każdemu dziecku. Aktorzy poprzez zabawę, śmiech i dynamiczną akcję angażują młodą publiczność, zapraszając ją do aktywnego uczestnictwa w historii.\r\n\r\nTo doskonała okazja do rozwijania wyobraźni, empatii i wrażliwości artystycznej najmłodszych. Gwarantujemy bezpieczną, przyjazną atmosferę oraz pozytywne emocje, które na długo pozostaną w pamięci dzieci i rodziców."
                    }
                };
                context.Wydarzenie.AddRange(wydarzenia);
                context.SaveChanges();

                Console.WriteLine("Dane aplikacji dodane.");
            }
            else
            {
                Console.WriteLine("Dane aplikacji już istnieją – pomijam.");
            }
        }

        private static void SeedUser(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            if (userManager.FindByEmailAsync(email).Result == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, password).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, role).Wait();
                    Console.WriteLine($"Użytkownik {email} dodany z rolą {role}");
                }
                else
                {
                    Console.WriteLine($"❌ Błąd tworzenia użytkownika {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
