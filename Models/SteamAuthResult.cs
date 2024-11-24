public class SteamAuthResult
{
    public string RedirectUrl { get; }
    public bool IsSuccess { get; }
    public string Message { get; }
    public string SteamId { get; } 

    public SteamAuthResult(string redirectUrl, bool isSuccess, string message, string steamId)
    {
        RedirectUrl = redirectUrl ?? throw new ArgumentNullException(nameof(redirectUrl));
        IsSuccess = isSuccess;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        SteamId = steamId ?? throw new ArgumentNullException(nameof(steamId));
    }

    public SteamAuthResult(string redirectUrl, string steamId)
    {
        RedirectUrl = redirectUrl ?? throw new ArgumentNullException(nameof(redirectUrl));
        IsSuccess = true; 
        Message = "Login realizado com sucesso!";
        SteamId = steamId ?? throw new ArgumentNullException(nameof(steamId));
    }
}
