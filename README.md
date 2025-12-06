### API de Gerenciamento de Usuários

#### Descrição
Este projeto implementa uma API REST para gerenciamento de usuários utilizando .NET 8, ASP.NET Core Minimal APIs e Entity Framework Core com SQLite (Code First + Migrations). A solução segue princípios de Clean Architecture, separando responsabilidades entre as camadas Domain, Application, Infrastructure e a API.

A API expõe endpoints para criar, listar, consultar por ID, atualizar e realizar soft delete de usuários, aplicando validações de entrada com FluentValidation e regras de negócio (e-mail único, idade mínima de 18 anos, normalização de e-mail, timestamps automáticos e soft delete).

#### Tecnologias Utilizadas
- .NET 8.0
- ASP.NET Core Minimal APIs
- Entity Framework Core 8
- SQLite
- FluentValidation
- Swashbuckle (Swagger/OpenAPI)

#### Padrões de Projeto Implementados
- Repository Pattern
- Service Pattern
- DTO Pattern
- Dependency Injection

#### Como Executar o Projeto

1) Pré‑requisitos
- .NET SDK 8.0 ou superior instalado

2) Passos
1. Clone o repositório
2. Entre na pasta do projeto
3. (Opcional) Ajuste a connection string em `APIUsuarios/appsettings.json` se desejar
4. Execute as migrations automaticamente ao subir a API (o projeto chama `Database.Migrate()` no startup)
5. Rode a aplicação:
```
dotnet build
dotnet run --project APIUsuarios/APIUsuarios.csproj
```
6. Acesse o Swagger em: `http://localhost:5000/swagger` (ou porta exibida no console)

Observação: O banco SQLite será criado como arquivo `app.db` na raiz do projeto `APIUsuarios/`.

#### Exemplos de Requisições

- Criar usuário (POST /usuarios)
```json
{
  "nome": "Maria Silva",
  "email": "maria.silva@example.com",
  "senha": "segredo123",
  "dataNascimento": "1990-05-10",
  "telefone": "(11) 98888-7777"
}
```

- Atualizar usuário (PUT /usuarios/{id})
```json
{
  "nome": "Maria A. Silva",
  "email": "maria.silva@example.com",
  "dataNascimento": "1990-05-10",
  "telefone": "(11) 98888-7777",
  "ativo": true
}
```

#### Estrutura do Projeto

```
APIUsuarios/
├── Domain/
│   └── Entities/
│       └── Usuario.cs
├── Application/
│   ├── DTOs/
│   │   ├── UsuarioCreateDto.cs
│   │   ├── UsuarioReadDto.cs
│   │   └── UsuarioUpdateDto.cs
│   ├── Interfaces/
│   │   ├── IUsuarioRepository.cs
│   │   └── IUsuarioService.cs
│   ├── Services/
│   │   └── UsuarioService.cs
│   └── Validators/
│       ├── UsuarioCreateDtoValidator.cs
│       └── UsuarioUpdateDtoValidator.cs
├── Infrastructure/
│   ├── Persistence/
│   │   └── AppDbContext.cs
│   └── Repositories/
│       └── UsuarioRepository.cs
├── Migrations/
│   └── (geradas automaticamente via EF)
├── Program.cs
├── appsettings.json
└── APIUsuarios.csproj
```

#### Endpoints
- GET `/usuarios` → 200 OK
- GET `/usuarios/{id}` → 200 OK ou 404 Not Found
- POST `/usuarios` → 201 Created, 400 Bad Request (validação), 409 Conflict (e-mail já cadastrado)
- PUT `/usuarios/{id}` → 200 OK, 400 Bad Request (validação), 404 Not Found, 409 Conflict
- DELETE `/usuarios/{id}` → 204 No Content, 404 Not Found (soft delete: `Ativo=false`)

#### Autor
- Nome: Julia Magnante dos Santos
- Disciplina: Backend - Professor Lucas Fogaça
- Curso: Analise e Desenvolvimento de Sistemas
