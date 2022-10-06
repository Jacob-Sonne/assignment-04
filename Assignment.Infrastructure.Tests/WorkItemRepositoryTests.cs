namespace Assignment.Infrastructure.Tests;
using Assignment.Core;

public class WorkItemRepositoryTests : IDisposable
{   
    private readonly KanbanContext _context;
    private readonly WorkItemRepository _repository;

    public WorkItemRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.AddRange(new WorkItem("repo0"),
                         new WorkItem("repo1"));

        context.SaveChanges();

        _context = context;
        _repository = new WorkItemRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void workitem_create_method_test()
    {
        // Assert
        var tags = new[] {"tag0", "tag1"};
        var creating = new WorkItemCreateDTO("repo2", 1, "repo2.", tags);

        // Act
        var actual = _repository.Create(creating);

        // Arrange
        actual.Should().Be((Response.Created, 3));
    }

    [Fact]
    public void workitem_delete_method_test()
    {
        // Assert

        // Act
        var actual = _repository.Delete(1);

        // Arrange
        actual.Should().Be((Response.Deleted));
    }

    /*[Fact]
    public void workitem_find_method_test()
    {
        // Assert
        var tags = new[] {"tag0", "tag1"};
        var expected = new WorkItemDetailsDTO(1, "repo0", "repo0.", DateTime.UtcNow, "May", tags, State.New, DateTime.UtcNow);

        // Act
        var actual = _repository.Find(1);

        // Arrange
        actual.Id.Should().Be(expected.Id);
        actual.Title.Should().Be(expected.Title);
        actual.Description.Should().Be(expected.Description);
        actual.Created.Should().BeCloseTo(expected.Created, precision: TimeSpan.FromSeconds(5));
        actual.AssignedToName.Should().Be(expected.AssignedToName);
        actual.Tags.Should().BeEmpty();
        actual.State.Should().Be(expected.State);
        actual.StateUpdated.Should().BeCloseTo(expected.StateUpdated, precision: TimeSpan.FromSeconds(5));
    }*/
}
