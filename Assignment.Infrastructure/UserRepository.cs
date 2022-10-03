namespace Assignment.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly KanbanContext _context;

    public UserRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int UserId) Create(UserCreateDTO user)
    {
        var dub = _context.Users.Find(user);

        if (dub != null) return (Response.Conflict, dub.Id);

        var entity = new User(user.Name, user.Email);
        
        _context.Users.Add(entity);
        _context.SaveChanges();
        return (Response.Created, entity.Id);
    }

    public Response Delete(int userId, bool force = false)
    {
        var entity = _context.Users.Find(userId);

        if (entity.Items != null || entity.Items.Count != 0 && !force) return Response.Conflict;
        if (entity == null) return Response.NotFound;

        _context.Users.Remove(entity);
        _context.SaveChanges();
        return (Response.Deleted);
    }

    public UserDTO Find(int userId)
    {
        var entity = _context.Users.Find(userId);

        if (entity == null) return null!;

        return new UserDTO(entity.Id, entity.Name, entity.Email);
    }

    public IReadOnlyCollection<UserDTO> Read()
    {
        return _context.Users.Select(u => new UserDTO(u.Id, u.Name, u.Email)).ToList();
    }

    public Response Update(UserUpdateDTO user)
    {
        var entity = _context.Users.Find(user);

        if (entity == null) return Response.NotFound;

        entity.Name = user.Name;
        entity.Email = user.Email;

        _context.SaveChanges();
        return Response.Updated;
    }
}
