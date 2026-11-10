# FCG — UsersAPI

Microsserviço de **Identidade**: cadastro, autenticação (emissão do token JWT) e autorização (perfis `Usuario` e `Administrador`).

## Modelo de domínio
- **Usuario** (Raiz de Agregação): nome, e-mail, hash da senha, perfil e situação.
- **Email** e **Senha** (Value Objects): normalização/formato do e-mail e política de senha forte (8+ caracteres, letra, número e especial).
- **UsuarioCadastrado** (Evento de Domínio) → traduzido para o evento de integração **UserCreatedEvent**.

## Eventos
| Publica | Routing key | Conteúdo |
|---|---|---|
| `UserCreatedEvent` | `user.created` | `UserId`, `Name`, `Email`, `Role` |

## Endpoints
| Método | Rota | Acesso |
|---|---|---|
| POST | `/api/auth/registrar` | público |
| POST | `/api/auth/login` | público (retorna o JWT) |
| GET | `/api/usuarios/me` | autenticado |
| PUT | `/api/usuarios/me/senha` | autenticado |
| GET/POST | `/api/usuarios` | Administrador |
| GET/PUT | `/api/usuarios/{id}` | Administrador |
| PATCH | `/api/usuarios/{id}/perfil`, `/desativar`, `/ativar` | Administrador |

## Administrador inicial
Desabilitado por padrão. Para criar o primeiro administrador defina `AdministradorInicial:Habilitado=true`
(ConfigMap) e `AdministradorInicial:Senha` (Secret). Só é criado se ainda não existir nenhum administrador.

## Camadas (DDD)

```
src/
  FCG.Users.Domain          Entidades, Value Objects, Agregados, Eventos de Domínio, Serviços de Domínio e interfaces de Repositório
  FCG.Users.Application     Casos de uso, DTOs, Mappers, eventos de integração e seus handlers, portas de saída
  FCG.Users.Infrastructure  EF Core (SQL Server), repositórios, RabbitMQ (RabbitMQ.Client), BCrypt e emissão de JWT
  FCG.Users.Api             Apresentação: controllers, middleware de erros, JWT, Swagger e health checks
tests/
  FCG.Users.Domain.Tests    Testes de unidade das regras de domínio (xUnit)
k8s/                       Deployment, Service, ConfigMap e Secret
Dockerfile                 Build multi-stage (sdk:8.0 -> aspnet:8.0, usuário não-root)
```

O Domínio não referencia nenhum framework. A Infraestrutura implementa as interfaces definidas no Domínio/Aplicação.

## Executar

```powershell
dotnet build FCG.Users.slnx
dotnet test  FCG.Users.slnx
```

- **Local (dotnet run):** suba a infraestrutura com o compose do repositório `fcg-orchestration`
  (`docker compose up sqlserver rabbitmq`) e rode `dotnet run --project src/FCG.Users.Api` (ambiente Development, porta 5101).
- **Docker:** `docker build -t fcg/users-api:1.0.0 .`
- **Kubernetes:** `kubectl apply -f k8s/` (após aplicar namespace e infraestrutura do `fcg-orchestration`).

Health checks: `/health/live` (liveness) e `/health/ready` (readiness: SQL Server e RabbitMQ).
