using System.Text.Json;
using System.Text.Json.Serialization;
using FCG.Users.Application.Abstractions;

namespace FCG.Users.Infrastructure.Messaging;

/// <summary>Serialização JSON padronizada dos eventos de integração (camelCase, enums como texto).</summary>
public static class SerializadorDeEventos
{
    public static readonly JsonSerializerOptions Opcoes = CriarOpcoes();

    public static string Serializar(IntegrationEvent evento) =>
        JsonSerializer.Serialize(evento, evento.GetType(), Opcoes);

    public static TEvento? Desserializar<TEvento>(string json) =>
        JsonSerializer.Deserialize<TEvento>(json, Opcoes);

    private static JsonSerializerOptions CriarOpcoes()
    {
        var opcoes = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        opcoes.Converters.Add(new JsonStringEnumConverter());
        return opcoes;
    }
}
