namespace APIUsuarios.Application.DTOs;

public record UsuarioReadDto(
    int Id,
    string Nome,
    string Email,
    DateTime DataNascimento,
    string? Telefone,
    bool Ativo,
    DateTime DataCriacao
);
