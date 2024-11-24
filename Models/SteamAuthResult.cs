public class SteamAuthResult
{
    public string RedirectUrl { get; }
    public bool IsSuccess { get; }
    public string Message { get; }
    public string SteamId { get; }
    public string Username { get; }

    // Construtor com todos os parâmetros
    public SteamAuthResult(string redirectUrl, bool isSuccess, string message, string steamId, string username)
    {
        RedirectUrl = redirectUrl ?? throw new ArgumentNullException(nameof(redirectUrl));
        IsSuccess = isSuccess;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        SteamId = steamId ?? throw new ArgumentNullException(nameof(steamId));
        Username = username ?? throw new ArgumentNullException(nameof(username));
    }

    // Construtor simplificado (usado na sua situação)
    public SteamAuthResult(string redirectUrl, string steamId)
    {
        RedirectUrl = redirectUrl ?? throw new ArgumentNullException(nameof(redirectUrl));
        IsSuccess = true;
        Message = "Login realizado com sucesso!";
        SteamId = steamId ?? throw new ArgumentNullException(nameof(steamId));
        Username = "Unknown";
    }
}