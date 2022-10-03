namespace Assignment.Infrastructure;

public class TagRepository : ITagRepository
{
    private readonly KanbanContext _context;

    public TagRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int TagId) Create(TagCreateDTO tag)
    {           
        var entity = new Tag(tag.Name);

        _context.Add(entity);
        _context.SaveChanges();
        return (Response.Created, entity.Id);
    }

    public Response Delete(int tagId, bool force = false)
    {
        var entity = _context.Tags.Find(tagId);

        if (entity.WorkItems != null || entity.WorkItems.Count != 0 && !force) return Response.Conflict;
        if (entity == null) return Response.NotFound;

        _context.Tags.Remove(entity);
        _context.SaveChanges();

        return Response.Deleted;
    }

    public TagDTO Find(int tagId)
    {
        var entity = _context.Tags.Find(tagId);

        if (entity == null) return null!;

        return new TagDTO(entity.Id, entity.Name);
    }

    public IReadOnlyCollection<TagDTO> Read()
    {
        return _context.Tags.Select(u => new TagDTO(u.Id, u.Name)).ToList();
    }

    public Response Update(TagUpdateDTO tag)
    {

        var entity = _context.Users.Find(tag);

        if (entity == null) return Response.NotFound;

        entity.Name = tag.Name;

        _context.SaveChanges();
        return Response.Updated;
    }
}
