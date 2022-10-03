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
        var tag3 = new TagCreateDTO("tag3");

        // Act
        var actual = _repository.Delete(1);

        // Arrange
        actual.Should().Be((Response.Deleted));
    }
}
