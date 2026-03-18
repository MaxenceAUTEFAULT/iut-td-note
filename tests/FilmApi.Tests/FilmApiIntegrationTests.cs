using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FilmApi.Models;
using Xunit;

namespace FilmApi.Tests;

/// <summary>
/// Tests d'intégration : HTTP → API → Service → Repository → MongoDB.
/// </summary>
public sealed class FilmApiIntegrationTests : IClassFixture<MongoFixture>, IAsyncLifetime, IDisposable
{
    private readonly MongoFixture _mongo;
    private readonly FilmApiAppFactory _factory;
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public FilmApiIntegrationTests(MongoFixture mongo)
    {
        _mongo = mongo;
        _factory = new FilmApiAppFactory(mongo);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _mongo.InitializeAsync();
        await _mongo.ClearFilmsAsync();
    }

    public void Dispose() => _factory.Dispose();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task POST_films_Returns_201_And_Film()
    {
        // Arrange
        var director = new Director { Id = "d1", LastName = "Dupont", FirstName = "Jean", Nationality = "FR" };
        var request = new CreateFilmRequest(
            Title: "Mon Film",
            Summary: "Résumé.",
            Year: 2024,
            DurationMinutes: 90,
            ReleaseDate: null,
            Director: director,
            Genres: new List<Genre> { new() { Id = "g1", Name = "Drame" } },
            Actors: new List<Actor>(),
            ProductionCountry: new Country { Code = "FR", Name = "France" }
        );

        // Act
        var response = await _client.PostAsJsonAsync("/films", request, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var film = await response.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(film);
        Assert.False(string.IsNullOrEmpty(film.Id));
        Assert.Equal("Mon Film", film.Title);
        Assert.Equal(2024, film.Year);
    }

    [Fact]
    public async Task GET_films_id_Returns_200_After_Post()
    {
        // Arrange
        var director = new Director { Id = "d2", LastName = "Martin", FirstName = "Marie", Nationality = "FR" };
        var request = new CreateFilmRequest(
            "Film pour GET",
            "Résumé GET",
            2023,
            100,
            null,
            director,
            new List<Genre> { new() { Id = "g2", Name = "Comédie" } },
            new List<Actor>(),
            null
        );
        var postResponse = await _client.PostAsJsonAsync("/films", request, JsonOptions);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(created);

        // Act
        var response = await _client.GetAsync($"/films/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var film = await response.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(film);
        Assert.Equal(created.Id, film.Id);
        Assert.Equal("Film pour GET", film.Title);
        Assert.Equal("Martin", film.Director.LastName);
    }
    
        [Fact]
    public async Task GET_films_FilterByReleaseYear_Returns_Only_Matching_Films()
    {
        // Arrange
        var director = new Director { Id = "d3", LastName = "Nolan", FirstName = "Christopher", Nationality = "GB" };
        var film2020a = new CreateFilmRequest(
            "Film 2020 A", "Résumé", 2020, 120,
            ReleaseDate: new DateTime(2020, 3, 15),
            director,
            new List<Genre>(), new List<Actor>(), null
        );
        var film2020b = new CreateFilmRequest(
            "Film 2020 B", "Résumé", 2020, 90,
            ReleaseDate: new DateTime(2020, 11, 1),
            director,
            new List<Genre>(), new List<Actor>(), null
        );
        var film2022 = new CreateFilmRequest(
            "Film 2022", "Résumé", 2022, 100,
            ReleaseDate: new DateTime(2022, 6, 10),
            director,
            new List<Genre>(), new List<Actor>(), null
        );
        await _client.PostAsJsonAsync("/films", film2020a, JsonOptions);
        await _client.PostAsJsonAsync("/films", film2020b, JsonOptions);
        await _client.PostAsJsonAsync("/films", film2022, JsonOptions);

        // Act
        var response = await _client.GetAsync("/films?releaseYear=2020");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<Film>>(JsonOptions);
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, f => Assert.Equal(2020, f.ReleaseDate!.Value.Year));
    }

    [Fact]
    public async Task DELETE_films_id_Film_No_Longer_Exists_In_GET()
    {
        // Arrange
        var director = new Director { Id = "d4", LastName = "Kubrick", FirstName = "Stanley", Nationality = "US" };
        var request = new CreateFilmRequest(
            "Film à supprimer", "Résumé", 2001, 141,
            ReleaseDate: null,
            director,
            new List<Genre>(), new List<Actor>(), null
        );
        var postResponse = await _client.PostAsJsonAsync("/films", request, JsonOptions);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(created);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/films/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var listResponse = await _client.GetAsync("/films");
        var result = await listResponse.Content.ReadFromJsonAsync<PagedResult<Film>>(JsonOptions);
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }
}
