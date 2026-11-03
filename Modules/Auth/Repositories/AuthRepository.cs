using Backend.Data;
using Backend.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Auth.Repositories;

public class AuthRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public Task<User?> FindByEmailAsync(string email) => _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> FindByUuidAsync(Guid uuid) => _context.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);

    public Task<bool> EmailExistsAsync(string email) => _context.Users.AnyAsync(u => u.Email == email);

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}