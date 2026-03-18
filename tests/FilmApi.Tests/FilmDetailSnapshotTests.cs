using FilmApi.Models;
using FilmApi.Services;
using FilmApi.Repositories;
using NSubstitute;
using Xunit;

namespace FilmApi.Tests;

/// <summary>
/// État initial du squelette : ce test vérifie un DTO complexe (Film avec Réalisateur, Acteurs, Genres)
/// via une longue série d'Assert.Equal.
/// </summary>
public class FilmDetailSnapshotTests
{
    [Fact]
    public async Task GetById_Returns_Complex_Film_Structure()
    {
        var substituteRepo = Substitute.For<IFilmRepository>();
        var director = new Director
        {
            Id = "dir-1",
            LastName = "Villeneuve",
            FirstName = "Denis",
            Nationality = "CA",
            BirthDate = new DateTime(1967, 10, 3)
        };
        var actors = new List<Actor>
        {
            new() { Id = "a1", LastName = "Chalamet", FirstName = "Timothée", Role = "Paul Atréides" },
            new() { Id = "a2", LastName = "Zendaya", FirstName = "", Role = "Chani" }
        };
        var genres = new List<Genre>
        {
            new() { Id = "g1", Name = "Science-Fiction" },
            new() { Id = "g2", Name = "Aventure" }
        };
        var film = new Film
        {
            Id = "film-abc-123",
            Title = "Dune",
            Summary = "Sur la planète Arrakis...",
            Year = 2021,
            DurationMinutes = 155,
            ReleaseDate = new DateTime(2021, 9, 15),
            Director = director,
            Actors = actors,
            Genres = genres,
            ProductionCountry = new Country { Code = "US", Name = "États-Unis" }
        };
        substituteRepo.GetByIdAsync("film-abc-123").Returns(film);

        var service = new FilmService(substituteRepo);
        var result = await service.GetByIdAsync("film-abc-123");

        await Verify(result)
            .ScrubMembersWithType<DateTime>()
            .ScrubMember("Id");
    }
}
