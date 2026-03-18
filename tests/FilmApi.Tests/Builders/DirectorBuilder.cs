using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class DirectorBuilder
{
    private string _id = "dir-1";
    private string _lastName = "Doe";
    private string _firstName = "John";
    private string _nationality = "FR";
    private DateTime? _birthDate = null;

    public DirectorBuilder WithId(string id) { _id = id; return this; }
    public DirectorBuilder WithLastName(string lastName) { _lastName = lastName; return this; }
    public DirectorBuilder WithFirstName(string firstName) { _firstName = firstName; return this; }
    public DirectorBuilder WithNationality(string nationality) { _nationality = nationality; return this; }
    public DirectorBuilder WithBirthDate(DateTime birthDate) { _birthDate = birthDate; return this; }

    public Director Build() => new Director
    {
        Id = _id,
        LastName = _lastName,
        FirstName = _firstName,
        Nationality = _nationality,
        BirthDate = _birthDate
    };
}