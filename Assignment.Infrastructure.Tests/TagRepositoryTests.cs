namespace Assignment.Infrastructure.Tests;
using Assignment.Core;

public class TagRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.AddRange(new Tag("tag0"){Id = 1},
                         new Tag("tag1"){Id = 2});

        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void tag_create_method_test()
    {
        // Assert
        var tag2 = new TagCreateDTO("tag2");
        
        // Act
        var actual = _repository.Create(tag2);

        // Arrange
        actual.Should().Be((Response.Created, 3));
    }

    [Fact]
    public void tag_delete_method_test()
    {
        // Assert

        // Act
        var actual = _repository.Delete(1, true);

        // Arrange
        actual.Should().Be((Response.Deleted));
    }

    [Fact]
    public void tag_delete_method_notFound_and_conflict_test()
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
    public void tag_find_method_test()
    {
        // Assert
        var expected = new TagDTO(1, "tag0");

        // Act
        var actual = _repository.Find(1);

        // Arrange
        actual.Should().Be(expected);
    }

    [Fact]
    public void tag_find_method_notFound_test()
    {
        // Assert

        // Act
        var actual = _repository.Find(10);

        // Arrange
        actual.Should().Be(null);
    }

    [Fact]
    public void tag_read_method_test()
    {
        // Assert  
        var expected = new[] { new TagDTO(1, "tag0"), new TagDTO(2, "tag1") };

        // Act
        var actual = _repository.Read();

        // Arrange
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void tag_update_method_test()
    {
        // Assert
        var update = new TagUpdateDTO(1, "tag10");

        // Act
        var actual = _repository.Update(update);

        // Arrange
        actual.Should().Be(Response.Updated);
    }

    [Fact]
    public void tag_update_method_test_notFound()
    {
        // Assert
        var update = new TagUpdateDTO(15, "tag999");

        // Act
        var actual = _repository.Update(update);

        // Arrange
        actual.Should().Be(Response.NotFound);
    }
}
