using System.ComponentModel.DataAnnotations;

namespace APIUsuarios.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    public DateTime DataNascimento { get; set; }

    public string? Telefone { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; }

    public DateTime? DataAtualizacao { get; set; }
}
