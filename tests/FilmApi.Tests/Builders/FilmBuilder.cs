using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class FilmBuilder
{
    private string _id = "film-1";
    private string _title = "Default Film";
    private string _summary = "Default summary.";
    private int _year = 2020;
    private int _durationMinutes = 120;
    private DateTime? _releaseDate = null;
    private Director _director = new DirectorBuilder().Build();
    private List<Genre> _genres = new();
    private List<Actor> _actors = new();
    private Country? _productionCountry = null;

    public FilmBuilder WithId(string id) { _id = id; return this; }
    public FilmBuilder WithTitle(string title) { _title = title; return this; }
    public FilmBuilder WithSummary(string summary) { _summary = summary; return this; }
    public FilmBuilder WithYear(int year) { _year = year; return this; }
    public FilmBuilder WithDurationMinutes(int duration) { _durationMinutes = duration; return this; }
    public FilmBuilder WithReleaseDate(DateTime releaseDate) { _releaseDate = releaseDate; return this; }
    public FilmBuilder WithDirector(Director director) { _director = director; return this; }
    public FilmBuilder WithGenres(params Genre[] genres) { _genres = genres.ToList(); return this; }
    public FilmBuilder WithActors(params Actor[] actors) { _actors = actors.ToList(); return this; }
    public FilmBuilder WithProductionCountry(Country country) { _productionCountry = country; return this; }

    public Film Build() => new Film
    {
        Id = _id,
        Title = _title,
        Summary = _summary,
        Year = _year,
        DurationMinutes = _durationMinutes,
        ReleaseDate = _releaseDate,
        Director = _director,
        Genres = _genres,
        Actors = _actors,
        ProductionCountry = _productionCountry
    };
}