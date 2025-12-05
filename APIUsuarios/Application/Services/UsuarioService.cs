using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Services;

public class UsuarioService(IUsuarioRepository repo) : IUsuarioService
{
    public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
    {
        var usuarios = await repo.GetAllAsync(ct);
        return usuarios.Select(MapToReadDto);
    }

    public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
    {
        var usuario = await repo.GetByIdAsync(id, ct);
        return usuario is null ? null : MapToReadDto(usuario);
    }

    public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await repo.EmailExistsAsync(email, ct))
            throw new InvalidOperationException("EMAIL_DUPLICADO");

        if (!IsAdult(dto.DataNascimento))
            throw new InvalidOperationException("IDADE_MINIMA_NAO_ATENDIDA");

        var usuario = new Usuario
        {
            Nome = dto.Nome.Trim(),
            Email = email,
            Senha = dto.Senha, // em produção, hashear senha
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Ativo = true
        };

        await repo.AddAsync(usuario, ct);
        await repo.SaveChangesAsync(ct);
        return MapToReadDto(usuario);
    }

    public async Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var usuario = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("USUARIO_NAO_ENCONTRADO");

        var email = dto.Email.Trim().ToLowerInvariant();
        var existente = await repo.GetByEmailAsync(email, ct);
        if (existente is not null && existente.Id != id)
            throw new InvalidOperationException("EMAIL_DUPLICADO");

        if (!IsAdult(dto.DataNascimento))
            throw new InvalidOperationException("IDADE_MINIMA_NAO_ATENDIDA");

        usuario.Nome = dto.Nome.Trim();
        usuario.Email = email;
        usuario.DataNascimento = dto.DataNascimento;
        usuario.Telefone = dto.Telefone;
        usuario.Ativo = dto.Ativo;

        await repo.UpdateAsync(usuario, ct);
        await repo.SaveChangesAsync(ct);
        return MapToReadDto(usuario);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var usuario = await repo.GetByIdAsync(id, ct);
        if (usuario is null) return false;

        // Soft delete
        usuario.Ativo = false;
        await repo.UpdateAsync(usuario, ct);
        await repo.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return repo.EmailExistsAsync(normalized, ct);
    }

    private static bool IsAdult(DateTime birthDate)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age >= 18;
    }

    private static UsuarioReadDto MapToReadDto(Usuario u) => new(
        u.Id,
        u.Nome,
        u.Email,
        u.DataNascimento,
        u.Telefone,
        u.Ativo,
        u.DataCriacao
    );
}
