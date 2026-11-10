using FCG.Users.Api.Extensions;
using FCG.Users.Api.Middlewares;
using FCG.Users.Application.UseCases;
using FCG.Users.Infrastructure;
using FCG.Users.Infrastructure.Messaging;
using FCG.Users.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((contexto, configuracao) => configuracao.ReadFrom.Configuration(contexto.Configuration));

// Presentation
builder.Services.AddControllers();
builder.Services.AdicionarSwagger("FCG UsersAPI", "Cadastro, autenticação (JWT) e autorização de usuários.");
builder.Services.AdicionarAutenticacaoJwt(builder.Configuration);

// Application (casos de uso)
builder.Services.AddScoped<CadastrarUsuarioUseCase>();
builder.Services.AddScoped<AutenticarUsuarioUseCase>();
builder.Services.AddScoped<ConsultarUsuariosUseCase>();
builder.Services.AddScoped<GerenciarUsuarioUseCase>();
builder.Services.AddScoped<CriarAdministradorInicialUseCase>();

// Infrastructure
builder.Services.AdicionarInfraestrutura(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<UsersDbContext>("sqlserver", tags: new[] { ApiExtensions.TagProntidao })
    .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: new[] { ApiExtensions.TagProntidao });

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<TratamentoDeErrosMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapearHealthChecks();

await app.InicializarBancoDeDadosAsync<UsersDbContext>();
await app.CriarAdministradorInicialSeHabilitadoAsync();

app.Run();

public partial class Program { }
