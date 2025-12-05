using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using FluentValidation;

namespace APIUsuarios.Application.Validators;

public class UsuarioUpdateDtoValidator : AbstractValidator<UsuarioUpdateDto>
{
    public UsuarioUpdateDtoValidator(IUsuarioRepository repo)
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (dto, email, context, ct) =>
            {
                // Normalizar email
                var normalized = (email ?? string.Empty).Trim().ToLowerInvariant();
                // Extrair o id do usuário do contexto
                int currentId = 0;
                if (context.RootContextData.TryGetValue("UserId", out var val) && val is int idFromCtx)
                    currentId = idFromCtx;

                var existing = await repo.GetByEmailAsync(normalized, ct);
                // válido se não existe ou se pertence ao próprio usuário
                return existing is null || existing.Id == currentId;
            })
            .WithMessage("Email já cadastrado para outro usuário");

        RuleFor(x => x.DataNascimento)
            .NotEmpty();

        RuleFor(x => x.Telefone)
            .Matches(@"^\(\d{2}\) \d{5}-\d{4}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefone))
            .WithMessage("Telefone deve estar no formato (XX) XXXXX-XXXX");
    }
}
