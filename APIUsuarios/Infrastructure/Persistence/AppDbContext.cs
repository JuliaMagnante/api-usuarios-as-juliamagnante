using APIUsuarios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var usuario = modelBuilder.Entity<Usuario>();
        usuario.ToTable("Usuarios");
        usuario.HasKey(u => u.Id);
        usuario.Property(u => u.Nome).IsRequired().HasMaxLength(100);
        usuario.Property(u => u.Email).IsRequired().HasMaxLength(200);
        usuario.HasIndex(u => u.Email).IsUnique();
        usuario.Property(u => u.Senha).IsRequired();
        usuario.Property(u => u.Ativo).HasDefaultValue(true);
        usuario.Property(u => u.DataCriacao).IsRequired();
        usuario.Property(u => u.Telefone);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<Usuario>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.DataCriacao = now;
                entry.Entity.Ativo = entry.Entity.Ativo; // keep default true unless changed explicitly
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.DataAtualizacao = now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
