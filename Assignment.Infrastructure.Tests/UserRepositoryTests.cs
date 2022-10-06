namespace Assignment.Infrastructure.Tests;
using Assignment.Core;

public class UserRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.AddRange(new User("Bob", "bob@bob.bob"),
                         new User("May", "may@may.may"));

        context.SaveChanges();

        _context = context;
        _repository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void user_create_method_test()
    {
        // Assert
        var creating = new UserCreateDTO("Steve", "steve@steve.steve");

        // Act
        var actual = _repository.Create(creating);

        // Arrange
        actual.Should().Be((Response.Created, 3));
    }

    [Fact]
    public void user_delete_method_test()
    {
        // Assert

        // Act
        var actual = _repository.Delete(1, true);

        // Arrange
        actual.Should().Be((Response.Deleted));
    }

    [Fact]
    public void user_delete_method_notFound_and_conflict_test()
    {
        // Assert

        // Act
        var actual = _repository.Delete(1, false);
        var actual2 = _repository.Delete(10, true);


        // Arrange
        actual.Should().Be((Response.Conflict));
        actual2.Should().Be((Response.NotFound));
    }
    [Fact]
    public void user_find_method_test()
    {
        // Assert
        var expected = new UserDTO(2, "May", "may@may.may");

        // Act
        var actual = _repository.Find(2);

        // Arrange
        actual.Should().Be(expected);
    }

    [Fact]
    public void user_find_method_notFound_test()
    {
        // Assert

        // Act
        var actual = _repository.Find(10);

        // Arrange
        actual.Should().Be(null);
    }

    [Fact]
    public void user_read_method_test()
    {
        // Assert  
        var expected = new[] { new UserDTO(1, "Bob", "bob@bob.bob"), new UserDTO(2, "May", "may@may.may") };

        // Act
        var actual = _repository.Read();

        // Arrange
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void user_update_method_test()
    {
        // Assert
        var update = new UserUpdateDTO(1, "Steve", "steve@steve.steve");

        // Act
        var actual = _repository.Update(update);

        // Arrange
        actual.Should().Be(Response.Updated);
    }

    [Fact]
    public void user_update_method_test_notFound()
    {
        // Assert
        var update = new UserUpdateDTO(15, "Alex", "alex@alex.alex");

        // Act
        var actual = _repository.Update(update);

        // Arrange
        actual.Should().Be(Response.NotFound);
    }
}
