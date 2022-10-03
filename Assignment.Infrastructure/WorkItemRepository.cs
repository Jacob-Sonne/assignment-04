namespace Assignment.Infrastructure;

public class WorkItemRepository : IWorkItemRepository
{
    private readonly KanbanContext _context;

    public WorkItemRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int ItemId) Create(WorkItemCreateDTO item)
    {   
        var dub = _context.Items.Find(item);

        if (dub != null) return (Response.Conflict, dub.Id);
        
        var entity = new WorkItem(item.Title);
        entity.State = State.New;

        _context.Add(entity);
        _context.SaveChanges();
        return (Response.Created, entity.Id);
    }

    public Response Delete(int itemId)
    {
        var entity = _context.Items.Find(itemId);

        if (entity.State == State.New) 
        {_context.Items.Remove(entity); return Response.Deleted;};

        if (entity.State == State.Active) 
        {entity.State = State.Removed; _context.SaveChanges(); return Response.Deleted;} 
        else return Response.Conflict;
    }

    public WorkItemDetailsDTO Find(int itemId)
    {
        var entity = _context.Items.Find(itemId);

        if (entity == null) return null!;

        return new WorkItemDetailsDTO(entity.Id, entity.Title, null!, DateTime.UtcNow, entity.AssignedTo.Name, entity.Tags.Select(x => x.Name).ToList(), entity.State, DateTime.UtcNow);
    }

    public IReadOnlyCollection<WorkItemDTO> Read()
    {
        return _context.Items.Select(u => new WorkItemDTO(u.Id, u.Title, u.AssignedTo.Name, u.Tags.Select(x => x.Name).ToList(), u.State)).ToList();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByState(State state)
    {   
        var entity = from s in Read() where s.State == state select s;

        return entity.Any() ? entity.ToList() : null!;
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByTag(string tag)
    {
        var entity = from t in Read() where t.Tags.Contains(tag) select t;

        return entity.Any() ? entity.ToList() : null!;
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByUser(int userId)
    {
        var entity = from u in Read() where u.Id == userId select u;

        return entity.Any() ? entity.ToList() : null!;
    }

    public IReadOnlyCollection<WorkItemDTO> ReadRemoved()
    {
        return ReadByState(State.Removed);
    }

    public Response Update(WorkItemUpdateDTO item)
    {
        var entity = _context.Items.Find(item);

        if (entity == null) return Response.NotFound;

        entity.State = item.State;
        entity.Title = item.Title;
        entity.AssignedTo = _context.Users.FirstOrDefault(u => u.Id == entity.AssignedToId);

        throw new NotImplementedException();
    }
}
