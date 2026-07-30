using System.Text.Json.Serialization;

public record CriarUsuarioRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
    
    [JsonIgnore]
    public string NivelAcesso { get; init; } = "cliente";
}