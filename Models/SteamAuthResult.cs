public class SteamAuthResult
{
    public string RedirectUrl { get; }
    public bool IsSuccess { get; }
    public string Message { get; }

    public SteamAuthResult(string redirectUrl, bool isSuccess, string message)
    {
        RedirectUrl = redirectUrl ?? throw new ArgumentNullException(nameof(redirectUrl));
        IsSuccess = isSuccess;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public SteamAuthResult(string redirectUrl)
    {
        RedirectUrl = redirectUrl ?? throw new ArgumentNullException(nameof(redirectUrl));
        IsSuccess = false;
        Message = string.Empty;
    }
}