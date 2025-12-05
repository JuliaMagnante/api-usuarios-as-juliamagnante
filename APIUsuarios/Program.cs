using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.Validators;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=app.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// DI
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch
    {
        db.Database.EnsureCreated();
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIUsuarios v1");
});
    

// Minimal API endpoints
app.MapGet("/usuarios", async (IUsuarioService service, CancellationToken ct) =>
{
    var result = await service.ListarAsync(ct);
    return Results.Ok(result);
})
.WithName("ListarUsuarios")
.WithOpenApi();

app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var usuario = await service.ObterAsync(id, ct);
    return usuario is null ? Results.NotFound() : Results.Ok(usuario);
})
.WithName("ObterUsuario")
.WithOpenApi();

app.MapPost("/usuarios", async (UsuarioCreateDto dto, IUsuarioService service, IValidator<UsuarioCreateDto> validator, CancellationToken ct) =>
{
    var validation = await validator.ValidateAsync(dto, ct);
    if (!validation.IsValid)
        return Results.ValidationProblem(validation.ToDictionary());

    // Email duplicado
    var emailExists = await service.EmailJaCadastradoAsync(dto.Email, ct);
    if (emailExists)
        return Results.Conflict(new { message = "Email já cadastrado" });

    var created = await service.CriarAsync(dto, ct);
    return Results.Created($"/usuarios/{created.Id}", created);
})
.WithName("CriarUsuario")
.WithOpenApi();

app.MapPut("/usuarios/{id:int}", async (int id, UsuarioUpdateDto dto, IUsuarioService service, IValidator<UsuarioUpdateDto> validator, CancellationToken ct) =>
{
    var context = new ValidationContext<UsuarioUpdateDto>(dto);
    context.RootContextData["UserId"] = id;
    var validation = await validator.ValidateAsync(context, ct);
    if (!validation.IsValid)
        return Results.ValidationProblem(validation.ToDictionary());

    try
    {
        var updated = await service.AtualizarAsync(id, dto, ct);
        return Results.Ok(updated);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
    catch (InvalidOperationException ex) when (ex.Message == "EMAIL_DUPLICADO")
    {
        return Results.Conflict(new { error = "Email já cadastrado" });
    }
    catch (InvalidOperationException ex) when (ex.Message == "IDADE_MINIMA_NAO_ATENDIDA")
    {
        return Results.BadRequest(new { error = "Usuário deve ter pelo menos 18 anos" });
    }
})
.WithName("AtualizarUsuario")
.WithOpenApi();

app.MapDelete("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var removed = await service.RemoverAsync(id, ct);
    return removed ? Results.NoContent() : Results.NotFound();
})
.WithName("RemoverUsuario")
.WithOpenApi();

app.Run();
