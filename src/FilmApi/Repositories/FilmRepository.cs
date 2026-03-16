using FilmApi.Models;
using MongoDB.Driver;

namespace FilmApi.Repositories;

public class FilmRepository : IFilmRepository
{
    private readonly IMongoCollection<Film> _collection;

    public FilmRepository(IMongoCollection<Film> collection)
    {
        _collection = collection;
    }

    public async Task<Film> AddAsync(Film film)
    {
        await _collection.InsertOneAsync(film);
        return film;
    }

    public async Task<Film?> GetByIdAsync(string id)
    {
        var cursor = await _collection.FindAsync(f => f.Id == id);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task<(IReadOnlyList<Film> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var totalCount = await _collection.CountDocumentsAsync(FilterDefinition<Film>.Empty);
        var items = await _collection
            .Find(FilterDefinition<Film>.Empty)
            .SortBy(f => f.Id)
            .Skip(skip)
            .Limit(take)
            .ToListAsync();
        return (items, (int)totalCount);
    }
}
