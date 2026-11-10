using FCG.Users.Application.UseCases;

namespace FCG.Users.Api.Extensions;

/// <summary>
/// Criação opcional do primeiro administrador. DESABILITADA por padrão:
/// só roda com AdministradorInicial:Habilitado=true (senha vinda do Secret).
/// </summary>
public static class AdministradorInicialExtensions
{
    public static async Task CriarAdministradorInicialSeHabilitadoAsync(this WebApplication app)
    {
        var secao = app.Configuration.GetSection("AdministradorInicial");
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("AdministradorInicial");

        if (!secao.GetValue<bool>("Habilitado"))
            return;

        try
        {
            using var escopo = app.Services.CreateScope();
            var useCase = escopo.ServiceProvider.GetRequiredService<CriarAdministradorInicialUseCase>();

            var criado = await useCase.ExecutarAsync(
                secao["Nome"] ?? "Administrador FCG",
                secao["Email"] ?? string.Empty,
                secao["Senha"] ?? string.Empty);

            logger.LogInformation(criado
                ? "Administrador inicial criado."
                : "Já existe administrador cadastrado. Nada a fazer.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Não foi possível criar o administrador inicial.");
        }
    }
}
