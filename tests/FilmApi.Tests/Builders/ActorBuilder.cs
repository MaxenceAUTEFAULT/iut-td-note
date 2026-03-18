using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class ActorBuilder
{
    private string _id = "actor-1";
    private string _lastName = "Smith";
    private string _firstName = "John";
    private string _role = "Main Character";

    public ActorBuilder WithId(string id) { _id = id; return this; }
    public ActorBuilder WithLastName(string lastName) { _lastName = lastName; return this; }
    public ActorBuilder WithFirstName(string firstName) { _firstName = firstName; return this; }
    public ActorBuilder WithRole(string role) { _role = role; return this; }

    public Actor Build() => new Actor
    {
        Id = _id,
        LastName = _lastName,
        FirstName = _firstName,
        Role = _role
    };
}