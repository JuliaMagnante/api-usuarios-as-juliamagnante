using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;
using APIUsuarios.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Repositories;

public class UsuarioRepository(AppDbContext db) : IUsuarioRepository
{
    public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct)
        => await db.Usuarios.AsNoTracking().ToListAsync(ct);

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct)
        => await db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct)
        => await db.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AddAsync(Usuario usuario, CancellationToken ct)
        => await db.Usuarios.AddAsync(usuario, ct);

    public Task UpdateAsync(Usuario usuario, CancellationToken ct)
    {
        db.Usuarios.Update(usuario);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Usuario usuario, CancellationToken ct)
    {
        db.Usuarios.Remove(usuario);
        return Task.CompletedTask;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
        => await db.Usuarios.AnyAsync(u => u.Email == email, ct);

    public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
