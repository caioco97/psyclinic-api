# PsyClinic API

API REST da aplicação **PsyClinic**, construída com ASP.NET Core e Identity, com autenticação JWT via cookie HttpOnly.

## Requisitos

- .NET SDK 9.0+
- SQL Server (local ou remoto)

## Configuração

1. Ajuste a string de conexão em `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=...;Database=...;Trusted_Connection=True;TrustServerCertificate=True"
}
```

2. Configure as chaves JWT em `appsettings.json`:

```json
"Jwt": {
  "Key": "sua-chave-secreta-forte",
  "Issuer": "PsyClinic",
  "Audience": "PsyClinic.Frontend",
  "ExpiresMinutes": 60
}
```

## Rodando o projeto

```bash
dotnet restore
dotnet build
dotnet run --project PsyClinic/PsyClinic.Api.csproj
```

Por padrão, a API sobe com Swagger em ambiente de desenvolvimento.

## Migrações (EF Core)

```bash
dotnet ef database update --project PsyClinic.Infrasctructure --startup-project PsyClinic
```

## Autenticação

- O login gera um JWT e salva em cookie `access_token`.
- O cookie é `HttpOnly`, `Secure`, `SameSite=None`.
- O middleware JWT também lê token desse cookie para autenticar requests.

## Endpoints principais

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`

## CORS

A policy atual (`FrontendPolicy`) permite origem:

- `http://localhost:3000`

Se o frontend rodar em outra origem, ajuste em `Program.cs`.

## Tratamento global de erros

A API usa `ExceptionHandlingMiddleware` para padronizar erros inesperados com:

- `statusCode`
- `message`
- `traceId`

## Melhorias recomendadas (próximas)

- Rate limit para `/api/auth/login`
- Health checks e endpoint `/health`
- Observabilidade (OpenTelemetry + métricas)
- Testes automatizados para autenticação e middlewares

---

Se quiser, no próximo passo eu já posso adicionar também um `README` na raiz do repositório com visão geral da arquitetura (Api / Application / Domain / Infrastructure).
